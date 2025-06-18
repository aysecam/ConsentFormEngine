using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsentFormEngine.Entities.Entities
{
    public class City 
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; } = null!;
        [Column("CountryId")]
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
    }
}
