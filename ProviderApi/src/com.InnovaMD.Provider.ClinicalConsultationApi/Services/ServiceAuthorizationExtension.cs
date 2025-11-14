using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Provider.PortalApi.Security;
using com.InnovaMD.Utilities.Provider.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceAuthorizationExtension
    {
        public static void ConfigureServiceAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var scope = configuration.GetValue<string>("OAuthOptions:Scope");

            services.AddAuthorization(auth =>
            {

                // Example of a policy validating that an user is authenticated and has a role assigned
                auth.AddPolicy(nameof(Policies.BearerAuthenticated), new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimsIdentity.DefaultRoleClaimType)
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireClaim("Scope", new string[] { scope })
                    .RequireAuthenticatedUser()
                    .Build());

                // Example of a policy validating that an user has access to specific application features
                AddPolicy(auth, Policies.ViewClinicalConsultationHistory, scope, Features.ViewClinicalConsultationhistory);
                AddPolicies(auth, Policies.AllowPrintOrDownloadClinicalConsultation, scope, new[] { (int)Features.PrintClinicalConsultationHistory, (int)Features.DownloadClinicalConsultationHistory }.ToList());
                AddPolicy(auth, Policies.CreateClinicalConsultation, scope, Features.CreateClinicalConsultation);
                AddPolicies(auth, Policies.BearerClinicalConsultation, scope, new[] { (int)Features.CreateClinicalConsultation, (int)Features.ViewClinicalConsultationhistory }.ToList());

            });
        }
        private static void AddPolicy(
            AuthorizationOptions auth
            , Policies policy
            , string scope
            , Features? feature = null
            , ApplicationDomainContexts? context = null
            , ApplicationDomainSubContexts? subContext = null
            , bool? isDelegate = null)
        {
            AddPolicy(auth, policy.ToString(), scope, feature, context, subContext, isDelegate);
        }

        private static void AddPolicy(
            AuthorizationOptions auth
            , string policyName
            , string scope
            , Features? feature = null
            , ApplicationDomainContexts? context = null
            , ApplicationDomainSubContexts? subContext = null
            , bool? isDelegate = null)
        {
            var policyBuilder = new AuthorizationPolicyBuilder()
                .RequireClaim(ClaimsIdentity.DefaultRoleClaimType);

            var requirements = new List<IAuthorizationRequirement>();

            if (feature.HasValue)
            {
                int? contextValue = context.HasValue ? (int?)context : null;
                int? subContextValue = subContext.HasValue ? (int?)subContext : null;
                requirements.Add(new FeatureRequirement(new List<int> { (int)feature.Value }, contextValue, subContextValue));
            }

            if (isDelegate.HasValue)
            {
                requirements.Add(new IsDelegateRequirement(isDelegate.Value));
            }

            if (requirements.Any())
            {
                policyBuilder.AddRequirements(requirements.ToArray());
            }

            var policy = policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim("Scope", scope)
                .RequireAuthenticatedUser()
                .Build();

            auth.AddPolicy(policyName, policy);
        }


        private static void AddPolicies(AuthorizationOptions auth, Policies policyName, string scope, List<int> features = null)
        {
            var policyBuilder = new AuthorizationPolicyBuilder()
                .RequireClaim(ClaimsIdentity.DefaultRoleClaimType);

            if (features != null)
            {
                policyBuilder.AddRequirements(new FeatureRequirement(features));
            }

            var policy = policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim("Scope", new string[] { scope })
                .RequireAuthenticatedUser()
                .Build();

            auth.AddPolicy(policyName.ToString(), policy);
        }

    }
}
