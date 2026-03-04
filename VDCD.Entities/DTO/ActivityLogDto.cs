using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string TypeText { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}
