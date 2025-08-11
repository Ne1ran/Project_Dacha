using Simple_Pie_Menu.Scripts.CustomAttributes.OnValueChangeAttribute;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Singleton;
using SimplePieMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.Settings
{
    [ExecuteInEditMode]
    public class MenuItemColorSettings : MonoBehaviour
    {
        [OnValueChange(nameof(OnColorValueChange))] [SerializeField]
        Color normalColor;

        [OnValueChange(nameof(OnColorValueChange))] [SerializeField]
        Color selectedColor;

        [OnValueChange(nameof(OnColorValueChange))] [SerializeField]
        Color disabledColor;

        private PieMenu pieMenu;
        private MenuItemColorSettingsHandler settingsHandler;

        private void OnEnable()
        {
            pieMenu = GetComponent<PieMenu>();
            pieMenu.OnComponentsInitialized += InitializeMenuItemColorSettings;
        }

        private void OnDisable()
        {
            pieMenu.OnComponentsInitialized -= InitializeMenuItemColorSettings;
        }

        public void OnColorValueChange()
        {
            ColorBlock colors = new();
            colors.normalColor = normalColor;
            colors.highlightedColor = normalColor;
            colors.selectedColor = selectedColor;
            colors.disabledColor = disabledColor;

            settingsHandler.HandleColorChange(pieMenu, colors);
        }

        private void InitializeMenuItemColorSettings()
        {
            settingsHandler = PieMenuShared.References.MenuItemColorSettingsHandler;
            SetColorFields();
        }

        private void SetColorFields()
        {
            Transform menuItem = pieMenu.MenuItemTemplate.GetTemplate(pieMenu.GetMenuItems()).transform;
            ColorBlock colors = menuItem.GetComponent<Button>().colors;

            normalColor = colors.normalColor;
            selectedColor = colors.selectedColor;
            disabledColor = colors.disabledColor;
        }
    }
}
