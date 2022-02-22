using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Internal;
using DevExpress.ExpressApp.Services;
using System.Security.Claims;

namespace SecuritySharedLibrary.Services {
    public class SecurityStandardAuthenticationService {

        readonly ISecurityUserProvider securityUserProvider;
        readonly ISecurityStrategyBase security;
        readonly INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory;
        readonly IXafIdentityCreator xafIdentityCreator;

        public SecurityStandardAuthenticationService(ISecurityStrategyBase security, INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory, ISecurityUserProvider securityUserProvider, IXafIdentityCreator xafIdentityCreator) {
            this.security = security;
            this.nonSecuredObjectSpaceFactory = nonSecuredObjectSpaceFactory;
            this.securityUserProvider = securityUserProvider;
            this.xafIdentityCreator = xafIdentityCreator;
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
                        return new ClaimsPrincipal(xafIdentityCreator.CreateXafIdentity(userKey, xafUser.UserName));
                    }
                } catch {
                    //XafSecurity authentication failed
                }
                return null;

            }
        }
    }
}
