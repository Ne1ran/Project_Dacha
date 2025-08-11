using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler
{
    [ExecuteInEditMode]
    public class MenuItemTemplate : MonoBehaviour
    {
        [SerializeField] GameObject menuItem;

        public GameObject MenuItem
        {
            get { return menuItem; }
        }

        public GameObject GetTemplate(Dictionary<int, PieMenuItem> menuItems)
        {
            GameObject template;
            if (menuItems.Count > 0)
            {
                template = menuItems[0].gameObject;
            }
            else
                template = menuItem;

            return template;
        }
    }
}
