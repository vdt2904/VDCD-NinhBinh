namespace VDCD.Entities.Security;

public static class AdminRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string ContentManager = "ContentManager";
    public const string HrManager = "HrManager";
    public const string Viewer = "Viewer";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SuperAdmin,
        ContentManager,
        HrManager,
        Viewer
    };

    public const string SuperAdminOnly = SuperAdmin;
    public const string ContentAccess = $"{SuperAdmin},{ContentManager}";
    public const string HrAccess = $"{SuperAdmin},{HrManager}";
    public const string FileAccess = $"{SuperAdmin},{ContentManager},{HrManager}";
    public const string DashboardAccess = $"{SuperAdmin},{ContentManager},{HrManager},{Viewer}";

    public static bool IsValid(string? role)
    {
        return !string.IsNullOrWhiteSpace(role) &&
               All.Any(x => string.Equals(x, role, StringComparison.OrdinalIgnoreCase));
    }

    public static string Normalize(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return Viewer;

        var matched = All.FirstOrDefault(x => string.Equals(x, role, StringComparison.OrdinalIgnoreCase));
        return matched ?? Viewer;
    }
}
