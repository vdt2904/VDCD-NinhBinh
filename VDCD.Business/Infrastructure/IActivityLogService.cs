using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Entities.DTO;
using VDCD.Entities.Enums;

namespace VDCD.Business.Infrastructure
{
    public interface IActivityLogService    
    {
        Task LogAsync(
            ActivityLogType type,
            string content,
            HttpContext httpContext
            //int? userId = null,
            //string? userName = null
        );

        Task<PagedResult<ActivityLogDto>> GetPagedAsync(
        int page,
        int pageSize);

        Task<PagedResult<ActivityLogDto>> SearchAsync(
            ActivityLogSearchRequest req);
    }
}
