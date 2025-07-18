using Core.EntryPoints;
using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.CurrentScope = this;

            MainMenuEntryPoint entryPoint = this.AddComponent<MainMenuEntryPoint>();
            builder.RegisterComponent(entryPoint);
        }
    }
}