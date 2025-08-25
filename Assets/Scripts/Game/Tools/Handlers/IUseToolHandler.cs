using Core.Parameters;
using Cysharp.Threading.Tasks;

namespace Game.Tools.Handlers
{
    public interface IUseToolHandler
    {
        UniTask UseAsync(Parameters parameters);
    }
}