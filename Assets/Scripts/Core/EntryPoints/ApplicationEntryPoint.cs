using System.Threading;
using Core.Console.Service;
using Core.Descriptors.Service;
using Core.Resources.Service;
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
        [Inject]
        private ConsoleService _consoleService = null!;
        [Inject]
        private AddressablesManager _addressablesManager = null!;
        [Inject]
        private IDescriptorService _descriptorService = null!;

        private void Awake()
        {
            Debug.Log("Starting Game Application");
            DontDestroyOnLoad(this);
            InitializeAsync(destroyCancellationToken).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken token)
        {
            // todo neiran implement runner?
            await InitializeConsole();
            await InitializeAddressables();
            await InitializeDescriptors();
            InitializeLocalization();
            await UniTask.Yield(token);
            InitializeMainMenu();
        }

        private async UniTask InitializeConsole()
        {
            await _consoleService.InitializeAsync();
        }

        private async UniTask InitializeAddressables()
        {
            await _addressablesManager.InitializeAsync();
        }

        private async UniTask InitializeDescriptors()
        {
            await _descriptorService.InitializeAsync();
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