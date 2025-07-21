using Game.Common.Controller;

namespace Game.Tools.Component
{
    public class ToolController : PickableComponent
    {
        public string GetName => gameObject.name;
    }
}