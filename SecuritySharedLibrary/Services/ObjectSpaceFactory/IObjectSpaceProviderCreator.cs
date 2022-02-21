using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace SecuritySharedLibrary.Services {
    public interface IObjectSpaceProviderCreator {
        IObjectSpaceProvider CreateObjectSpaceProvider(ISecurityStrategyBase security, Type entityType);
    }
}
