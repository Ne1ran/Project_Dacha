using Core.EntryPoints;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class RailRoadScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.CurrentScope = this;
            builder.RegisterComponentInHierarchy<RailRoadEntryPoint>();
        }
    }
}