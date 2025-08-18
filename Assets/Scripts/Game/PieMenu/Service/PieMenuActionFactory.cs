using Core.Attributes;
using Game.Common.Handlers;
using Game.PieMenu.ActionHandler;

namespace Game.PieMenu.Service
{
    [Factory]
    public class PieMenuActionFactory : HandlerFactory<IPieMenuActionHandler>
    {
        
    }
}