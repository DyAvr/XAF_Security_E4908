using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Services;
using Microsoft.Extensions.Options;
using SecuritySharedLibrary.Services;

namespace Microsoft.Extensions.DependencyInjection {
    public static class SecuritySharedLibraryExtensions {

        public class XafSecurityObjectsLayerEvents {
            public Action<ITypesInfo, IServiceProvider>? CustomizeTypesInfo { get; set; }
        }

        public class XafSecurityObjectsLayerOptions {

            public XafSecurityObjectsLayerEvents Events { get; } = new XafSecurityObjectsLayerEvents();


        }
        public static IServiceCollection AddXafSecurityObjectsLayer<T>(this IServiceCollection services) where T : class, IObjectSpaceProviderCreator => services.AddXafSecurityObjectsLayer<T>(_ => { });

        public static IServiceCollection AddXafSecurityObjectsLayer<T>(this IServiceCollection services, Action<XafSecurityObjectsLayerOptions> options) where T : class, IObjectSpaceProviderCreator {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            services.Configure(options);

            services.AddSingleton<ITypesInfo>(s => {
                TypesInfo typesInfo = new TypesInfo();

                var o = s.GetRequiredService<IOptions<XafSecurityObjectsLayerOptions>>();
                o.Value.Events.CustomizeTypesInfo?.Invoke(typesInfo, s);
                return typesInfo;
            });

            services.AddScoped<IObjectSpaceProviderCreator, T>();

            services.AddScoped<IObjectSpaceFactory, ObjectSpaceFactory>();
            services.AddScoped<INonSecuredObjectSpaceFactory, NonSecuredObjectSpaceFactory>();
            services.AddScoped(s => (IUpdatingObjectSpaceFactory)s.GetRequiredService<INonSecuredObjectSpaceFactory>());
            return services;
        }
    }
}
