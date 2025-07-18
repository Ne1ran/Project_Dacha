using Core.EntryPoints;
using Game.Inventory.Repo;
using Game.Inventory.Service;
using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class DachaScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.CurrentScope = this;
            
            DachaEntryPoint entryPoint = this.AddComponent<DachaEntryPoint>();
            builder.RegisterComponent(entryPoint);
            builder.Register<InventoryRepo>(Lifetime.Singleton);
            builder.Register<InventoryService>(Lifetime.Singleton);
        }
    }
}