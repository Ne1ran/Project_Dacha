using Core.Parameters;
using Cysharp.Threading.Tasks;

namespace Game.PieMenu.ActionHandler
{
    public interface IPieMenuActionHandler
    {
        void Initialize(Parameters parameters);
        
        UniTask ActionAsync();
    }
}