using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Interactable.PieMenu.UI;
using JetBrains.Annotations;

namespace Game.Interactable.PieMenu.Service
{
    [UsedImplicitly]
    public class PieMenuService
    {
        private readonly UIService _uiService;

        public PieMenuService(UIService uiService)
        {
            _uiService = uiService;
        }

        public async UniTask<PieMenuController> CreatePieMenuAsync()
        {
            PieMenuController pieMenu = await _uiService.ShowDialogAsync<PieMenuController>();
            pieMenu.transform.parent.gameObject.SetActive(true);
            pieMenu.gameObject.SetActive(true);
            pieMenu.Initialize();
            return pieMenu;
        }
    }
}