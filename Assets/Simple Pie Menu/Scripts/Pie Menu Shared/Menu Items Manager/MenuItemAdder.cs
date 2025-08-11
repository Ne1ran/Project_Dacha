using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Items_Manager
{
    [RequireComponent(typeof(PieMenuDrawer))]
    public class MenuItemAdder : MonoBehaviour
    {
        private PieMenuDrawer drawer;

        private void OnEnable()
        {
            drawer = GetComponent<PieMenuDrawer>();
        }

        public void Add(PieMenu pieMenu, List<GameObject> menuItems)
        {
            MenuItemSelectionHandler selectionHandler = pieMenu.SelectionHandler;

            selectionHandler.ToggleSelection(false);

            foreach (GameObject item in menuItems)
            {
                Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;
                Instantiate(item, menuItemsDir);
                pieMenu.MenuItemsTracker.Initialize(menuItemsDir);
            }

            drawer.Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }
    }
}
