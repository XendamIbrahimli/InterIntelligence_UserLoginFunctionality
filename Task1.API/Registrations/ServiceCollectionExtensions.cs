
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Task1.BL.Helpers;
using Task1.BL.Services;
using Task1.Core.Services;

namespace Task1.API.Registrations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddHttpContextAccessor();

            services.Configure<JwtOptions>(config.GetSection(JwtOptions.Jwt));

            JwtOptions jwtOptions = new JwtOptions();
            jwtOptions.Audience = config.GetSection("JwtOptions")["Audience"]!;
            jwtOptions.Issuer = config.GetSection("JwtOptions")["Issuer"]!;
            jwtOptions.Secret = config.GetSection("JwtOptions")["Secret"]!;

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = secKey,
                    ClockSkew = TimeSpan.Zero
                };
            });
            return services;
        }
    }
}
