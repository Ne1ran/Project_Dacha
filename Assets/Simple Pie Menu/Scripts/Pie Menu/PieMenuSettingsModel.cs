using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection.Input_Devices;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;

namespace Simple_Pie_Menu.Scripts.Pie_Menu
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