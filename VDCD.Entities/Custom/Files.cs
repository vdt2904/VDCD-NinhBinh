using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class Files
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? path { get; set; }
        public long? parent_id { get; set; }
        public bool IsFolder { get; set; }
        public string? content_type { get; set; }
        public long? size { get; set; }
        public DateTime? created_at {  get; set; }
    }
}
