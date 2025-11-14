using com.InnovaMD.Provider.Models.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceAuthenticationExtension
    {
        public static void ConfigureServiceAuthentication(this IServiceCollection services)
        {
            services.AddMvcCore().AddJsonOptions(o =>
            {
                _ = o.JsonSerializerOptions;
            });

            var oauthOptions = services.BuildServiceProvider().GetRequiredService<IOptions<OAuthOptions>>().Value;

            //Protection API
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = oauthOptions.Authority;
                    o.RequireHttpsMetadata = oauthOptions.RequireHttpsMetadata;
                    o.Audience = o.Audience = oauthOptions.Audience;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ClockSkew = TimeSpan.FromSeconds(oauthOptions.ClockSkew),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        NameClaimType = "unique_name",
                        RequireSignedTokens = true,
                        RequireExpirationTime = true
                    };
                });
        }
    }
}
