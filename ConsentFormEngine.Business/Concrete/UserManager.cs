using Azure.Core;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.Constants;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Business.ValidationRules;
using ConsentFormEngine.Core.Aspects.Autofac.Validation;
using ConsentFormEngine.Core.Attributes;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Mailing;
using ConsentFormEngine.Core.Security;
using ConsentFormEngine.Core.Services;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;
using ConsentFormEngine.Entities.Enums;
using static ConsentFormEngine.Business.DTOs.LoginResponseDto;

namespace ConsentFormEngine.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;

        private readonly ICurrentUserService _currentUser;

        public UserManager(IRepository<User> userRepository, IAuthService authService, IMailService mailService, ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _authService = authService;
            _mailService = mailService;
            _currentUser = currentUser;
        }
        [Transactional]
        [ValidationAspect(typeof(CreateUserRequestDtoValidator))]
        public async Task<Result> CreateUserAsync(CreateUserRequestDto dto)
        {
            var existingUser = await _userRepository.GetAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return new Result(false, Messages.EmailAlreadyExists);
            }
            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                UserType = UserType.Admin
            };

            await _userRepository.AddAsync(user);

            return new Result(true, Messages.UserCreated);
        }

        [Transactional]
        public async Task<Result> ForgotOrCreateUserAsync(ForgotOrCreateUserRequestDto dto)
        {
            var user = await _userRepository.GetAsync(u => u.Email == dto.Email);

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

            if (user != null)//şifre sıfırlama
            {
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _userRepository.Update(user);
                return new Result(true, Messages.PasswordUpdated);
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Surname))
                return new Result(false, Messages.NameSurnameRequired);

            var newUser = new User
            {
                Name = dto.Name!,
                Surname = dto.Surname!,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserType = UserType.Admin
            };

            await _userRepository.AddAsync(newUser);
            return new Result(true, Messages.UserCreated);
        }

        public async Task<DataResult<LoggedHttpResponse>> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = await _userRepository.GetAsync(x => x.Email == userForLoginDto.Email);
            if (userToCheck == null)
                return new DataResult<LoggedHttpResponse>(null, false, Messages.UserNotFound);

            if (userToCheck.PasswordHash == null || userToCheck.PasswordSalt == null || !HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new DataResult<LoggedHttpResponse>(null, false, Messages.PasswordIncorrect);
            }

            try
            {
                var createdAccessToken = await _authService.CreateAccessToken(userToCheck);
                var createdRefreshToken = await _authService.CreateRefreshToken(userToCheck, _currentUser.IpAddress);
                var addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);
                await _authService.DeleteOldRefreshTokens(userToCheck.Id);

                return new DataResult<LoggedHttpResponse>(new LoggedHttpResponse
                {
                    AccessToken = createdAccessToken.Token,
                    Expiration = createdAccessToken.Expiration,
                    RefreshToken = addedRefreshToken.Token
                }, true, Messages.LoginSuccess);
            }
            catch (Exception ex)
            {
                return new DataResult<LoggedHttpResponse>(null, false, Messages.LoginFailed);
            }
        }

        public async Task<Result> SendForgotPasswordMail(string mail, string baseUrl)
        {
            var user = await _userRepository.GetAsync(u => u.Email == mail);
            if (user == null)
                return new Result(false, "Kullanıcı bulunamadı.");

            var newPassword = Guid.NewGuid().ToString("N")[..8].ToUpper();

            HashingHelper.CreatePasswordHash(newPassword, out var hash, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            await _userRepository.Update(user);
            var loginUrl = baseUrl + "/login";
            var htmlBody = $@"
                <html lang='tr'>
                <head>
                  <meta charset='UTF-8'>
                  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                </head>
                <body style='margin:0;padding:0;background-color:#f9f9f9;'>
                  <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='padding: 20px 0;'>
                    <tr>
                      <td align='center'>
                        <table border='0' cellpadding='0' cellspacing='0' width='600' style='background-color:#ffffff;border-radius:8px;padding:30px;box-shadow:0 2px 8px rgba(0,0,0,0.05);font-family:Arial,sans-serif;color:#333333;'>
                          <tr>
                            <td style='text-align:left;'>
                              <h2 style='margin-bottom:20px;'>Sayın {user.Name} {user.Surname.ToUpper()},</h2>
                              <p style='font-size:14px;line-height:22px;'>Yeni şifreniz aşağıda yer almaktadır:</p>
                              <p style='font-size:16px;line-height:22px; font-weight: bold; margin-top:10px; color: #007BFF;'>{newPassword}</p>
                              <p style='margin-top:20px;font-size:14px;line-height:22px;'>Aşağıdaki bağlantıya tıklayarak giriş yapabilirsiniz:</p>
                              <p style='text-align:center;margin:30px 0;'>
                                <a href='{loginUrl}' style='background-color:#007BFF;color:#ffffff;padding:12px 25px;text-decoration:none;border-radius:5px;font-size:16px;'>Giriş Yap</a>
                              </p>
                              <hr style='border:none;border-top:1px solid #eaeaea;margin:40px 0'>
                              <p style='font-size:12px;color:#999;'>Bu e-posta Medlog sistemleri tarafından otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
                              <p style='text-align:center;color:#aaa;font-size:12px;margin-top:20px;'>© {DateTime.Now.Year} Medlog | Tüm hakları saklıdır.</p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>";


            var mailObj = new Mail
            {
                Subject = "Şifre Sıfırlama",
                HtmlBody = htmlBody,
                ToList = user.Email
            };

            _mailService.SendMail(mailObj);

            return new Result(true, Messages.SendMailforForgotPassword);
        }

    }
}
