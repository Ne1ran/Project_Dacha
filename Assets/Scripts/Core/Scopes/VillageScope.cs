using Core.EntryPoints;
using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class VillageScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.CurrentScope = this;

            VillageEntryPoint entryPoint = this.AddComponent<VillageEntryPoint>();
            builder.RegisterComponent(entryPoint);
        }
    }
}