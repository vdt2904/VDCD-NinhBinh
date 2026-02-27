using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Custom;
using VDCD.Entities.Enums;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VDCD.Entities.DTO;
using VDCD.Helper;

namespace VDCD.Business.Service
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _db;
        private readonly IRepository<ActivityLog> _repo;

        public ActivityLogService(
            AppDbContext db,
            IRepository<ActivityLog> repo)
        {
            _db = db;
            _repo = repo;
        }

        public async Task LogAsync(
            ActivityLogType type,
            string content,
            HttpContext httpContext)
        {
            var user = httpContext.User;

            int? userId = null;
            string? userName = null;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim =
                    user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? user.FindFirst("sub");

                if (userIdClaim != null &&
                    int.TryParse(userIdClaim.Value, out var id))
                {
                    userId = id;
                }

                userName =
                    user.FindFirst(ClaimTypes.Name)?.Value
                    ?? user.Identity?.Name;
            }

            var ip =
                httpContext.Request.Headers["X-Forwarded-For"]
                    .FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString();

            var log = new ActivityLog
            {
                ActivityLogType = (byte)type,
                Content = content,
                Ip = ip,
                UserId = userId,
                UserName = userName,
                CreatedOnDate = DateTime.UtcNow
            };

            _repo.Create(log);
            await _db.SaveChangesAsync();
        }

        // ⭐ GET danh sách log (không filter)
        public async Task<PagedResult<ActivityLogDto>> GetPagedAsync(
            int page,
            int pageSize)
        {
            var query = _repo.Raw.AsNoTracking();

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedOnDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => ToDto(x))
                .ToListAsync();

            return new PagedResult<ActivityLogDto>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Data = data
            };
        }

        // ⭐ SEARCH nâng cao
        public async Task<PagedResult<ActivityLogDto>> SearchAsync(
            ActivityLogSearchRequest req)
        {
            var query = _repo.Raw.AsNoTracking();

            // 🔎 Filter Content
            if (!string.IsNullOrWhiteSpace(req.Content))
            {
                query = query.Where(x =>
                    x.Content.Contains(req.Content));
            }

            // 🏷️ Filter TypeText
            if (!string.IsNullOrWhiteSpace(req.TypeText))
            {
                var typeEnum =
                    EnumHelper.FromDescription<ActivityLogType>(req.TypeText);

                if (typeEnum.HasValue)
                {
                    byte typeValue = (byte)typeEnum.Value;

                    query = query.Where(x =>
                        x.ActivityLogType == typeValue);
                }
                else
                {
                    query = query.Where(x => false);
                }
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedOnDate)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => ToDto(x))
                .ToListAsync();

            return new PagedResult<ActivityLogDto>
            {
                Total = total,
                Page = req.Page,
                PageSize = req.PageSize,
                Data = data
            };
        }

        // ⭐ Map Entity → DTO
        private static ActivityLogDto ToDto(ActivityLog x)
        {
            return new ActivityLogDto
            {
                Id = x.ActivityLogId,
                Content = x.Content,
                UserName = x.UserName,
                Ip = x.Ip,
                CreatedOnDate = x.CreatedOnDate,
                TypeText =
                    ((ActivityLogType)x.ActivityLogType)
                    .GetDescription()
            };
        }
    }
}