using VDCD.Entities.Custom;

namespace VDCD.Helper
{
    public class Helper
    {
        public string CurrentUser(HttpContext context)
        {
            return context?.User?.Identity?.Name;
        }
    }
}
