using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using SecuritySharedLibrary.Services;

namespace Blazor.ServerSide.Services {
    public class ObjectSpaceProviderCreator : IObjectSpaceProviderCreator {
        readonly IXpoDataStoreProvider xpoDataStoreProvider;
        readonly ITypesInfo typesInfo;
        public ObjectSpaceProviderCreator(IXpoDataStoreProvider xpoDataStoreProvider, ITypesInfo typesInfo) {
            this.typesInfo = typesInfo;
            this.xpoDataStoreProvider = xpoDataStoreProvider;
        }

        IObjectSpaceProvider IObjectSpaceProviderCreator.CreateObjectSpaceProvider(ISecurityStrategyBase security, Type entityType) => new SecuredObjectSpaceProvider((ISelectDataSecurityProvider)security, xpoDataStoreProvider, typesInfo, null, true);
    }
}
