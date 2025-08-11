using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Items_Manager
{
    [RequireComponent(typeof(PieMenuDrawer))]
    [RequireComponent(typeof(MenuItemRestorer))]
    public class MenuItemHider : MonoBehaviour
    {
        private PieMenuDrawer drawer;
        private MenuItemRestorer menuItemRestorer;

        private void OnEnable()
        {
            drawer = GetComponent<PieMenuDrawer>();
            menuItemRestorer = GetComponent<MenuItemRestorer>();
        }

        public void Hide(PieMenuMain pieMenu, List<int> menuItemIds)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            HideMenuItems(pieMenu, menuItemIds);
            drawer.Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        public void Restore(PieMenuMain pieMenu, List<int> menuItemIds = null)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            menuItemRestorer.RestoreMenuItems(pieMenu, menuItemIds);
            drawer.Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        private void HideMenuItems(PieMenuMain pieMenu, List<int> menuItemIds)
        {
            foreach (int id in menuItemIds) {
                MenuItemsTracker tracker = pieMenu.MenuItemsTracker;

                PieMenuItem menuItem = tracker.GetMenuItem(id);

                if (menuItem == null) {
                    Debug.Log("Could not find menu item with given id");
                } else {
                    if (tracker.HiddenMenuItems.ContainsKey(id)) {
                        continue;
                    }

                    if (pieMenu.MenuItemsTracker.PieMenuItems.Count != 1) {
                        tracker.HiddenMenuItems.Add(id, menuItem);
                        menuItem.transform.SetParent(pieMenu.PieMenuElements.HiddenMenuItemsDir);
                    } else {
                        throw new($"You are trying to hide the last Menu Item. This is not allowed as one must always remain.");
                    }
                }
            }
        }
    }
}