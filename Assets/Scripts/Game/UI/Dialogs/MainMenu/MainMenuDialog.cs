using Core.Resources.Binding.Attributes;
using Core.SceneManagement;
using Core.SceneManagement.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI.Dialogs.MainMenu
{
    [NeedBinding("UI/Dialogs/MainMenu/MainMenuDialog")]
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
            _playButton.interactable = false;
            _sceneService.LoadSceneAsync(SceneUtils.DACHA_SCENE).Forget();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("You want to much for now... Pressed Settings");
        }

        private void OnExitClicked()
        {
            Debug.Log("There is no escape from dacha. Only compost. Pressed Exit");
        }
    }
}