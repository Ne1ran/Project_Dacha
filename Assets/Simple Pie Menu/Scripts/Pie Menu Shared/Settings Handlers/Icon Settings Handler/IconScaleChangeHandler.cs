using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Icon_Settings_Handler
{
    [RequireComponent(typeof(IconScaler))]
    [ExecuteInEditMode]
    public class IconScaleChangeHandler : MonoBehaviour
    {
        private IconScaler iconScaler;

        private void OnEnable()
        {
            iconScaler = GetComponent<IconScaler>();
        }

        public void Handle(PieMenu pieMenu, IconGetter iconGetter, float iconScale)
        {
            ChangeMenuItemsScale(pieMenu, iconGetter, iconScale);
        }

        private void ChangeMenuItemsScale(PieMenu pieMenu, IconGetter iconGetter, float iconScale)
        {
            foreach (KeyValuePair<int, PieMenuItem> menuItem in pieMenu.GetMenuItems())
            {
                iconScaler.ChangeScale(iconGetter.GetIcon(menuItem.Value.transform), iconScale);
            }
        }
    }
}
