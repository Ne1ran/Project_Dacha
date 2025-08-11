using Simple_Pie_Menu.Scripts.CustomAttributes.OnValueChangeAttribute;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using SimplePieMenu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.Settings
{
    public class PieMenuSelectionSettings : MonoBehaviour
    {
        [Tooltip("This option disables selection in the center of your Pie Menu. " +
            "This can be especially useful when you have a large number of options, " +
            "where even slight mouse movements can easily change the selection. ")]
        [OnValueChange(nameof(OnConstraintValueChange))]
        [SerializeField]
        bool selectionConstrained;

        private PieMenu pieMenu;
        private PieMenuSelectionSettingsHandler settingsHandler;

        private void OnEnable()
        {
            pieMenu = GetComponent<PieMenu>();
            pieMenu.OnComponentsInitialized += OnSelectionSettingsInitialzed;
        }

        private void OnDisable()
        {
            pieMenu.OnComponentsInitialized -= OnSelectionSettingsInitialzed;
        }

        public void OnConstraintValueChange()
        {
            if (Application.isPlaying)
                settingsHandler.ConstraintSelection(pieMenu, selectionConstrained);
        }

        private void OnSelectionSettingsInitialzed()
        {
            settingsHandler = PieMenuShared.References.SelectionSettingsHandler;
            OnConstraintValueChange();
        }
    }
}
