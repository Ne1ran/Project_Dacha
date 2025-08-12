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
            pieMenu.transform.parent.gameObject.SetActive(true);
            pieMenu.gameObject.SetActive(true);
            await pieMenu.InitializeAsync(pieMenuItemModels);
            _currentPieMenu = pieMenu;
            return pieMenu;
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
            foreach (InteractionPieMenuSettings pieMenuSettings in interactionDescriptorModel.Settings) {
                items.Add(new(pieMenuSettings.Title, pieMenuSettings.Description, pieMenuSettings.IconPath));
            }

            return items;
        }
    }
}