namespace ConsentFormEngine.Business.DTOs
{
    public class ForgotOrCreateUserRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
