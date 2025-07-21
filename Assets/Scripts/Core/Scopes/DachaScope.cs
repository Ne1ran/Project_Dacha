using Core.EntryPoints;
using Game.Inventory.Repo;
using Game.Inventory.Service;
using Game.Player.Service;
using Game.PlayMode.Service;
using Game.Tools.Service;
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
            
            builder.Register<InventoryRepo>(Lifetime.Singleton);
            builder.Register<InventoryService>(Lifetime.Singleton);
            builder.Register<PlayerService>(Lifetime.Singleton);
            builder.Register<PlayModeService>(Lifetime.Singleton);
            builder.Register<ToolsService>(Lifetime.Singleton);
            
            DachaEntryPoint entryPoint = this.AddComponent<DachaEntryPoint>();
            builder.RegisterComponent(entryPoint);
        }
    }
}