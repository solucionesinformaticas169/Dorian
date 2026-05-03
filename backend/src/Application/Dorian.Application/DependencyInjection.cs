namespace Dorian.Application;

using Dorian.Application.Access;
using Dorian.Application.Auth;
using Dorian.Application.Bookings;
using Dorian.Application.Branches;
using Dorian.Application.Classes;
using Dorian.Application.Customers;
using Dorian.Application.CustomerFitnessProfiles;
using Dorian.Application.Dashboard;
using Dorian.Application.GroupClasses;
using Dorian.Application.Memberships;
using Dorian.Application.Promotions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAccessService, AccessService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IClassSessionService, ClassSessionService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerFitnessProfileService, CustomerFitnessProfileService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IGroupClassCatalogService, GroupClassCatalogService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IPromotionService, PromotionService>();
        return services;
    }
}
