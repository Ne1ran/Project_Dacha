using VContainer.Unity;

namespace Core.Scopes
{
    public static class AppContext
    {
        public static LifetimeScope ApplicationScope { get; set; }
        public static LifetimeScope CurrentScope { get; set; }
    }
}