using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler
{
    [ExecuteInEditMode]
    public class MenuItemsCreator : MonoBehaviour
    {
        public void Create(PieMenu pieMenu, int newMenuItemsCount)
        {
            Dictionary<int, PieMenuItem> menuItems = pieMenu.MenuItemsTracker.PieMenuItems;

            int menuItemsToCreate = newMenuItemsCount - menuItems.Count;

            GameObject template = pieMenu.MenuItemTemplate.GetTemplate(menuItems);
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            for (int i = 0; i < menuItemsToCreate; i++)
            {
                GameObject newMenuItem = Instantiate(template, menuItemsDir);

                int menuItemIndex = menuItems.Count;
                pieMenu.MenuItemsTracker.InitializeMenuItem(newMenuItem.transform, menuItemIndex);
            }
        }
    }
}
