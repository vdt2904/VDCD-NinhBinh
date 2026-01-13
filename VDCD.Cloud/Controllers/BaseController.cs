using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;

namespace VDCD.Controllers
{
    public class BaseController : Controller
    {
        private readonly SeoMetaService _seo;
        protected BaseController(SeoMetaService seo)
        {
            _seo = seo;
        }
        protected void ApplySeo(string key)
        {
            var seo = _seo.Get(key);

            ViewData["Title"] = seo.Title;
            ViewData["Description"] = seo.Description;
            ViewData["Keywords"] = seo.Keywords;
            ViewData["NoIndex"] = !seo.Is_Index;
        }

    }
}
