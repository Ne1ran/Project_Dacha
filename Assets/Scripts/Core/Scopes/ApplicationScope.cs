using Core.Resources.Service;
using Core.Scene;
using Core.Scene.Service;
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
            builder.Register<IResourceService, ResourceService>(Lifetime.Singleton);
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<GameApplication>();
            builder.RegisterMessagePipe();
            builder.Register<UIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}