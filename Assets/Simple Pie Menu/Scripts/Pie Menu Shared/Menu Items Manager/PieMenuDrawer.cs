using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Items_Manager
{
    public class PieMenuDrawer : MonoBehaviour
    {
        private PieMenuModel _pieMenuModel = null!;
        private int _menuItemCount;
        private int _menuItemSpacing;
        private PieMenuGeneralSettings _generalSettings = null!;

        public void Redraw(PieMenuMain pieMenu)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            _generalSettings = pieMenu.PieMenuSettingsModel.GeneralSettings;
            _pieMenuModel = pieMenu.PieMenuModel;
            _menuItemCount = menuItemsDir.childCount;
            _menuItemSpacing = _pieMenuModel.MenuItemSpacing;
            int rotation = _pieMenuModel.Rotation;

            _generalSettings.HandleRotationChange(pieMenu, 0);

            pieMenu.MenuItemsTracker.Initialize(menuItemsDir);

            _generalSettings.UpdateButtons(pieMenu, _menuItemCount, _menuItemSpacing);

            ManageMenuItemSpacing(pieMenu);
            _generalSettings.HandleRotationChange(pieMenu, rotation);
        }

        private void ManageMenuItemSpacing(PieMenuMain pieMenu)
        {
            int preservedSpacing = _pieMenuModel.MenuItemPreservedSpacing;
            int newSpacing;
            if (_menuItemCount == 1)
            {
                _pieMenuModel.SetPreservedSpacing(_menuItemSpacing);
                newSpacing = 0;
                _generalSettings.HandleButtonSpacingChange(pieMenu, _menuItemCount, newSpacing);
            }
            else if (preservedSpacing != -1)
            {
                newSpacing = preservedSpacing;
                _pieMenuModel.SetPreservedSpacing(-1);
                _generalSettings.HandleButtonSpacingChange(pieMenu, _menuItemCount, newSpacing);
            }
        }
    }
}
