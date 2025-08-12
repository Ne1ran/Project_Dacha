using System.Collections.Generic;
using System.Threading;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Interactable.ViewModel;
using Game.PieMenu.InputDevices;
using Game.PieMenu.Model;
using Game.PieMenu.Service;
using Game.PieMenu.Settings;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.Player.Service;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.PieMenu.UI
{
    [PrefabPath("UI/Dialogs/PlayMode/pfPieMenu")]
    public class PieMenuController : MonoBehaviour
    {
        private const string SETTINGS_NAME = "Settings";

        [ComponentBinding]
        private PieMenuController _pieMenu = null!;
        [ComponentBinding]
        private RectTransform _rectTransform = null!;
        [ComponentBinding("MenuItemTemplate")]
        private PieMenuItemController _menuItemControllerTemplate = null!;

        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuModel _pieMenuModel = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuElementsModel _pieMenuElementsModel = null!;
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

        public PieMenuViewModel ViewModel { get; private set; } = new();
        public PieMenuModel PieMenuModel { get; private set; } = null!;
        public PieMenuSettingsModel PieMenuSettingsModel { get; private set; } = null!;

        private readonly InputDeviceGetter _inputDeviceGetter = new();
        private bool _closing = false;

        public void Initialize()
        {
            PieMenuSettingsModel = new(_menuItemControllerTemplate, _pieMenuModel, _pieMenuElementsModel, _pieMenuItemSelector, _inputDeviceGetter,
                                       _generalSettings, ViewModel);

            PieMenuSettingsModel = PieMenuSettingsModel;
            PieMenuModel = PieMenuSettingsModel.PieMenuModel;
            InitializePieMenu();

            ViewModel.Initialize(_resourceService);
            ViewModel.OnClickedTrigger.Triggered += OnItemClicked;
        }

        private void OnDestroy()
        {
            ViewModel.OnClickedTrigger.Triggered -= OnItemClicked;
        }

        private void OnItemClicked(PieMenuItemModel itemModel)
        {
            Debug.Log($"OnItemClicked: {itemModel}. Title={itemModel.Title} Description={itemModel.Description} Icon={itemModel.IconPath}");
            
            RemovePieMenu().Forget();
        }

        public async UniTask AddItemsAsync(List<PieMenuItemModel> items)
        {
            await ViewModel.AddAsync(_pieMenu, items, destroyCancellationToken);
        }

        public void RemoveItems()
        {
            ViewModel.RemoveItems(this);
        }

        public Dictionary<int, PieMenuItemController> GetMenuItems()
        {
            return ViewModel.PieMenuItems;
        }

        private void InitializePieMenu()
        {
            _inputDeviceGetter.Initialize(gameObject);
            _pieMenuItemSelector.Initialize(this, PieMenuSettingsModel);
            _generalSettings.Initialize(this);
            ReadDataAndInfoFields();
            ActivateMenuAsync(true).Forget();
            _menuItemControllerTemplate.transform.SetActive(false);
        }

        private void ReadDataAndInfoFields()
        {
            PieMenuModel.SetFillAmount(_menuItemControllerTemplate.GetComponent<Image>().fillAmount);

            float menuItemInitialSize = _menuItemControllerTemplate.SizeDeltaX;
            PieMenuModel.SetMenuItemInitialSize((int) menuItemInitialSize);

            float menuItemSize = _menuItemControllerTemplate.GetComponent<RectTransform>().sizeDelta.x;
            PieMenuModel.SetMenuItemSize((int) menuItemSize);
            float scale = PieMenuGeneralSettings.CalculatePieMenuScale(this, (int) menuItemSize);
            PieMenuModel.SetScale(scale);

            int rotation = (int) _pieMenuElementsModel.MenuItemsDir.rotation.eulerAngles.z;
            PieMenuModel.SetRotation(rotation);

            PieMenuModel.SetAnchoredPosition(_rectTransform);
        }

        private async UniTask RemovePieMenu()
        {
            await ActivateMenuAsync(false);
            _pieMenuService.RemovePieMenuAsync().Forget();
        }

        // todo neiran check for button pressed. If no or escape = close!
        private void Update()
        {
            if (_playerService.Player.InteractionButtonPressed) {
                return;
            }

            if (_closing) {
                return;
            }

            _closing = true;
            RemovePieMenu().Forget();
        }

        public async UniTask ActivateMenuAsync(bool isActive, CancellationToken token = default)
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
            _generalSettings.PlayAnimation(_pieMenuElementsModel.Animator, PieMenuGeneralSettings.TriggerActiveTrue);
            await WaitForAudioAndAnimationToFinishPlaying(true, token);
        }

        private async UniTask HidePieMenu(CancellationToken token)
        {
            _pieMenuItemSelector.ToggleSelection(false);
            _generalSettings.PlayAnimation(_pieMenuElementsModel.Animator, PieMenuGeneralSettings.TriggerActiveFalse);
            await WaitForAudioAndAnimationToFinishPlaying(false, token);
        }

        private async UniTask WaitForAudioAndAnimationToFinishPlaying(bool isActive, CancellationToken cancellationToken)
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
            float audioClipLength = 0f;
            if (pieMenuModel.MouseClick != null) {
                audioClipLength = pieMenuModel.MouseClick.length;
            }

            float animationClipLength = 0f;
            if (pieMenuModel.Animation != null) {
                animationClipLength = pieMenuModel.Animation.length;
            }

            return Mathf.Max(audioClipLength, animationClipLength);
        }

        private void DisableInfoPanel()
        {
            if (PieMenuModel.InfoPanelEnabled) {
                _generalSettings.SetActive(this, false);
            }
        }

        private void EnableInfoPanel()
        {
            if (!PieMenuModel.InfoPanelEnabled) {
                return;
            }

            _generalSettings.SetActive(this, true);
            _generalSettings.ModifyHeader(this, string.Empty);
            _generalSettings.ModifyDetails(this, string.Empty);
        }
    }
}