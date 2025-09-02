using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Common.Descriptors;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using Game.Tools.Descriptors;
using Game.Tools.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("WaterSoil")]
    public class WaterSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly ToolsService _toolsService = null!;
        [Inject]
        private readonly NotificationManager _notificationManager = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        [Inject]
        private readonly UIService _uiService = null!;

        private SliderSelectorComponent _sliderSelector = null!;
        private string _itemId = null!;
        private Parameters _parameters;
        private bool _isWaiting;

        public async UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            PieMenuItemSelectionModel pieMenuItemSelectionModel = itemModel.SelectionModels[itemModel.CurrentSelectionIndex];
            if (string.IsNullOrEmpty(pieMenuItemSelectionModel.ItemId)) {
                await _notificationManager.ShowNotification(NotificationType.WATER_TOOL_NOT_FOUND);
                return;
            }

            _parameters = parameters;
            _itemId = pieMenuItemSelectionModel.ItemId;

            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            ToolsDescriptorModel? toolsDescriptorModel = toolsDescriptor.ToolsDescriptors.Find(tool => tool.ToolId == _itemId);
            if (toolsDescriptorModel == null) {
                throw new KeyNotFoundException($"Tools descriptor not found. ToolId={_itemId}");
            }

            if (string.IsNullOrEmpty(toolsDescriptorModel.SelectorDescriptorId)) {
                return;
            }
            
            SelectorsDescriptor selectorDescriptor = _descriptorService.Require<SelectorsDescriptor>();
            SelectorDescriptorModel selectorDescriptorModel = selectorDescriptor.Require(toolsDescriptorModel.SelectorDescriptorId);

            _sliderSelector = await _uiService.ShowElementAsync<SliderSelectorComponent>();
            _sliderSelector.Initialize(new(selectorDescriptorModel.MinValue, selectorDescriptorModel.MaxValue, selectorDescriptorModel.StartValue,
                                           selectorDescriptorModel.StepsCount, selectorDescriptorModel.RoundDigits));
            
            _sliderSelector.OnAcceptPressed += OnAccepted;
            _sliderSelector.OnBackPressed += OnBackPressed;
            _isWaiting = true;
            await UniTask.WaitWhile(() => _isWaiting);
        }

        private void OnAccepted(float value)
        {
            _toolsService.UseToolAsync(_itemId, _parameters.AddParam(ParameterNames.WaterAmount, value)).Forget();
            RemoveSelector();
        }

        private void OnBackPressed()
        {
            RemoveSelector();
        }

        private void RemoveSelector()
        {
            _isWaiting = false;
            _sliderSelector.OnAcceptPressed -= OnAccepted;
            _sliderSelector.OnBackPressed -= OnBackPressed;
            _uiService.HideDialogAsync<SliderSelectorComponent>().Forget();
        }
    }
}