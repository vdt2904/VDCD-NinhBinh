using Hangfire;
using System;
using Hangfire.MySql;
using System.Transactions;
using Microsoft.EntityFrameworkCore;


using VDCD.Business;
using VDCD.Business.Infrastructure;
using VDCD.Business.Service;
using VDCD.DataAccess;
using VDCD.Hubs;
using Microsoft.AspNetCore.Http.Features;
using VDCD.Entities.Security;

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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSignalR();
// Realtime adapter
builder.Services.AddScoped<IRealtimeNotifier, SignalRNotifier>();

// 2. Cấu hình Hangfire sử dụng MySQL
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlStorageOptions
        {
            // Chỉ giữ lại các thuộc tính cơ bản và ổn định nhất
            TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
            QueuePollInterval = TimeSpan.FromSeconds(15),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            PrepareSchemaIfNecessary = true,
            TablesPrefix = "Hangfire_"
        })));

// 3. Chạy Hangfire Server (đối tượng xử lý các Job chạy ngầm)
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1;
});

// Ví dụ cấu hình cho phép file lớn trong ASP.NET Core
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});
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
builder.Services.AddHttpClient();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS user_roles (
            id INT NOT NULL AUTO_INCREMENT,
            user_id INT NOT NULL,
            role_name VARCHAR(50) NOT NULL DEFAULT 'Viewer',
            created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME NULL,
            PRIMARY KEY (id),
            UNIQUE KEY uq_user_roles_user_id (user_id),
            CONSTRAINT fk_user_roles_users_user_id FOREIGN KEY (user_id) REFERENCES users (UserId) ON DELETE CASCADE
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        """);

    db.Database.ExecuteSqlRaw($"""
        INSERT INTO user_roles (user_id, role_name, created_at)
        SELECT u.UserId, '{AdminRoles.SuperAdmin}', NOW()
        FROM users u
        LEFT JOIN user_roles ur ON ur.user_id = u.UserId
        WHERE LOWER(u.UserName) = 'admin' AND ur.id IS NULL;
        """);

    db.Database.ExecuteSqlRaw($"""
        INSERT INTO user_roles (user_id, role_name, created_at)
        SELECT u.UserId, '{AdminRoles.SuperAdmin}', NOW()
        FROM users u
        LEFT JOIN user_roles ur ON ur.user_id = u.UserId
        WHERE ur.id IS NULL
          AND NOT EXISTS (SELECT 1 FROM user_roles WHERE role_name = '{AdminRoles.SuperAdmin}')
        ORDER BY u.UserId
        LIMIT 1;
        """);

    var seedDemoRbacOnStartup = builder.Configuration.GetValue<bool>("Security:SeedDemoRbacOnStartup");
    if (seedDemoRbacOnStartup)
    {
        var resetDemoPasswords = builder.Configuration.GetValue<bool>("Security:ResetDemoRbacPasswordsOnStartup");
        var demoSeeder = scope.ServiceProvider.GetRequiredService<RbacDemoSeedService>();
        demoSeeder.SeedDemoUsers(resetDemoPasswords);
    }
}

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
app.UseAuthentication();
app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
{
    // Cho phép tất cả mọi người truy cập (Chỉ dùng khi test, sau này nên thêm Filter)
    Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
});
app.UseAuthorization();
app.MapControllerRoute(
    name: "center",
    pattern: "he-thong-trung-tam",
    defaults: new { controller = "Home", action = "Center" }
);
app.MapControllerRoute(
    name: "sitemap",
    pattern: "sitemap.xml",
    defaults: new { controller = "Home", action = "sitemap" }
);
app.MapControllerRoute(
    name: "privacy-policy",
    pattern: "privacy-policy",
    defaults: new { controller = "Home", action = "privacypolicy" }
);
app.MapControllerRoute(
    name: "data-deletion",
    pattern: "data-deletion",
    defaults: new { controller = "Home", action = "datadeletion" }
);
app.MapControllerRoute(
    name: "carees",
    pattern: "tuyen-dung",
    defaults: new { controller = "Careers", action = "index" }
);
app.MapControllerRoute(
    name: "carees_detail",
    pattern: "tuyen-dung/{slug}",
    defaults: new { controller = "Careers", action = "Details" }
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
app.MapHub<NotificationHub>("/hub/notification");
app.Run();
