namespace Dorian.Application;

using Dorian.Application.Auth;
using Dorian.Application.Branches;
using Dorian.Application.Memberships;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IMembershipService, MembershipService>();
        return services;
    }
}
