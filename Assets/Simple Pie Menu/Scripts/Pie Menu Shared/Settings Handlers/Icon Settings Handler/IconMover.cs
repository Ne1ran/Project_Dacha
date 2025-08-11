using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Icon_Settings_Handler
{
    public class IconMover : MonoBehaviour
    {
        public void Move(Transform icon, int offsetFromCenter)
        {
            RectTransform rectTransform = icon.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector3(offsetFromCenter, 0f, 0f);
        }
    }
}
