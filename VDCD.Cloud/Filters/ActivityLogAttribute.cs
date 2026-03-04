using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VDCD.Entities.Enums;
using VDCD.Business.Infrastructure;

namespace VDCD.Business.Infrastructure.Filters
{
    public class ActivityLogAttribute : ActionFilterAttribute
    {
        private readonly ActivityLogType _type;
        private readonly string _actionName;

        public ActivityLogAttribute(ActivityLogType type, string actionName)
        {
            _type = type;
            _actionName = actionName;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (resultContext.Exception == null)
            {
                var logService =
                    context.HttpContext.RequestServices
                    .GetRequiredService<IActivityLogService>();

                await logService.LogAsync(
                    _type,
                    _actionName,
                    context.HttpContext
                );
            }
        }
    }
}