using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.AspNetCore;
using DevExpress.ExpressApp.Services;

namespace SecutirySharedLibrary.Services {
    public abstract class ObjectSpaceFactoryBase : IDisposable, IObjectSpaceFactory {
        readonly ISecurityStrategyBase security;
        readonly IXafSecurityAuthenticationService xafSecurityAuthenticationService;
        IObjectSpaceProvider? objectSpaceProvider;
        IObjectSpaceProvider? nonSecuredObjectSpaceProvider;

        public ObjectSpaceFactoryBase(ISecurityStrategyBase security, IXafSecurityAuthenticationService xafSecurityLogin) {
            this.security = (SecurityStrategyComplex)security;
            this.xafSecurityAuthenticationService = xafSecurityLogin;
        }
        protected abstract IObjectSpaceProvider CreateObjectSpaceProvider(ISecurityStrategyBase security);

        IObjectSpaceProvider GetObjectSpaceProvider(Type entityType) {
            if (objectSpaceProvider == null) {
                xafSecurityAuthenticationService.EnsureLogon(this);
                objectSpaceProvider = CreateObjectSpaceProvider(security);
            }
            return objectSpaceProvider;
        }

        INonsecuredObjectSpaceProvider GetNonSecuredObjectSpaceProvider(Type entityType) {
            if (nonSecuredObjectSpaceProvider is null)
                nonSecuredObjectSpaceProvider = CreateObjectSpaceProvider(security);
            return (INonsecuredObjectSpaceProvider)nonSecuredObjectSpaceProvider;
        }
        public IObjectSpace CreateObjectSpace(Type entityType) => GetObjectSpaceProvider(entityType).CreateObjectSpace();

        public IObjectSpace CreateNonSecuredObjectSpace(Type entityType) => GetNonSecuredObjectSpaceProvider(entityType).CreateNonsecuredObjectSpace();

        public IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema) => ((IObjectSpaceProvider)GetNonSecuredObjectSpaceProvider(security.UserType)).CreateUpdatingObjectSpace(allowUpdateSchema);

        void IDisposable.Dispose() {
            if (objectSpaceProvider is IDisposable disposable) {
                disposable.Dispose();
            }

            objectSpaceProvider = null;

            if (nonSecuredObjectSpaceProvider is IDisposable disposable1) {
                disposable1.Dispose();
            }

            nonSecuredObjectSpaceProvider = null;
        }
    }
}
