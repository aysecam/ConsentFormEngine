using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsentFormEngine.Business.DTOs
{
    public class FormEntryCreateRequestDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public Guid CategoryId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool KvkkConsent { get; set; } 
    }

}
