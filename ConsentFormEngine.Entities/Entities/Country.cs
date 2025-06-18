using System.ComponentModel.DataAnnotations;

namespace ConsentFormEngine.Entities.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Rewrite { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string AreaCode { get; set; } = null!;
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
