using VDCD.Business.Infrastructure;
using VDCD.Business.Security;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;

namespace VDCD.Business.Service;

public sealed class RbacDemoSeedService
{
    private readonly IRepository<User> _userRepo;
    private readonly UserRoleService _userRoleService;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;

    public RbacDemoSeedService(
        IRepository<User> userRepo,
        UserRoleService userRoleService,
        ICacheService cacheService,
        AppDbContext context)
    {
        _userRepo = userRepo;
        _userRoleService = userRoleService;
        _cacheService = cacheService;
        _context = context;
    }

    public RbacDemoSeedResult SeedDemoUsers(bool resetPasswords = false)
    {
        var createdUsers = new List<string>();
        var updatedUsers = new List<string>();
        var demoAccounts = BuildDefaultDemoAccounts();

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            foreach (var account in demoAccounts)
            {
                var user = _userRepo.Get(false, x => x.UserName == account.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = account.UserName,
                        FullName = account.FullName,
                        Mail = account.Email,
                        Phone = account.Phone,
                        IsActive = true,
                        IsShow = false,
                        CreateAt = DateTime.Now,
                        HashPassword = PasswordSecurity.HashPassword(account.Password)
                    };
                    _userRepo.Create(user);
                    _context.SaveChanges();
                    createdUsers.Add(account.UserName);
                }
                else
                {
                    var hasChange = false;
                    if (user.IsActive != true)
                    {
                        user.IsActive = true;
                        hasChange = true;
                    }

                    if (string.IsNullOrWhiteSpace(user.FullName))
                    {
                        user.FullName = account.FullName;
                        hasChange = true;
                    }

                    if (string.IsNullOrWhiteSpace(user.Mail))
                    {
                        user.Mail = account.Email;
                        hasChange = true;
                    }

                    if (resetPasswords || string.IsNullOrWhiteSpace(user.HashPassword))
                    {
                        user.HashPassword = PasswordSecurity.HashPassword(account.Password);
                        hasChange = true;
                    }

                    if (hasChange)
                    {
                        _userRepo.Update(user);
                        _context.SaveChanges();
                        updatedUsers.Add(account.UserName);
                    }
                }

                _userRoleService.UpsertRole(user.UserId, account.RoleName, false);
            }

            _context.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        _cacheService.Remove(CacheParam.UsersAll);

        return new RbacDemoSeedResult
        {
            CreatedUsers = createdUsers,
            UpdatedUsers = updatedUsers,
            Accounts = demoAccounts
                .Select(x => new RbacDemoAccountInfo
                {
                    UserName = x.UserName,
                    Password = x.Password,
                    RoleName = x.RoleName,
                    FullName = x.FullName
                })
                .ToList()
        };
    }

    private static IReadOnlyList<DemoSeedAccount> BuildDefaultDemoAccounts()
    {
        return new[]
        {
            new DemoSeedAccount("demo.superadmin", "Demo Super Admin", "demo.superadmin@vdcd.local", "0900000001", AdminRoles.SuperAdmin, "Demo@123"),
            new DemoSeedAccount("demo.content", "Demo Content Manager", "demo.content@vdcd.local", "0900000002", AdminRoles.ContentManager, "Demo@123"),
            new DemoSeedAccount("demo.hr", "Demo HR Manager", "demo.hr@vdcd.local", "0900000003", AdminRoles.HrManager, "Demo@123"),
            new DemoSeedAccount("demo.viewer", "Demo Viewer", "demo.viewer@vdcd.local", "0900000004", AdminRoles.Viewer, "Demo@123")
        };
    }

    private sealed record DemoSeedAccount(
        string UserName,
        string FullName,
        string Email,
        string Phone,
        string RoleName,
        string Password);
}

public sealed class RbacDemoSeedResult
{
    public List<string> CreatedUsers { get; set; } = new();
    public List<string> UpdatedUsers { get; set; } = new();
    public List<RbacDemoAccountInfo> Accounts { get; set; } = new();
}

public sealed class RbacDemoAccountInfo
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
