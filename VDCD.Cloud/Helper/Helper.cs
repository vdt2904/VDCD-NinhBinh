using VDCD.Entities.Custom;

namespace VDCD.Helper
{
    public static class Helper
    {
        public static string CurrentUser(HttpContext context)
        {
            return context?.User?.Identity?.Name;
        }
    }
}
