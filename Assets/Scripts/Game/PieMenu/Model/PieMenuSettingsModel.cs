using Game.Interactable.ViewModel;
using Game.PieMenu.InputDevices;
using Game.PieMenu.Settings;
using Game.PieMenu.UI;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;

namespace Game.PieMenu.Model
{
    public class PieMenuSettingsModel
    {
        public PieMenuItemController MenuItemControllerTemplate { get; private set; }
        public PieMenuModel PieMenuModel { get; private set; }
        public PieMenuElements PieMenuElements { get; private set; }
        public MenuItemSelector MenuItemSelector { get; private set; }
        public PieMenuToggler PieMenuToggler { get; private set; } // todo neiran maybe remove
        public InputDeviceGetter InputDeviceGetter { get; private set; }
        public PieMenuGeneralSettings GeneralSettings { get; private set; }
        public PieMenuViewModel PieMenuViewModel { get; private set; }

        public PieMenuSettingsModel(PieMenuItemController menuItemControllerTemplate,
                                    PieMenuModel pieMenuModel,
                                    PieMenuElements pieMenuElements,
                                    MenuItemSelector menuItemSelector,
                                    PieMenuToggler pieMenuToggler,
                                    InputDeviceGetter inputDeviceGetter,
                                    PieMenuGeneralSettings generalSettings,
                                    PieMenuViewModel pieMenuViewModel)
        {
            MenuItemControllerTemplate = menuItemControllerTemplate;
            PieMenuModel = pieMenuModel;
            PieMenuElements = pieMenuElements;
            MenuItemSelector = menuItemSelector;
            PieMenuToggler = pieMenuToggler;
            InputDeviceGetter = inputDeviceGetter;
            GeneralSettings = generalSettings;
            PieMenuViewModel = pieMenuViewModel;
        }
    }
}