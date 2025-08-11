using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler
{
    public class MenuItemNameChanger : MonoBehaviour
    {
        public void Change(Dictionary<int, PieMenuItem> menuItems)
        {
            string name = "Menu Item ";
            int iteration = 0;

            foreach (KeyValuePair<int, PieMenuItem> item in menuItems)
            {
                item.Value.transform.name = name + iteration;
                iteration++;
            }
        }
    }
}
