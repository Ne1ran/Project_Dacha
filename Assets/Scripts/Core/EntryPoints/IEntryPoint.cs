using Cysharp.Threading.Tasks;

namespace Core.EntryPoints
{
    public interface IEntryPoint
    {
        UniTask RunAsync();
    }
}