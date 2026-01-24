using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class ContactCreateDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

}
