using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class ActivityLogSearchRequest
    {
        public string? Content { get; set; }     
        public string? TypeText { get; set; }  

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
