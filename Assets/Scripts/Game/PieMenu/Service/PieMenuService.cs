using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Interactable.Descriptor;
using Game.Interactable.Model;
using Game.PieMenu.Model;
using Game.PieMenu.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.PieMenu.Service
{
    [UsedImplicitly]
    public class PieMenuService
    {
        private readonly UIService _uiService;
        private readonly IDescriptorService _descriptorService;

        private PieMenuController? _currentPieMenu;

        public PieMenuService(UIService uiService, IDescriptorService descriptorService)
        {
            _uiService = uiService;
            _descriptorService = descriptorService;
        }

        public async UniTask<PieMenuController> CreatePieMenuAsync(InteractableType interactableType)
        {
            List<PieMenuItemModel> pieMenuItemModels = CreateItemModels(interactableType);
            PieMenuController pieMenu = await _uiService.ShowDialogAsync<PieMenuController>();
            pieMenu.gameObject.SetActive(true);
            pieMenu.Initialize();
            _currentPieMenu = pieMenu;
            await pieMenu.AddItemsAsync(pieMenuItemModels);
            return pieMenu;
        }

        public void RemoveCurrentItems()
        {
            if (_currentPieMenu == null) {
                return;
            }

            _currentPieMenu.RemoveItems();
        }

        public async UniTask ChangeItemsAsync(InteractableType interactableType)
        {
            if (_currentPieMenu == null) {
                Debug.LogWarning("Can't change items if pie menu is null");
                return;
            }
            
            _currentPieMenu.RemoveItems();
            await _currentPieMenu.AddItemsAsync(CreateItemModels(interactableType));
        }

        public async UniTask RemovePieMenuAsync()
        {
            if (_currentPieMenu == null) {
                Debug.Log("Can't remove pie menu because its null.");
                return;
            }

            await _uiService.HideDialogAsync<PieMenuController>();
            _currentPieMenu = null;
        }

        private List<PieMenuItemModel> CreateItemModels(InteractableType interactableType)
        {
            List<PieMenuItemModel> items = new();

            InteractionDescriptor interactionDescriptor = _descriptorService.Require<InteractionDescriptor>();
            InteractionDescriptorModel interactionDescriptorModel = interactionDescriptor.RequireByType(interactableType);

            // todo neiran also add checker or descriptor to check if item persists or so! For now show everything, just check for item before use!

            foreach (InteractionPieMenuSettings pieMenuSettings in interactionDescriptorModel.Settings) {
                items.Add(new(pieMenuSettings.Title, pieMenuSettings.Description, pieMenuSettings.IconPath));
            }

            return items;
        }
    }
}