using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    public class PieMenuPositionSettingsHandler : MonoBehaviour
    {
        public void Handle(PieMenu pieMenu, int horizontalPosition, int verticalPosition)
        {
            RectTransform rectTransform = pieMenu.GetComponent<RectTransform>();
            Vector2 anchoredPosition = rectTransform.anchoredPosition;

            anchoredPosition.Set(horizontalPosition, verticalPosition);
            rectTransform.anchoredPosition = anchoredPosition;
            pieMenu.PieMenuInfo.SetAnchoredPosition(rectTransform);
        }
    }
}
