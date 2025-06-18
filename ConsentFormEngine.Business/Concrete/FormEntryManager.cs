using AutoMapper;
using Microsoft.Extensions.Configuration;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.Constants;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Attributes;
using ConsentFormEngine.Core.Helpers;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Shared;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Core.Utilities.Expressions;
using ConsentFormEngine.Entities.Entities;
using ConsentFormEngine.Entities.Enums;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace ConsentFormEngine.Business.Concrete
{
    public class FormEntryManager : IFormEntryService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<FormEntry> _formEntryRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public FormEntryManager(IRepository<User> userRepository, IRepository<FormEntry> formEntryRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _formEntryRepository = formEntryRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        [Transactional]
        public virtual async Task<Result> SubmitFormAsync(FormEntryCreateRequestDto dto)
        {
            if (!dto.KvkkConsent)
                return new Result(false, Messages.KvkkConsentRequired);

            var existingUser = await _userRepository.GetAsync(u => u.Email == dto.Email);
            User user;

            bool isNewUser = false;

            if (existingUser == null)
            {
                user = new User
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    KvkkConsent = dto.KvkkConsent,
                    UserType = UserType.Guest
                };

                user = await _userRepository.AddAsync(user);
                isNewUser = true;
            }
            else
            {
                user = existingUser;
            }

            var formEntry = new FormEntry
            {
                UserId = user.Id,
                PhoneNumber = dto.Phone,
                CategoryId = dto.CategoryId,
                Company = dto.Company?.ToUpper(),
                Title = dto.Title?.ToUpper(),
                Country = dto.Country.ToString().ToUpper(),
                City = dto.City.ToString().ToUpper()
            };

            var addedForm = await _formEntryRepository.AddAsync(formEntry);
            if (addedForm == null)
                return new Result(false, Messages.FormSaveFailed);

            // Sadece yeni kullanıcı için KVKK gönder
            if (isNewUser)
            {
                await SendKvkk(
                    $"{user.Name} {user.Surname}",
                    user.Email,
                    (int)EnumKvkkDocumentType.ExplicitConsentText,
                    formEntry.Company!
                );
            }

            return new Result(true, Messages.FormAndUserSaveSuccess);
        }

        private async Task SendKvkk(string name, string mail, int? kvkkDocumentType, string projectName)
        {
            string dataProtectionText = "";
            string url = _configuration["Syslog:Uat"]!;  //TODO syslog kısmı
            var httpClient = new HttpClient();
            var apiClient = new ApiClient(httpClient);


            dataProtectionText = "KVKK Aydınlatma Metni - ";

            var data = new
            {
                System = "",
                FullName = name,
                Email = mail,
                DataProtectionText = dataProtectionText
            };

            var jsonData = JsonConvert.SerializeObject(data);
            await apiClient.PostAsync(url, jsonData);
        }


        public async Task<DataResult<PagedList<GetFormEntryReportResponseDto>>> GetFormEnrtyReport(GetFormEntryReportRequestDto dto)
        {
            var searchExpr = SearchExpressionBuilder.BuildSearchExpression<FormEntry>(
                dto.PageRequest?.SearchText ?? "",
                x => x.User.Name!,
                x => x.User.Surname!,
                x => x.User.Email!,
                x => x.Company!,
                x => x.Title!
            );

            var result = await _formEntryRepository.GetPagedAsync<FormEntry, GetFormEntryReportResponseDto>(
                dto.PageRequest!,
                filter: f => f.CreatedDate >= dto.StartDate && f.CreatedDate <= dto.EndDate,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                selector: f => new GetFormEntryReportResponseDto
                {
                    Name = f.User.Name,
                    Surname = f.User.Surname,
                    Email = f.User.Email,
                    PhoneNumber = f.PhoneNumber!,
                    Category = f.Category!.Name,
                    Title = f.Title!,
                    Company = f.Company!,
                    Country = f.Country!,
                    City = f.City!
                },
                includes: new Expression<Func<FormEntry, object>>[]
                {
                    f => f.User,
                    f => f.Category!
                });

            return new DataResult<PagedList<GetFormEntryReportResponseDto>>(result, true, Messages.ListedSuccessfull);
        }

    }
}
