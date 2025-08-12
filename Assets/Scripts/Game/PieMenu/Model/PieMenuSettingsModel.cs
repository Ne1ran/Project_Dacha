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
        public MenuItemSelector MenuItemSelector { get; private set; }
        public InputDeviceGetter InputDeviceGetter { get; private set; }
        public PieMenuGeneralSettings GeneralSettings { get; private set; }
        public PieMenuViewModel PieMenuViewModel { get; private set; }

        public PieMenuSettingsModel(PieMenuItemController menuItemControllerTemplate,
                                    PieMenuModel pieMenuModel,
                                    MenuItemSelector menuItemSelector,
                                    InputDeviceGetter inputDeviceGetter,
                                    PieMenuGeneralSettings generalSettings,
                                    PieMenuViewModel pieMenuViewModel)
        {
            MenuItemControllerTemplate = menuItemControllerTemplate;
            PieMenuModel = pieMenuModel;
            MenuItemSelector = menuItemSelector;
            InputDeviceGetter = inputDeviceGetter;
            GeneralSettings = generalSettings;
            PieMenuViewModel = pieMenuViewModel;
        }
    }
}