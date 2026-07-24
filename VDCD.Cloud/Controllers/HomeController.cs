using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using VDCD.Business.Service;
using VDCD.Cloud.Models;
using VDCD.Controllers;
using VDCD.Entities.Custom;
using VDCD.Entities.DTO;
using VDCD.Models;
using static System.Net.Mime.MediaTypeNames;

namespace VDCD.Cloud.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserBll userService;
        private readonly SettingService _settingService;
        private readonly CenterService _centerService;
        private readonly ProjectService _projectService;
        private readonly CategoryService _categoryService;  
        private readonly PostsService _postsService;  
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private readonly UserDepartmentJobtitlePositionService _userDepartmentJobtitlePositionService;
        private readonly DepartmentService _departmentService;
        private readonly SeoMetaService _seoMetaService;

        public HomeController(ILogger<HomeController> logger,UserBll userBll, SettingService settingService,CenterService centerService
            , SeoMetaService seoMetaService, ProjectService projectService, CategoryService categoryService,PostsService postsService,
            CustomerService customerService, UserService service,UserDepartmentJobtitlePositionService userDepartmentJobtitlePositionService,
            DepartmentService departmentService,SeoMetaService metaService) : base(seoMetaService)
        {
            _logger = logger;
            userService = userBll;
            _settingService = settingService;   
            _centerService = centerService;
            _projectService = projectService;
            _categoryService = categoryService;
            _postsService = postsService;
            _customerService = customerService;
            _userService = service;
            _departmentService = departmentService;
            _userDepartmentJobtitlePositionService = userDepartmentJobtitlePositionService;
            _seoMetaService = seoMetaService;
        }

        public IActionResult Index()
        {
            ApplySeo("home");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var homeModel = new HomeModelView
            {
                Settings = settingsDic, // Gán vào một thuộc tính Dictionary trong ViewModel
                Centers = _centerService.GetAll().ToList(),
                Projects = _projectService.GetAll().ToList(),
                Blogs = _postsService.GetAll().Take(6).ToList(),
                Customers = _customerService.GetAll().Where(x=>x.IsShow == true).ToList(),
            };

            return View(homeModel);
        }
        public IActionResult Center()
        {
            ApplySeo("he-thong-trung-tam");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var Data = new CenterModelView
            {
                Settings = settingsDic,
                Centers = _centerService.GetAll().ToList(),
            };
            return View(Data);
        }
        public IActionResult Abouts()
        {
            ApplySeo("Ve-chung-toi");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var homeModel = new HomeModelView
            {
                Settings = settingsDic, // Gán vào một thuộc tính Dictionary trong ViewModel
                Centers = _centerService.GetAll().ToList(),
                Projects = _projectService.GetAll().ToList(),
                Blogs = _postsService.GetAll().Take(6).ToList(),
                Customers = _customerService.GetAll().Where(x => x.IsShow == true).ToList(),
            };

            return View(homeModel);
        }
        public IActionResult Organizational()
        {
            ApplySeo("co-cau-to-chuc");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var allUsers = _userService.GetUsers().ToList();
            var allDepts = _departmentService.Gets();
            var allUserDepts = _userDepartmentJobtitlePositionService.Gets();
            var joinedUsers = (from u in allUsers
                               join ud in allUserDepts on u.UserId equals ud.UserId into userDeptGroup
                               from ud in userDeptGroup.DefaultIfEmpty() // Left Join để không mất User nếu chưa gán phòng ban
                               join d in allDepts on ud?.DepartmentId equals d.Id into deptGroup
                               from d in deptGroup.DefaultIfEmpty()
                               where ud == null || ud.IsMain == true // Chỉ lấy phòng ban chính (IsMain)
                               select new UserResponse
                               {
                                   UserId = u.UserId,
                                   FullName = u.FullName,
                                   Avatar = u.Avatar,
                                   Profile = u.Profile,
                                   IsShow = u.IsShow,
                                   IsActive = u.IsActive,
                                   // Gán DepartmentName từ bảng Department
                                   DepartmentName = d?.DepartmentName ?? "N/A"
                               }).ToList();
            var Data = new OrganizationalModelView
            {
                Settings= settingsDic,
                Users = joinedUsers,   
            };
            return View(Data);
        }
        public JsonResult GetUsers()
        {
            var lstU = userService.GetAllActiveUsers();
            return Json(lstU);
        }
        public JsonResult GetHeader()
        {
            var lstSetting = _settingService.GetAll();


            var footer = lstSetting
                .Where(x => x.SettingKey.StartsWith("setting.general.footer."))
                .OrderBy(x => x.SettingKey)
                .ToList();

            var slice = lstSetting
                .Where(x => x.SettingKey.StartsWith("setting.general.sliders."))
                .AsEnumerable() // Chuyển về bộ nhớ để xử lý chuỗi
                .OrderBy(x => {
                    var parts = x.SettingKey.Split('.');
                    int index;
                    return int.TryParse(parts.Last(), out index) ? index : 0;
                })
                .ToList();

            return Json(new
            {
                success = true,
                footer = footer,
                slice = slice
            });
        }
        public IActionResult News()
        {
            ApplySeo("tin-tuc");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetListNews(string search = "", int page = 1, int pageSize = 6, bool getAll = false)
        {
            try
            {
                // 1. Lấy Query cơ sở (chưa thực thi xuống DB)
                var query = _postsService.GetAll(search);

                // 2. Tính toán tổng số lượng để phân trang
                int totalItems = query.Count();

                // 3. Xử lý logic lấy toàn bộ hoặc lấy theo trang
                if (!getAll)
                {
                    query = (IReadOnlyList<Posts>)query.Skip((page - 1) * pageSize).Take(pageSize);
                }

                // 4. Lấy danh sách Categories một lần duy nhất để map (Tránh N+1)
                var categories = _categoryService.GetAll().ToDictionary(x => x.Id, x => x.CategoryName);

                // 5. Thực thi truy vấn và Map dữ liệu
                var data = query.ToList().Select(a => new
                {
                    Title = a.Title,
                    Summary = a.Summary, // Sửa từ Description để khớp với view cũ của bạn
                    Thumbnail = a.Thumbnail,
                    CategoryName = categories.ContainsKey((int)a.CategoryId) ? categories[(int)a.CategoryId] : "TỔNG HỢP",
                    PublishedDate = a.PublishedDate,
                    // Giả sử bạn cần Slug để làm link chi tiết
                    Slug = a.Slug
                }).ToList();

                // 6. Trả về kết quả kèm thông tin phân trang
                return Ok(new
                {
                    success = true,
                    data = data,
                    totalItems = totalItems,
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                    currentPage = page,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Contact()
        {
            ApplySeo("lien-he");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            return View();
        }
        public IActionResult Careers()
        {
            return View();
        }
        public IActionResult Sitemap()
        {
            var seos = _seoMetaService.Gets();

            var map = new Dictionary<string, string>
    {
        { "project", "du-an" },
        { "jobposition", "tuyen-dung" },
        { "post", "tin-tuc" }
    };

            var urls = seos
                .Where(x => x.Is_Index)
                .Select(x =>
                {
                    var key = x.Seo_Key;

                    foreach (var m in map)
                    {
                        if (key.StartsWith(m.Key + ":"))
                        {
                            key = key.Replace(m.Key + ":", m.Value + "/");
                            break;
                        }
                    }

                    return $"https://vdcd.site/{key}";
                })
                .ToList();

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var xml = new XDocument(
                new XElement(ns + "urlset",
                    urls.Select(url =>
                        new XElement(ns + "url",
                            new XElement(ns + "loc", url),
                            new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                            new XElement(ns + "priority", "0.8")
                        )
                    )
                )
            );

            return Content(xml.ToString(), "application/xml");
        }
        public IActionResult privacypolicy()
        {
            return View();
        }
        public IActionResult datadeletion()
        {
            return View();
        }
    }
}
