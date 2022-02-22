using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Internal;
using DevExpress.ExpressApp.Services;
using System.Security.Claims;

namespace SecuritySharedLibrary.Services {

    public interface IPrincipalCreator {
        ClaimsPrincipal CreatePrincipal(string userKey, string userName);
    }
    internal class XafSecurityDefaultPrincipalCreator : IPrincipalCreator {
        public ClaimsPrincipal CreatePrincipal(string userKey, string userName) {
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, userKey),
                new Claim(ClaimTypes.Name, userName),
                new Claim(SecurityDefaults.AuthenticationPassed, SecurityDefaults.AuthenticationPassed)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, SecurityDefaults.PasswordAuthentication, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            ClaimsPrincipal principal = new ClaimsPrincipal(id);
            return principal;
        }
    }

    public class SecurityStandardAuthenticationService {

        readonly ISecurityUserProvider securityUserProvider;
        readonly ISecurityStrategyBase security;
        readonly INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory;
        readonly IPrincipalCreator principalCreator;

        public SecurityStandardAuthenticationService(ISecurityStrategyBase security, INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory, ISecurityUserProvider securityUserProvider, IPrincipalCreator principalCreator) {
            this.security = security;
            this.nonSecuredObjectSpaceFactory = nonSecuredObjectSpaceFactory;
            this.securityUserProvider = securityUserProvider;
            this.principalCreator = principalCreator;
        }
        public ClaimsPrincipal? Authenticate(string userName, string password) {
            return Authenticate(new AuthenticationStandardLogonParameters(userName, password));
        }
        public ClaimsPrincipal? Authenticate(IAuthenticationStandardLogonParameters logonParameters) {
            return AuthenticateCore(logonParameters);
        }

        private ClaimsPrincipal? AuthenticateCore(IAuthenticationStandardLogonParameters logonParameters) {
            if (logonParameters == null) {
                throw new ArgumentNullException(nameof(logonParameters));
            }
            using (IObjectSpace loginObjectSpace = nonSecuredObjectSpaceFactory.CreateNonSecuredObjectSpace(security.UserType)) {
                security.Logoff();
                try {
                    var xafUser = (ISecurityUser)securityUserProvider.Authenticate(loginObjectSpace, logonParameters);
                    if (xafUser != null) {
                        string userKey = loginObjectSpace.GetKeyValueAsString(xafUser);
                        return principalCreator.CreatePrincipal(userKey, xafUser.UserName);
                    }
                } catch {
                    //XafSecurity authentication failed
                }
                return null;

            }
        }
    }
}
