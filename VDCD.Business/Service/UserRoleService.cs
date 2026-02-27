using VDCD.DataAccess;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;

namespace VDCD.Business.Service;

public class UserRoleService
{
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly AppDbContext _context;

    public UserRoleService(IRepository<UserRole> userRoleRepo, AppDbContext context)
    {
        _userRoleRepo = userRoleRepo;
        _context = context;
    }

    public string GetRoleNameByUserId(int userId)
    {
        var role = _userRoleRepo.GetReadOnly(x => x.UserId == userId);
        return NormalizeRole(role?.RoleName);
    }

    public Dictionary<int, string> GetRoleMapByUserId()
    {
        return _userRoleRepo
            .GetsReadOnly()
            .GroupBy(x => x.UserId)
            .ToDictionary(
                x => x.Key,
                x => NormalizeRole(x.OrderByDescending(r => r.Id).First().RoleName));
    }

    public void UpsertRole(int userId, string? roleName, bool autoCommit = true)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId is invalid.", nameof(userId));

        var normalizedRole = NormalizeRole(roleName);
        var currentRole = _userRoleRepo.Get(false, x => x.UserId == userId);

        if (currentRole == null)
        {
            _userRoleRepo.Create(new UserRole
            {
                UserId = userId,
                RoleName = normalizedRole,
                CreateAt = DateTime.Now
            });
        }
        else
        {
            currentRole.RoleName = normalizedRole;
            currentRole.UpdateAt = DateTime.Now;
            _userRoleRepo.Update(currentRole);
        }

        if (autoCommit)
            _context.SaveChanges();
    }

    public void DeleteByUserId(int userId, bool autoCommit = true)
    {
        var roles = _userRoleRepo.Gets(false, x => x.UserId == userId).ToList();
        if (roles.Count == 0)
            return;

        _userRoleRepo.DeleteRange(roles);

        if (autoCommit)
            _context.SaveChanges();
    }

    public string NormalizeRole(string? roleName)
    {
        return AdminRoles.Normalize(roleName);
    }
}
