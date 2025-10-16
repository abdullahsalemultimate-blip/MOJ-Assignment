namespace InventorySys.Domain.Constants;

public static class IdentityConsts
{
    public const bool PasswordRequireDigit = true;
    public const bool PasswordRequireLowercase = true;
    public const bool PasswordRequireNonAlphanumeric = false;
    public const bool PasswordRequireUppercase = true;
    public const int PasswordRequiredLength = 10;
    public const int PasswordRequiredUniqueChars = 1;

    public static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    public const int MaxFailedAccessAttempts = 11;
    public const bool LockoutAllowedForNewUsers = true;

    public const bool UserRequireUniqueEmail = true;

    public const bool SignInRequireConfirmedEmail = false;
}
