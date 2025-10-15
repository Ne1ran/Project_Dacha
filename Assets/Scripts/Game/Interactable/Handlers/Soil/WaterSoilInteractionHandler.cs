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
        private readonly ToolsDescriptor _toolsDescriptor = null!;
        [Inject]
        private readonly SelectorsDescriptor _selectorsDescriptor = null!;
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
                await _notificationManager.ShowNotification(NotificationType.WaterToolNotFound);
                return;
            }

            _parameters = parameters;
            _itemId = pieMenuItemSelectionModel.ItemId;

            ToolsDescriptorModel toolsDescriptorModel = _toolsDescriptor.Require(_itemId);
            if (string.IsNullOrEmpty(toolsDescriptorModel.SelectorDescriptorId)) {
                return;
            }

            SelectorDescriptorModel selectorDescriptorModel = _selectorsDescriptor.Require(toolsDescriptorModel.SelectorDescriptorId);

            _sliderSelector = await _uiService.ShowElementAsync<SliderSelectorComponent>();
            _sliderSelector.Initialize(new(selectorDescriptorModel.MinValue, selectorDescriptorModel.MaxValue, selectorDescriptorModel.StartValue,
                                           selectorDescriptorModel.StepValue, selectorDescriptorModel.RoundDigits));

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
            _uiService.RemoveElementAsync(_sliderSelector.gameObject).Forget();
        }
    }
}