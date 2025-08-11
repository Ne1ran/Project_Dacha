using Simple_Pie_Menu.Scripts.Others;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu
{
    [ExecuteInEditMode]
    public class PieMenuSharedLifecycleHandler : MonoBehaviour
    {
        [SerializeField] GameObject pieMenuShared;

        private void OnEnable()
        {
            if (PrefabIsolationModeHelper.IsInPrefabIsolationMode()) return;

            if (PieMenuShared.Instance == null)
            {
                GameObject singleton = Instantiate(pieMenuShared, null);
                singleton.name = "Pie Menu Shared";    
            }

            PieMenuShared.Instance.OnNewPieMenuCreated();
        }

        private void OnDisable()
        {
            if (PieMenuShared.Instance != null)
                PieMenuShared.Instance.OnPieMenuDestroyed();
        }
    }
}
