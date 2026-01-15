using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class SeoMeta
    {
        public long Id { get; set; }

        // định danh trang (home, service:slug, project:slug)
        public string Seo_Key { get; set; } = null!;

        // Google title
        public string Title { get; set; } = null!;

        // Google description
        public string Description { get; set; } = null!;

        public string? Keywords { get; set; }

        // có cho Google index không
        public bool Is_Index { get; set; } = true;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
