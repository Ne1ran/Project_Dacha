using System.Collections.Generic;
using System.Threading;
using Core.Parameters;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Interactable.Service;
using Game.Interactable.ViewModel;
using Game.PieMenu.InputDevices;
using Game.PieMenu.Model;
using Game.PieMenu.Service;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.PieMenu.Utils;
using Game.Player.Service;
using Game.Utils;
using UnityEngine;
using VContainer;

namespace Game.PieMenu.UI
{
    [NeedBinding("pfPieMenu")]
    public class PieMenuController : MonoBehaviour
    {
        private const string SETTINGS_NAME = "Settings";

        [ComponentBinding]
        private RectTransform _rectTransform = null!;
        [ComponentBinding("ItemsHolder")]
        private Transform _itemsHolder = null!;
        [ComponentBinding("MenuItemTemplate")]
        private PieMenuItemController _menuItemControllerTemplate = null!;

        [ComponentBinding(SETTINGS_NAME)]
        private MenuItemSelector _pieMenuItemSelector = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuGeneralSettings _generalSettings = null!;

        [Inject]
        private readonly IResourceService _resourceService = null!;
        [Inject]
        private readonly PieMenuService _pieMenuService = null!;
        [Inject]
        private readonly PlayerService _playerService = null!;
        [Inject]
        private readonly PieMenuInteractionService _pieMenuInteractionService = null!;

        public PieMenuViewModel ViewModel { get; private set; } = new();
        public PieMenuModel PieMenuModel { get; private set; } = new();
        public PieMenuSettingsModel PieMenuSettingsModel { get; private set; } = null!;

        private readonly InputDeviceGetter _inputDeviceGetter = new();

        private Parameters _parameters;
        private bool _closing = false;

        public void Initialize(Parameters parameters)
        {
            _parameters = parameters;

            _playerService.Player.ChangeLookActive(false);
            _playerService.Player.ChangeMovementActive(false);
            _menuItemControllerTemplate.transform.SetActive(false);

            PieMenuSettingsModel = new(PieMenuModel, _pieMenuItemSelector, _inputDeviceGetter, _generalSettings, ViewModel);

            PieMenuSettingsModel = PieMenuSettingsModel;
            PieMenuModel = PieMenuSettingsModel.PieMenuModel;
            InitializePieMenu();

            ViewModel.Initialize(_resourceService, this);
            ViewModel.OnClickedTrigger.Triggered += OnItemClicked;
        }

        private void OnDestroy()
        {
            ViewModel.OnClickedTrigger.Triggered -= OnItemClicked;
        }

        private void OnItemClicked(PieMenuItemModel itemModel)
        {
            InteractAsync(itemModel).Forget();
        }

        private async UniTaskVoid InteractAsync(PieMenuItemModel itemModel)
        {
            PieMenuModel.SetActiveState(false);
            await _pieMenuInteractionService.InteractAsync(itemModel, _parameters);
            PieMenuModel.SetActiveState(true);
            RemovePieMenu().Forget();
        }

        public async UniTask AddItemsAsync(List<PieMenuItemModel> items)
        {
            await ViewModel.AddAsync(items, _itemsHolder, destroyCancellationToken);
            _pieMenuItemSelector.Initialize(PieMenuSettingsModel);
            ActivateMenuAsync(true).Forget();
        }

        public void RemoveItems()
        {
            ViewModel.RemoveItems();
        }

        public Dictionary<int, PieMenuItemController> GetMenuItems()
        {
            return ViewModel.PieMenuItems;
        }

        private void InitializePieMenu()
        {
            ReadDataAndInfoFields();

            _inputDeviceGetter.Initialize(gameObject);
            _generalSettings.Initialize(this);
            _itemsHolder.rotation = Quaternion.Euler(0f, 0f, PieMenuModel.Rotation);
        }

        private void ReadDataAndInfoFields()
        {
            PieMenuModel.SetFillAmount(1f);
            PieMenuModel.SetMenuItemInitialSize(_generalSettings.ItemSize);
            PieMenuModel.SetMenuItemSize(_generalSettings.ItemSize);
            PieMenuModel.SetScale(_generalSettings.Scale);
            PieMenuModel.SetSpacing(_generalSettings.MenuItemSpacing);
            PieMenuModel.SetRotation(_generalSettings.GlobalRotation);

            Vector2 anchoredPosition = _rectTransform.anchoredPosition;
            float difference = (float) Screen.width / Screen.currentResolution.width;
            anchoredPosition = new(anchoredPosition.x * difference, anchoredPosition.y * difference);
            PieMenuModel.SetAnchoredPosition(anchoredPosition);
        }

        private async UniTask RemovePieMenu()
        {
            _playerService.Player.ChangeLookActive(true);
            _playerService.Player.ChangeMovementActive(true);
            await ActivateMenuAsync(false);
            _pieMenuService.RemovePieMenuAsync().Forget();
        }

        private void Update()
        {
            if (!PieMenuModel.IsActive) {
                return;
            }
            
            if (_playerService.Player.InteractionButtonPressed) {
                return;
            }

            if (_closing) {
                return;
            }

            _closing = true;
            RemovePieMenu().Forget();
        }

        private async UniTask ActivateMenuAsync(bool isActive, CancellationToken token = default)
        {
            DisableInfoPanel();
            if (isActive) {
                await ShowPieMenu(token);
            } else {
                await HidePieMenu(token);
            }
        }

        private async UniTask ShowPieMenu(CancellationToken token)
        {
            this.SetActive(true);
            _generalSettings.PlayAnimation(PieMenuUtils.TriggerActiveTrue);
            await WaitForAnimationFinish(true, token);
        }

        private async UniTask HidePieMenu(CancellationToken token)
        {
            _pieMenuItemSelector.ToggleSelection(false);
            _generalSettings.PlayAnimation(PieMenuUtils.TriggerActiveFalse);
            await WaitForAnimationFinish(false, token);
        }

        private async UniTask WaitForAnimationFinish(bool isActive, CancellationToken cancellationToken)
        {
            float timeToWait = CalculateTimeToWait(this);
            await UniTask.WaitForSeconds(timeToWait, cancellationToken: cancellationToken);

            if (isActive) {
                EnableInfoPanel();
                _pieMenuItemSelector.ToggleSelection(true);
                _pieMenuItemSelector.EnableClickDetecting();
            } else {
                this.SetActive(false);
            }

            PieMenuModel.SetActiveState(isActive);
        }

        private float CalculateTimeToWait(PieMenuController pieMenu)
        {
            PieMenuModel pieMenuModel = pieMenu.PieMenuModel;
            AnimationClip? animationClip = pieMenuModel.Animation;
            return animationClip != null ? animationClip.length : 0f;
        }

        private void DisableInfoPanel()
        {
            if (PieMenuModel.InfoPanelEnabled) {
                _generalSettings.SetActiveInfoPanel(false);
            }
        }

        private void EnableInfoPanel()
        {
            if (!PieMenuModel.InfoPanelEnabled) {
                return;
            }

            _generalSettings.SetActiveInfoPanel(true);
            _generalSettings.ModifyHeaderText(string.Empty);
            _generalSettings.ModifyDetailsText(string.Empty);
        }
    }
}