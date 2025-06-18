using ConsentFormEngine.Core.Base;
using ConsentFormEngine.Entities.Enums;

namespace ConsentFormEngine.Entities.Entities
{
    public class User: BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public UserType UserType { get; set; }
        public bool? KvkkConsent { get; set; }
        public FormEntry FormEntry { get; set; } = null!;
    }


}

