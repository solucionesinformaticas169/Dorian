namespace Dorian.Infrastructure;

using System.Text;
using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Infrastructure.Authentication;
using Dorian.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IAppPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<ITokenHasher, Sha256TokenHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        var connectionString = NormalizePostgresConnectionString(
            configuration.GetConnectionString("Postgres")
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? "Host=localhost;Port=5432;Database=dorian;Username=dorian;Password=dorian_dev_password");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        services.AddScoped<IDorianDbContext>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();
        return services;
    }

    private static string NormalizePostgresConnectionString(string rawConnectionString)
    {
        if (rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            || rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(rawConnectionString);
            var userParts = uri.UserInfo.Split(':', 2);
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = uri.AbsolutePath.Trim('/'),
                Username = userParts.Length > 0 ? Uri.UnescapeDataString(userParts[0]) : string.Empty,
                Password = userParts.Length > 1 ? Uri.UnescapeDataString(userParts[1]) : string.Empty,
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }

        return rawConnectionString;
    }
}

