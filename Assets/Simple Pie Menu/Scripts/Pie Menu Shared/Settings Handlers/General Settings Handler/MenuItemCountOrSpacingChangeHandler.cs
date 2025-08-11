using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler
{
    [RequireComponent(typeof(MenuItemsCreator))]
    [RequireComponent(typeof(MenuItemsRemover))]
    [RequireComponent(typeof(FillAmountCalculator))]
    [ExecuteInEditMode]
    public class MenuItemCountOrSpacingChangeHandler : MonoBehaviour
    {
        private MenuItemsCreator menuItemsCreator;
        private MenuItemsRemover menuItemsRemover;
        private FillAmountCalculator fillAmountCalculator;

        private void OnEnable()
        {
            menuItemsCreator = GetComponent<MenuItemsCreator>();
            menuItemsRemover = GetComponent<MenuItemsRemover>();
            fillAmountCalculator = GetComponent<FillAmountCalculator>();
        }

        public void HandleButtonCountChange(PieMenu pieMenu, int newMenuItemCount)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;
            int currentMenuItemsCount = menuItemsDir.childCount;

            if (currentMenuItemsCount > newMenuItemCount)
            {
                menuItemsRemover.Remove(pieMenu, newMenuItemCount);
            }
            else if (currentMenuItemsCount < newMenuItemCount)
            {
                menuItemsCreator.Create(pieMenu, newMenuItemCount);
            }
        }

        public void UpdateFillAmount(PieMenu pieMenu, int menuItemCount, int menuItemSpacing)
        {

            float fillAmount = fillAmountCalculator.CalculateMenuItemFillAmount(menuItemCount, menuItemSpacing);

            pieMenu.PieMenuInfo.SetFillAmount(fillAmount);
            fillAmountCalculator.ModifyFillAmount(pieMenu.GetMenuItems(), fillAmount, menuItemSpacing);
        }
    }
}
