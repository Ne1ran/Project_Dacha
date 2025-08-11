using System.Collections.Generic;
using Game.Utils;
using Simple_Pie_Menu.Scripts.Menu_Item;
using Simple_Pie_Menu.Scripts.Others;
using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Pie_Menu.Scripts.Pie_Menu
{
    public class PieMenuMain : MonoBehaviour
    {
        public PieMenuModel PieMenuModel { get; private set; } = null!;
        public PieMenuElements PieMenuElements { get; private set; } = null!;
        public MenuItemsTracker MenuItemsTracker { get; private set; } = null!;
        public PieMenuItem MenuItemTemplate { get; private set; } = null!;
        public PieMenuToggler PieMenuToggler { get; private set; } = null!;
        public MenuItemSelector MenuItemSelector { get; private set; } = null!;
        public PieMenuGeneralSettings GeneralSettings { get; private set; } = null!;
        public PieMenuSettingsModel PieMenuSettingsModel { get; private set; } = null!;

        public void Initialize(PieMenuSettingsModel settingsModel)
        {
            PieMenuSettingsModel = settingsModel;
            PieMenuModel = settingsModel.PieMenuModel;
            PieMenuElements = settingsModel.PieMenuElements;
            MenuItemsTracker = settingsModel.MenuItemsTracker;
            MenuItemTemplate = settingsModel.MenuItemTemplate;
            MenuItemSelector = settingsModel.MenuItemSelector;
            GeneralSettings = settingsModel.GeneralSettings;
            PieMenuToggler = settingsModel.PieMenuToggler;
            InitializePieMenu();
        }

        public Dictionary<int, PieMenuItem> GetMenuItems()
        {
            return MenuItemsTracker.PieMenuItems;
        }

        private void InitializePieMenu()
        {
            if (PrefabIsolationModeHelper.IsInPrefabIsolationMode()) {
                return;
            }

            MenuItemsTracker.Initialize(PieMenuElements.MenuItemsDir);
            MenuItemSelector.Initialize(this, PieMenuSettingsModel);
            GeneralSettings.Initialize(this);
            ReadDataAndSetPieMenuInfoFields();
            PieMenuToggler.SetActive(this, true);
            
            MenuItemTemplate.transform.SetActive(false);
        }

        private void ReadDataAndSetPieMenuInfoFields()
        {
            PieMenuModel.SetFillAmount(MenuItemTemplate.GetComponent<Image>().fillAmount);

            float menuItemInitialSize = MenuItemTemplate.SizeDeltaX;
            PieMenuModel.SetMenuItemInitialSize((int) menuItemInitialSize);

            float menuItemSize = MenuItemTemplate.GetComponent<RectTransform>().sizeDelta.x;
            PieMenuModel.SetMenuItemSize((int) menuItemSize);
            float scale = PieMenuGeneralSettings.CalculatePieMenuScale(this, (int) menuItemSize);
            PieMenuModel.SetScale(scale);

            int rotation = (int) PieMenuElements.MenuItemsDir.rotation.eulerAngles.z;
            PieMenuModel.SetRotation(rotation);

            RectTransform rectTransform = GetComponent<RectTransform>();
            PieMenuModel.SetAnchoredPosition(rectTransform);
        }
    }
}