using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Business.ObjectCache
{
    public class SettingCache { 
        public int SettingId { get; set; }
        public string? SettingKey { get; set; }
        public string? Value { get; set; }
        public int? Group { get; set; }
        public string? Description { get; set; }
    }
}
