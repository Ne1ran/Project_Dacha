using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Icon_Settings_Handler
{
    public class IconScaler : MonoBehaviour
    {
        public void ChangeScale(Transform icon, float newScale)
        {
            icon.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
