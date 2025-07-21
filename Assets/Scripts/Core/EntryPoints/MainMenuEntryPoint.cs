using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.UI.Dialogs.MainMenu;
using UnityEngine;
using VContainer;

namespace Core.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [Inject]
        private UIService _uiService = null!;

        public void Start()
        {
            _uiService.ShowDialogAsync<MainMenuDialog>().Forget();
        }
    }
}