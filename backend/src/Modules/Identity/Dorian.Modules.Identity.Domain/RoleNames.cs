namespace Dorian.Modules.Identity.Domain.Constants;

public static class RoleNames
{
    public const string Customer = "Customer";
    public const string Trainer = "Trainer";
    public const string Reception = "Reception";
    public const string BranchAdmin = "BranchAdmin";
    public const string SuperAdmin = "SuperAdmin";

    public static readonly string[] All =
    [
        Customer,
        Trainer,
        Reception,
        BranchAdmin,
        SuperAdmin
    ];
}
