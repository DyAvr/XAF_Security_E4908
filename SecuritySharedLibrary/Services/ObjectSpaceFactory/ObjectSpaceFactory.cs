using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.AspNetCore;
using DevExpress.ExpressApp.Services;

namespace SecuritySharedLibrary.Services {
    internal class ObjectSpaceFactory : IDisposable, IObjectSpaceFactory {
        readonly IObjectSpaceProviderCreator objectSpaceProviderCreator;
        readonly IAuthenticatedSecurityProvider securityProvider;
        IObjectSpaceProvider? objectSpaceProvider;

        public ObjectSpaceFactory(IObjectSpaceProviderCreator objectSpaceProviderCreator, IAuthenticatedSecurityProvider securityProvider) {
            this.objectSpaceProviderCreator = objectSpaceProviderCreator;
            this.securityProvider = securityProvider;
        }

        IObjectSpaceProvider GetObjectSpaceProvider(Type entityType) {
            if (objectSpaceProvider == null) {
                objectSpaceProvider = objectSpaceProviderCreator.CreateObjectSpaceProvider(securityProvider.GetSecurity(), entityType);
            }
            return objectSpaceProvider;
        }
        public IObjectSpace CreateObjectSpace(Type entityType) => GetObjectSpaceProvider(entityType).CreateObjectSpace();

        void IDisposable.Dispose() {
            if (objectSpaceProvider is IDisposable disposable) {
                disposable.Dispose();
            }

            objectSpaceProvider = null;
        }
    }
}
