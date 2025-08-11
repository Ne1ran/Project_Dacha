using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    public class PieMenuCloseFunctionalitySettingsHandler : MonoBehaviour
    {
        public void Handle(PieMenu pieMenu, bool closeable)
        {
            pieMenu.PieMenuInfo.SetCloseableState(closeable);
        }
    }
}
