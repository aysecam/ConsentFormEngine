namespace ConsentFormEngine.Core.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } 
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

    }
}
