using Core.Resources.Binding.Attributes;
using Core.Scene;
using Core.Scene.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Dialogs.MainMenu
{
    [PrefabPath("UI/Dialogs/MainMenuDialog")]
    public class MainMenuDialog : MonoBehaviour
    {
        [ComponentBinding("PlayButton")]
        private Button _playButton = null!;
        [ComponentBinding("SettingsButton")]
        private Button _settingsButton = null!;
        [ComponentBinding("ExitButton")]
        private Button _exitButton = null!;
        
        [Inject]
        private SceneService _sceneService = null!;

        private void Start()
        {
            _playButton.onClick.AddListener(OnStartClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(OnStartClicked);
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnStartClicked()
        {
            StartGame().Forget();

            async UniTask StartGame()
            {
                _playButton.interactable = false;
                await _sceneService.LoadSceneAsync(SceneUtils.COUNTRY_HOUSE_SCENE);
                _playButton.interactable = true;
            }
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings");
        }

        private void OnExitClicked()
        {
            Debug.Log("Exit");
        }
    }
}