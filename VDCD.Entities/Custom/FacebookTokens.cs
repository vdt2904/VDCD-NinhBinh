using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class FacebookToken
    {
        public int Id { get; set; }

        public string? PageId { get; set; }
        public string? PageName { get; set; }

        public string? PageAccessToken { get; set; }

        public string? UserAccessToken { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastRefreshDate { get; set; }

        public bool? IsActive { get; set; }
    }
}
