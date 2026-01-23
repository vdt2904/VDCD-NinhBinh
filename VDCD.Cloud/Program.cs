using Microsoft.EntityFrameworkCore;
using System;
using VDCD.Business;
using VDCD.Business.Infrastructure;
using VDCD.Business.Service;
using VDCD.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheSevice>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
// DI service
builder.Services.AddBusinessServices();
/*builder.Services.AddScoped<UserBll>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<FileManagerService>();
builder.Services.AddScoped<CacheSevice>();
builder.Services.AddScoped<CategoryService>();*/

builder.Services.AddControllersWithViews();
// authen riêng cho admin
builder.Services.AddAuthentication("AdminAuth")
    .AddCookie("AdminAuth", options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "center",
    pattern: "he-thong-trung-tam",
    defaults: new { controller = "Home", action = "Center" }
);
app.MapControllerRoute(
    name: "category_detail",
    pattern: "danh-muc/{slug}",
    defaults: new { controller = "Category", action = "Index" }
);
app.MapControllerRoute(
    name: "post",
    pattern: "tin-tuc/{slug}",
    defaults: new { controller = "Posts", action = "Index" }
);
app.MapControllerRoute(
    name: "project_detail",
    pattern: "du-an/{slug}",
    defaults: new { controller = "Projects", action = "Project" }
);
app.MapControllerRoute(
    name: "Organizational",
    pattern: "co-cau-to-chuc",
    defaults: new { controller = "Home", action = "Organizational" }
);
app.MapControllerRoute(
    name: "News",
    pattern: "tin-tuc",
    defaults: new { controller = "Home", action = "News" }
);
app.MapControllerRoute(
    name: "Contact",
    pattern: "lien-he",
    defaults: new { controller = "Home", action = "Contact" }
);
app.MapControllerRoute(
    name: "abouts",
    pattern: "ve-chung-toi",
    defaults: new { controller = "Home", action = "Abouts" }
);
app.MapControllerRoute(
    name: "project",
    pattern: "du-an",
    defaults: new { controller = "Projects", action = "Index" }
);
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
