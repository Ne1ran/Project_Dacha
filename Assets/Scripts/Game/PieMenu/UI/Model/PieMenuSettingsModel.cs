using Game.Interactable.ViewModel;
using Game.PieMenu.InputDevices;
using Game.PieMenu.UI.Common;

namespace Game.PieMenu.UI.Model
{
    public class PieMenuSettingsModel
    {
        public PieMenuModel PieMenuModel { get; private set; }
        public MenuItemSelector MenuItemSelector { get; private set; }
        public InputDeviceGetter InputDeviceGetter { get; private set; }
        public PieMenuGeneralSettings GeneralSettings { get; private set; }
        public PieMenuViewModel PieMenuViewModel { get; private set; }

        public PieMenuSettingsModel(PieMenuModel pieMenuModel,
                                    MenuItemSelector menuItemSelector,
                                    InputDeviceGetter inputDeviceGetter,
                                    PieMenuGeneralSettings generalSettings,
                                    PieMenuViewModel pieMenuViewModel)
        {
            PieMenuModel = pieMenuModel;
            MenuItemSelector = menuItemSelector;
            InputDeviceGetter = inputDeviceGetter;
            GeneralSettings = generalSettings;
            PieMenuViewModel = pieMenuViewModel;
        }
    }
}