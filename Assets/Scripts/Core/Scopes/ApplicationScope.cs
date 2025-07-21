using Core.EntryPoints;
using Core.Resources.Service;
using Core.SceneManagement.Service;
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
            
            builder.Register<IResourceService, ResourceService>(Lifetime.Singleton);
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.RegisterMessagePipe();
            builder.Register<UIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<ApplicationEntryPoint>();
        }
    } 
}