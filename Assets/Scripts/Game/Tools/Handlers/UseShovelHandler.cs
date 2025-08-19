using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Tools.Handlers
{
    [Handler("UseShovel")]
    public class UseShovelHandler : IUseToolHandler
    {
        public UniTask UseAsync()
        {
            Debug.LogWarning("Using Shovel");
            return UniTask.CompletedTask;
        }
    }
}