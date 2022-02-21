using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.AspNetCore;
using DevExpress.ExpressApp.Services;

namespace SecutirySharedLibrary.Services {
    public interface ISecurityProvider {
        SecurityStrategy GetSecurity();
    }

    public class SecurityProvider : ISecurityProvider {
        private ISecurityStrategyBase security;
        private readonly IObjectSpaceFactory objectSpaceFactory;
        private readonly IXafSecurityAuthenticationService xafSecurityAuthenticationService;

        public SecurityProvider(ISecurityStrategyBase security, IObjectSpaceFactory objectSpaceFactory, IXafSecurityAuthenticationService xafSecurityAuthenticationService) {
            this.security = security;
            this.objectSpaceFactory = objectSpaceFactory;
            this.xafSecurityAuthenticationService = xafSecurityAuthenticationService;
        }
        SecurityStrategy ISecurityProvider.GetSecurity() {
            xafSecurityAuthenticationService.EnsureLogon(objectSpaceFactory);
            return (SecurityStrategy)security;
        }
    }
}
