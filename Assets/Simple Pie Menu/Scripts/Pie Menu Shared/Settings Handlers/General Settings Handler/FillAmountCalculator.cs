using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Calculators;
using Simple_Pie_Menu.Scripts.Menu_Item;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler
{
    public class FillAmountCalculator : MonoBehaviour
    {
        private readonly float circleDegrees = 360f;

        public void ModifyFillAmount(Dictionary<int, PieMenuItem> menuItems, float fillAmount, float menuItemSpacing)
        {
            int iteration = 0;
            foreach (KeyValuePair<int, PieMenuItem> item in menuItems)
            {
                Image image = item.Value.GetComponent<Image>();
                image.fillAmount = fillAmount;

                float zAxisRotation = (fillAmount * iteration * circleDegrees) + (menuItemSpacing * iteration);
                item.Value.transform.rotation = Quaternion.Euler(0, 0, zAxisRotation);
                iteration++;
            }
        }

        public float CalculateMenuItemFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacingPercentage =
                MenuItemSpacingCalculator.CalculatTotalSpacingPercentage(menuItemCount, menuItemSpacing);

            float maxFillAmount = 1f;
            float fillAmountLeft = maxFillAmount - totalSpacingPercentage;
            float fillAmount = fillAmountLeft / menuItemCount;
            return fillAmount;
        }
    }
}
