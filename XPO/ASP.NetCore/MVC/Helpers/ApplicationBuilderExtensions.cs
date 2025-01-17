﻿using DevExpress.ExpressApp.Xpo;
using DatabaseUpdater;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ApplicationBuilderExtensions {
        public static IApplicationBuilder UseDemoData(this IApplicationBuilder app, string connectionString) {
            using(var objectSpaceProvider = new XPObjectSpaceProvider(connectionString)) {
                SecurityProvider.RegisterEntities(objectSpaceProvider);
                using(var objectSpace = objectSpaceProvider.CreateUpdatingObjectSpace(true)) {
                    new Updater(objectSpace).UpdateDatabase();
                }
            }
            return app;
        }
    }
}
