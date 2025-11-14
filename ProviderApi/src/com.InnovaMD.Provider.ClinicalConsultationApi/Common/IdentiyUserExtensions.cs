using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Provider.Security;
using Microsoft.Azure.Amqp.Framing;
using System;
using System.Reflection;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace com.InnovaMD.Provider.PortalApi.Common
{

    public static class IdentiyUserExtensions
    {
        public static IdentityUser GetIdentityUserBasicInformation(this ClaimsPrincipal principal)
        {
            var user = new IdentityUser()
            {
                UserId = principal.GetUserIdInt(),
                IsDelegate = principal.IsDelegate() ?? false,
                ApplicationDomainId = principal.GetApplicationDomainIdInt(),
                ActiveRole = principal.GetActiveUserRole()
            };

            return user;
        }

        public static IdentityUser GetIdentityUserForProvider(this ClaimsPrincipal principal)
        {
            var user = principal.GetIdentityUserBasicInformation();
            user.ProviderAffiliations = principal.GetProviderAffiliationIds();
            user.ProviderId = principal.GetProviderIdInt();

            return user;
        }

        public static IdentityUser GetIdentityUserForAdministrationGroup(this ClaimsPrincipal principal)
        {
            var user = principal.GetIdentityUserBasicInformation();
            user.AdministrationGroup = principal.GetAdministrationGroup();

            return user;
        }

        public static IdentityUser GetIdentityUserForPayer(this ClaimsPrincipal principal)
        {
            var user = principal.GetIdentityUserBasicInformation();
            user.HealthPlan = principal.GetHealthPlan();

            return user;
        }

        public static IdentityUser GetIdentityUser(this ClaimsPrincipal principal)
        {
            var user = new IdentityUser()
            {
                UserId = principal.GetUserIdInt(),
                Username = principal.GetUsername(),
                IsDelegate = principal.IsDelegate() ?? false,
                ApplicationDomainId = principal.GetApplicationDomainIdInt(),
                ActiveRole = principal.GetActiveUserRole(),
                ProviderAffiliations = principal.GetProviderAffiliationIds(),
                ProviderId = principal.GetProviderIdInt(),
                AdministrationGroup = principal.GetAdministrationGroup(),
                HealthPlan = principal.GetHealthPlan(),
                Expiration = principal.GetExpiration(),
                SessionId = principal.FindFirstValue("session_id")
            };

            return user;
        }

        public static IdentityUserExtended GetIdentityUserExtended(this ClaimsPrincipal principal)
        {

            var user = principal.GetIdentityUser();
            var userExtended = new IdentityUserExtended();

            var baseProps = typeof(IdentityUser).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in baseProps)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var valor = prop.GetValue(user);
                    prop.SetValue(userExtended, valor);
                }
            }

            userExtended.Name = principal.FindFirstValue("given_name");

            return userExtended;
        }

        public static DateTime GetExpiration(this ClaimsPrincipal principal)
        {
            try
            {
                var exp = long.Parse(principal.FindFirstValue("exp"));
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(exp);
                return dateTimeOffset.LocalDateTime;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }

    }
