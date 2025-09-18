using Core.Console.Service;
using Core.Descriptors.Service;
using Core.EntryPoints;
using Core.Notifications.Service;
using Core.Resources.Service;
using Core.SceneManagement.Service;
using Core.Serialization;
using Core.UI.Service;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class ApplicationScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.ApplicationScope = this;
            AppContext.CurrentScope = this;
            
            builder.Register<AddressablesManager>(Lifetime.Singleton);
            builder.Register<PrefabBinderManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();;
            builder.Register<IResourceService, AddressablesResourceService>(Lifetime.Singleton);
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.Register<ConsoleService>(Lifetime.Singleton);
            builder.Register<IDescriptorService, ResourcesDescriptorsService>(Lifetime.Singleton);
            builder.RegisterMessagePipe();
            
            builder.Register<UIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ISerializer, JsonSerializer>(Lifetime.Singleton);
            builder.Register<NotificationManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<ApplicationEntryPoint>();
        }
    } 
}