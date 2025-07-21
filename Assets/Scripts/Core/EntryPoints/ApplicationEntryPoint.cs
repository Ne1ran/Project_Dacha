using System.Threading;
using Core.SceneManagement;
using Core.SceneManagement.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.EntryPoints
{
    public class ApplicationEntryPoint : MonoBehaviour
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
        
        // private void Awake()
        // {
        //     LifetimeScope appScope = LifetimeScope.Create(ConfigureAppScope);
        //     appScope.name = "ApplicationScope";
        //     AppContext.ApplicationScope = appScope;
        //     AppContext.CurrentScope = appScope;
        //     DontDestroyOnLoad(appScope);
        //
        //     Debug.Log("Starting Game Application");
        //     
        //     appScope.Container.Resolve<SceneService>().LoadSceneAsync(SceneUtils.MAIN_MENU_SCENE).Forget();
        // }

        // private void ConfigureAppScope(IContainerBuilder builder)
        // {
        //     MessagePipeOptions messagePipeOptions = builder.RegisterMessagePipe(o => { o.EnableCaptureStackTrace = true; });
        //     RegisterEvents(builder, messagePipeOptions);
        //     
        //     builder.Register<IResourceService, ResourceService>(Lifetime.Singleton);
        //     builder.Register<SceneService>(Lifetime.Singleton);
        //     builder.Register<UIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        //     
        //     InitializeAddressables();
        //     InitializeLocalization();
        //     // await UniTask.Yield();
        //     // await UniTask.Delay(5000, cancellationToken: token);
        //     // InitializeMainMenu();
        //     // builder.RegisterComponentOnNewGameObject<ApplicationPauseComponent>(Lifetime.Singleton, "AppPause").DontDestroyOnLoad();
        // }
    }
}