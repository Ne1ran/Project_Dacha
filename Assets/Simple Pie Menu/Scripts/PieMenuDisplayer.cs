using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts
{
    public class PieMenuDisplayer : MonoBehaviour
    {
        public void ShowPieMenu(PieMenu pieMenu)
        {
            if (pieMenu != null)
            {
                PieMenuInfo pieMenuInfo = pieMenu.PieMenuInfo;
                if (pieMenuInfo != null && !pieMenuInfo.IsActive && !pieMenuInfo.IsTransitioning)
                {            
                    PieMenuShared.References.PieMenuToggler.SetActive(pieMenu, true);
                }
                else if(pieMenuInfo == null) InitializePieMenu(pieMenu);
            }
        }
        private void InitializePieMenu(PieMenu pieMenu)
        {
            pieMenu.transform.parent.gameObject.SetActive(true);
            pieMenu.gameObject.SetActive(true);
        }
    }
}

