using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Calculators
{
    public class MenuItemAngleCalculator : MonoBehaviour
    {
        private const int CircleDegrees = 360;

        public static void Calculate(PieMenuMain pieMenu)
        {
            int menuItemCount = pieMenu.MenuItemsTracker.PieMenuItems.Count;
            int menuItemSpacing = pieMenu.PieMenuModel.MenuItemSpacing;

            int totalSpacing = MenuItemSpacingCalculator.CalculateTotalSpacing(menuItemCount, menuItemSpacing);

            int totalMenuItemsAngle = CircleDegrees - totalSpacing;
            int menuItemAngle = totalMenuItemsAngle / menuItemCount;

            if (menuItemAngle < 0) {
                menuItemAngle = 0;
            }

            pieMenu.PieMenuModel.SetMenuItemAngle(menuItemAngle);
        }
    }
}