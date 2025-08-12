using System.Collections.Generic;
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

        public PieMenuModel PieMenuModel { get; private set; } = null!;
        public PieMenuElements PieMenuElements { get; private set; } = null!;
        public PieMenuViewModel ViewModel { get; private set; } = null!;
        public PieMenuItemController MenuItemControllerTemplate { get; private set; } = null!;
        public PieMenuToggler PieMenuToggler { get; private set; } = null!;
        public MenuItemSelector MenuItemSelector { get; private set; } = null!;
        public PieMenuGeneralSettings GeneralSettings { get; private set; } = null!;
        public PieMenuSettingsModel PieMenuSettingsModel { get; private set; } = null!;

        [ComponentBinding]
        private PieMenuController _pieMenu = null!;
        [ComponentBinding("MenuItemTemplate")]
        private PieMenuItemController _menuItemControllerTemplate = null!;

        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuModel _pieMenuModel = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuElements _pieMenuElements = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private MenuItemSelector _pieMenuItemSelector = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuToggler _pieMenuToggler = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private InputDeviceGetter _inputDeviceGetter = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuGeneralSettings _generalSettings = null!;

        [Inject]
        private readonly IResourceService _resourceService = null!;
        [Inject]
        private readonly PieMenuService _pieMenuService = null!;

        private readonly PieMenuViewModel _viewModel = new();

        public void Initialize()
        {
            PieMenuSettingsModel = new(_menuItemControllerTemplate, _pieMenuModel, _pieMenuElements, _pieMenuItemSelector, _pieMenuToggler,
                                       _inputDeviceGetter, _generalSettings, _viewModel);

            PieMenuSettingsModel = PieMenuSettingsModel;
            PieMenuModel = PieMenuSettingsModel.PieMenuModel;
            PieMenuElements = PieMenuSettingsModel.PieMenuElements;
            ViewModel = PieMenuSettingsModel.PieMenuViewModel;
            MenuItemControllerTemplate = PieMenuSettingsModel.MenuItemControllerTemplate;
            MenuItemSelector = PieMenuSettingsModel.MenuItemSelector;
            GeneralSettings = PieMenuSettingsModel.GeneralSettings;
            PieMenuToggler = PieMenuSettingsModel.PieMenuToggler;
            InitializePieMenu();

            _viewModel.Initialize(_resourceService);
        }

        public async UniTask AddItemsAsync(List<PieMenuItemModel> items)
        {
            await _viewModel.AddAsync(_pieMenu, items, destroyCancellationToken);
        }

        public void RemoveItems()
        {
            _viewModel.RemoveItems(this);
        }

        public Dictionary<int, PieMenuItemController> GetMenuItems()
        {
            return ViewModel.PieMenuItems;
        }

        private void InitializePieMenu()
        {
            MenuItemSelector.Initialize(this, PieMenuSettingsModel);
            GeneralSettings.Initialize(this);
            ReadDataAndInfoFields();
            PieMenuToggler.SetActive(this, true);
            MenuItemControllerTemplate.transform.SetActive(false);
        }

        private void ReadDataAndInfoFields()
        {
            PieMenuModel.SetFillAmount(MenuItemControllerTemplate.GetComponent<Image>().fillAmount);

            float menuItemInitialSize = MenuItemControllerTemplate.SizeDeltaX;
            PieMenuModel.SetMenuItemInitialSize((int) menuItemInitialSize);

            float menuItemSize = MenuItemControllerTemplate.GetComponent<RectTransform>().sizeDelta.x;
            PieMenuModel.SetMenuItemSize((int) menuItemSize);
            float scale = PieMenuGeneralSettings.CalculatePieMenuScale(this, (int) menuItemSize);
            PieMenuModel.SetScale(scale);

            int rotation = (int) PieMenuElements.MenuItemsDir.rotation.eulerAngles.z;
            PieMenuModel.SetRotation(rotation);

            RectTransform rectTransform = GetComponent<RectTransform>();
            PieMenuModel.SetAnchoredPosition(rectTransform);
        }

        // todo neiran check for button pressed. If no or escape = close!
        private void RemovePieMenu()
        {
            _pieMenuService.RemovePieMenuAsync().Forget();
        }
    }
}