using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Attributes;
using Core.Conditions.Checker;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Core.Parameters;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Interactable.Descriptor;
using Game.Interactable.Model;
using Game.PieMenu.Model;
using Game.PieMenu.PrepareHandlers;
using Game.PieMenu.UI;
using UnityEngine;

namespace Game.PieMenu.Service
{
    [Service]
    public class PieMenuService
    {
        private readonly UIService _uiService;
        private readonly IDescriptorService _descriptorService;
        private readonly PieMenuPrepareFactory _pieMenuPrepareFactory;
        private readonly ConditionService _conditionService;

        private PieMenuController? _currentPieMenu;

        public PieMenuService(UIService uiService,
                              IDescriptorService descriptorService,
                              PieMenuPrepareFactory pieMenuPrepareFactory,
                              ConditionService conditionService)
        {
            _uiService = uiService;
            _descriptorService = descriptorService;
            _pieMenuPrepareFactory = pieMenuPrepareFactory;
            _conditionService = conditionService;
        }

        public async UniTask<PieMenuController> CreatePieMenuAsync(InteractableType interactableType, Parameters parameters)
        {
            List<PieMenuItemModel> pieMenuItemModels = await CreateItemModelsAsync(interactableType, parameters);
            PieMenuController pieMenu = await _uiService.ShowDialogAsync<PieMenuController>();
            pieMenu.gameObject.SetActive(true);
            pieMenu.Initialize(parameters);
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
            List<PieMenuItemModel> pieMenuItemModels = await CreateItemModelsAsync(interactableType);
            await _currentPieMenu.AddItemsAsync(pieMenuItemModels);
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

        private async UniTask<List<PieMenuItemModel>> CreateItemModelsAsync(InteractableType interactableType,
                                                                            Parameters parameters = default,
                                                                            CancellationToken token = default)
        {
            List<UniTask<PieMenuItemModel>> items = new();

            InteractionDescriptor interactionDescriptor = _descriptorService.Require<InteractionDescriptor>();
            InteractionDescriptorModel interactionDescriptorModel = interactionDescriptor.RequireByType(interactableType);

            // todo neiran also add checker or descriptor to check if item persists or so! For now show everything, just check for item before use!

            foreach (InteractionPieMenuSettings pieMenuSettings in interactionDescriptorModel.Settings) {
                ConditionResult conditionResult = _conditionService.Check(pieMenuSettings.Conditions, parameters);
                if (conditionResult.IsAllowed) {
                    items.Add(CreateItemModelAsync(pieMenuSettings, token));
                }
            }

            PieMenuItemModel[] itemModels = await UniTask.WhenAll(items);
            return itemModels.ToList();
        }

        private UniTask<PieMenuItemModel> CreateItemModelAsync(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            IPieMenuPrepareHandler pieMenuPrepareHandler = _pieMenuPrepareFactory.Create(pieMenuSettings.PrepareHandlerName);
            return pieMenuPrepareHandler.PrepareAsync(pieMenuSettings, token);
        }
    }
}