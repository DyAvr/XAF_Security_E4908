using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Services;

namespace SecuritySharedLibrary.Services {
    public interface IUpdatingObjectSpaceFactory {
        IObjectSpace CreateUpdatingObjectSpace(Type entityType, bool allowUpdateSchema);
    }

    internal class NonSecuredObjectSpaceFactory : IDisposable, INonSecuredObjectSpaceFactory, IUpdatingObjectSpaceFactory {

        readonly IObjectSpaceProviderCreator objectSpaceProviderCreator;
        readonly ISecurityStrategyBase security;
        IObjectSpaceProvider? nonSecuredObjectSpaceProvider;

        public NonSecuredObjectSpaceFactory(IObjectSpaceProviderCreator objectSpaceProviderCreator, ISecurityStrategyBase security) {
            this.objectSpaceProviderCreator = objectSpaceProviderCreator;
            this.security = security;
        }

        internal INonsecuredObjectSpaceProvider GetNonSecuredObjectSpaceProvider(Type entityType) {
            if (nonSecuredObjectSpaceProvider is null)
                nonSecuredObjectSpaceProvider = objectSpaceProviderCreator.CreateObjectSpaceProvider(security, entityType);
            return (INonsecuredObjectSpaceProvider)nonSecuredObjectSpaceProvider;
        }

        public IObjectSpace CreateNonSecuredObjectSpace(Type entityType) => GetNonSecuredObjectSpaceProvider(entityType).CreateNonsecuredObjectSpace();

        public IObjectSpace CreateUpdatingObjectSpace(Type entityType, bool allowUpdateSchema) => ((IObjectSpaceProvider)GetNonSecuredObjectSpaceProvider(entityType)).CreateUpdatingObjectSpace(allowUpdateSchema);

        void IDisposable.Dispose() {
            if (nonSecuredObjectSpaceProvider is IDisposable disposable1) {
                disposable1.Dispose();
            }

            nonSecuredObjectSpaceProvider = null;
        }
    }
}
