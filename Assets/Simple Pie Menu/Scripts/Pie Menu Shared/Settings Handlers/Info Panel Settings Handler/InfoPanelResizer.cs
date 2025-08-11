using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Info_Panel_Settings_Handler
{
    public class InfoPanelResizer : MonoBehaviour
    {
        public void Resize(PieMenu pieMenu, float newScale)
        {
            if (pieMenu.PieMenuInfo.InfoPanelEnabled)
            {
                PieMenuShared.References.InfoPanelSettingsHandler.ChangeScale(pieMenu, newScale);

                PieMenuInfoPanelSettings infoPanelSettings = pieMenu.transform.GetComponent<PieMenuInfoPanelSettings>();
                infoPanelSettings.SetScale(newScale);
            }
        }
    }
}
