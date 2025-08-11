using UnityEngine;

namespace Game.Interactable.PieMenu.Calculators
{
    public class MenuItemSpacingCalculator : MonoBehaviour
    {
        private const float CircleDegrees = 360;

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