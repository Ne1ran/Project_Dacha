using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    public class PieMenuSelectionSettingsHandler : MonoBehaviour
    {
        public void ConstraintSelection(PieMenu pieMenu, bool selectionConstrained)
        {
            pieMenu.PieMenuInfo.SetSelectionConstraintState(selectionConstrained);

            SelectionCalculator selectionCalculator = pieMenu.SelectionHandler.SelectionCalculator;
            selectionCalculator.ToggleSelectionConstraint(selectionConstrained);

            if (selectionConstrained)
            {
                float maxDistance = CalculateConstraintMaxDistance(pieMenu);
                selectionCalculator.SetContraintMaxDistance((int)maxDistance);
            }
        }

        public float CalculateConstraintMaxDistance(PieMenu pieMenu)
        {
            return pieMenu.PieMenuInfo.MenuItemSize * 0.10f;
        }
    }
}
