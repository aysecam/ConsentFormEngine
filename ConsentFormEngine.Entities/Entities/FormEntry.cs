using ConsentFormEngine.Core.Base;

namespace ConsentFormEngine.Entities.Entities
{
    public class FormEntry: BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
        public string? Title { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public Category? Category { get; set; } 
        public User User { get; set; } = null!;

    }
}
