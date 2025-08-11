using Game.Interactable.PieMenu.Input_Devices;
using Game.Interactable.PieMenu.Menu_Item_Selection;
using Game.Interactable.PieMenu.Menu_Item;
using Game.Interactable.PieMenu.Menu_Toggler;
using Game.Interactable.PieMenu.References;
using Game.Interactable.PieMenu.Settings;

namespace Game.Interactable.PieMenu.UI
{
    public class PieMenuSettingsModel
    {
        public PieMenuItem MenuItemTemplate { get; private set; } = null!;
        public PieMenuModel PieMenuModel { get; private set; } = null!;
        public PieMenuElements PieMenuElements { get; private set; } = null!;
        public MenuItemsTracker MenuItemsTracker { get; private set; } = null!;
        public MenuItemSelector MenuItemSelector { get; private set; } = null!;
        public PieMenuToggler PieMenuToggler { get; private set; } = null!; // todo neiran maybe remove
        public InputDeviceGetter InputDeviceGetter { get; private set; } = null!;
        public PieMenuGeneralSettings GeneralSettings { get; private set; } = null!;

        public PieMenuSettingsModel(PieMenuItem menuItemTemplate,
                                    PieMenuModel pieMenuModel,
                                    PieMenuElements pieMenuElements,
                                    MenuItemsTracker menuItemsTracker,
                                    MenuItemSelector menuItemSelector,
                                    PieMenuToggler pieMenuToggler,
                                    InputDeviceGetter inputDeviceGetter,
                                    PieMenuGeneralSettings generalSettings)
        {
            MenuItemTemplate = menuItemTemplate;
            PieMenuModel = pieMenuModel;
            PieMenuElements = pieMenuElements;
            MenuItemsTracker = menuItemsTracker;
            MenuItemSelector = menuItemSelector;
            PieMenuToggler = pieMenuToggler;
            InputDeviceGetter = inputDeviceGetter;
            GeneralSettings = generalSettings;
        }
    }
}