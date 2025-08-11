using UnityEngine;
using UnityEngine.UI;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    public class PieMenuBackgroundSettingsHandler : MonoBehaviour
    {
        public void SetActive(Image background, bool isActive)
        {
            background.gameObject.SetActive(isActive);
        }

        public void ChangeColor(Image background, Color newColor)
        {
            background.color = newColor;
        }
    }
}
