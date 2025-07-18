using System.Threading;
using Core.Scene;
using Core.Scene.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.EntryPoints
{
    public class GameApplicationEntryPoint : MonoBehaviour
    {
        [Inject]
        private SceneService _sceneService = null!;
        
        private void Awake()
        {
            Debug.Log("Starting Game Application");
            DontDestroyOnLoad(this);
            InitializeAsync(destroyCancellationToken).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken token)
        {
            InitializeAddressables();
            InitializeLocalization();
            await UniTask.Yield();
            await UniTask.Delay(5000, cancellationToken: token);
            InitializeMainMenu();
        }

        private void InitializeAddressables()
        {
            Debug.Log("Need to implement addressables");
        }

        private void InitializeMainMenu()
        {
            _sceneService.LoadSceneAsync(SceneUtils.MAIN_MENU_SCENE).Forget();
        }

        private void InitializeLocalization()
        {
            Debug.Log("Need to implement localization");
        }
    }
}