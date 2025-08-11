using System.Collections.Generic;
using Core.Resources.Binding.Attributes;
using Game.Interactable.PieMenu.Input_Devices;
using Game.Interactable.PieMenu.Menu_Item_Selection;
using Game.Interactable.PieMenu.Menu_Item;
using Game.Interactable.PieMenu.Menu_Toggler;
using Game.Interactable.PieMenu.References;
using Game.Interactable.PieMenu.Settings;
using Game.Interactable.ViewModel;
using UnityEngine;

namespace Game.Interactable.PieMenu.UI
{
    [PrefabPath("UI/Dialogs/PlayMode/pfPieMenu")]
    public class PieMenuController : MonoBehaviour
    {
        private const string SETTINGS_NAME = "Settings";

        private PieMenuSettingsModel _settingsModel = null!;
        private PieMenuDrawer _drawer = new();

        [ComponentBinding]
        private PieMenuMain _pieMenu = null!;
        [ComponentBinding("MenuItemTemplate")]
        private PieMenuItem _menuItemTemplate = null!;

        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuModel _pieMenuModel = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuElements _pieMenuElements = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private MenuItemsTracker _pieMenuItemsTracker = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private MenuItemSelector _pieMenuItemSelector = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuToggler _pieMenuToggler = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private InputDeviceGetter _inputDeviceGetter = null!;
        [ComponentBinding(SETTINGS_NAME)]
        private PieMenuGeneralSettings _generalSettings = null!;
        

        public void Initialize(List<PieMenuItemController> items)
        {
            _settingsModel = new(_menuItemTemplate, _pieMenuModel, _pieMenuElements, _pieMenuItemsTracker, _pieMenuItemSelector, _pieMenuToggler,
                                 _inputDeviceGetter, _generalSettings);
            _pieMenu.Initialize(_settingsModel);
            _drawer.Initialize(_settingsModel);
            
            _drawer.Add(_pieMenu, items);
        }
    }
}