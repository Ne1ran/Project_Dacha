using Core.Resources.Binding.Attributes;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection.Input_Devices;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using UnityEngine;

namespace Game.Interactable.PieMenu.UI
{
    [PrefabPath("UI/Dialogs/PlayMode/pfPieMenu")]
    public class PieMenuController : MonoBehaviour
    {
        private const string SETTINGS_NAME = "Settings";

        private PieMenuSettingsModel _settingsModel = null!;

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

        public void Initialize()
        {
            _settingsModel = new(_menuItemTemplate, _pieMenuModel, _pieMenuElements, _pieMenuItemsTracker, _pieMenuItemSelector, _pieMenuToggler,
                                 _inputDeviceGetter, _generalSettings);
            _pieMenu.Initialize(_settingsModel);
        }
    }
}