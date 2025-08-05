using System.Collections.Generic;
using System.Linq;
using Core.Descriptors.Service;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Event;
using Game.Inventory.Service;
using Game.PlayMode.UI.Component;
using Game.PlayMode.UI.Model;
using Game.PlayMode.UI.ViewModel;
using Game.Utils;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.PlayMode.UI.Screen
{
    [PrefabPath("UI/Dialogs/PlayMode/PlayModeScreen")]
    public class PlayModeScreen : MonoBehaviour
    {
        [ComponentBinding("Crosshair")]
        private CrosshairController _crosshairController;
        [ComponentBinding("HotkeyPanel")]
        private HorizontalLayoutGroup _hotkeyPanel;

        [Inject]
        private IResourceService _resourceService;
        [Inject]
        private IDescriptorService _descriptorService;
        [Inject]
        private InventoryService _inventoryService;
        [Inject]
        private ISubscriber<string, HotkeyChangedEvent> _hotkeyChangedSubscriber;
        
        [CanBeNull]
        private PlayModeScreenViewModel _viewModel;
        
        private List<HotkeySlotView> _hotkeySlotViews = new(Constants.Constants.HOT_KEY_SLOTS);

        
        public async UniTask InitializeAsync()
        {
            FadeCrosshair(true).Forget();
            _viewModel = new(_inventoryService, _descriptorService, _hotkeyChangedSubscriber);
            _viewModel.OnHotkeyChanged += OnHotkeyChanged;
            await CreateHotkeysAsync();
        }

        private void OnDestroy()
        {
            if (_viewModel != null) {
                _viewModel.OnHotkeyChanged -= OnHotkeyChanged;
            }

            _viewModel?.Dispose();
            _viewModel = null;
        }

        private async UniTask CreateHotkeysAsync()
        {
            foreach (HotkeySlotView hotkeySlotView in _hotkeySlotViews) {
                hotkeySlotView.DestroyObject();
            }
            
            _hotkeySlotViews.Clear();
            
            List<HotkeySlotViewModel> slots = _viewModel!.GetCurrentSlotsViewModels();
            List<UniTask<HotkeySlotView>> loadTasks = new(slots.Count);
            for (int i = 0; i < slots.Count; i++) {
                loadTasks.Add(_resourceService.LoadObjectAsync<HotkeySlotView>());
            }

            Transform panelTransform = _hotkeyPanel.transform;
            _hotkeySlotViews = (await UniTask.WhenAll(loadTasks)).ToList();
            for (int i = 0; i < slots.Count; i++) {
                HotkeySlotViewModel slotViewModel = slots[i];
                HotkeySlotView view = _hotkeySlotViews[i];
                view.Initialize(slotViewModel);
                view.transform.SetParent(panelTransform, false);
            }
        }

        private void OnHotkeyChanged(int hotkey)
        {
            CreateHotkeysAsync().Forget(); // todo neiran remove after tests, temp workaround 
        }
        
        public UniTask ShowCrosshair(bool instantly = false)
        {
            return _crosshairController.Show(true);
        }
        
        public UniTask FadeCrosshair(bool instantly = false)
        {
            return _crosshairController.Fade(true);
        }
        
        public void SetColor(Color color)
        {
            _crosshairController.ChangeColor(color);
        }
    }
}