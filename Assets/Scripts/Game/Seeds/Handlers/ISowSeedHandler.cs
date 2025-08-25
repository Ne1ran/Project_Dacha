using Core.Parameters;
using Cysharp.Threading.Tasks;

namespace Game.Seeds.Handlers
{
    public interface ISowSeedHandler
    {
        UniTask SowSeedAsync(string seedId, Parameters parameters);
    }
}