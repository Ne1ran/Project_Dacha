using Game.PieMenu.UI;
using UnityEngine;

namespace Game.PieMenu.Utils
{
    public static class PieMenuUtils
    {
        public const string InputDevice = "InputDevice";
        private const int CircleDegrees = 360;

        public static void CalculateItemAngle(PieMenuController pieMenu)
        {
            int menuItemCount = pieMenu.ViewModel.PieMenuItems.Count;
            if (menuItemCount == 0) {
                Debug.Log("Can't divide by zero! Ensure menu items != 0");
                return;
            }
            
            int menuItemSpacing = pieMenu.PieMenuModel.MenuItemSpacing;

            int totalSpacing = CalculateTotalSpacing(menuItemCount, menuItemSpacing);

            int totalMenuItemsAngle = CircleDegrees - totalSpacing;
            int menuItemAngle = totalMenuItemsAngle / menuItemCount;

            if (menuItemAngle < 0) {
                menuItemAngle = 0;
            }

            pieMenu.PieMenuModel.SetMenuItemAngle(menuItemAngle);
        }
        
        public static int CalculateTotalSpacing(int menuItemCount, int menuItemSpacing)
        {
            int totalSpacing = menuItemCount * menuItemSpacing;
            return totalSpacing;
        }

        public static float CalculateTotalSpacingPercentage(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacing = CalculateTotalSpacing(menuItemCount, menuItemSpacing);
            float totalSpacingToPercentage = totalSpacing / CircleDegrees;
            return totalSpacingToPercentage;
        }
    }
}