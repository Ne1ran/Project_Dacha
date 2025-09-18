using Core.Attributes;
using Core.Console.Controller;
using Cysharp.Threading.Tasks;

namespace Core.Console.Service
{
    [Service]
    public class ConsoleService
    {
        public UniTask InitializeAsync()
        {
            UnityEngine.Resources.Load<ConsoleController>("pfDebugConsole");
            return UniTask.CompletedTask;
        }
    }
}