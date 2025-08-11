using System.Collections.Generic;
using Game.Interactable.PieMenu.Menu_Item_Selection;
using Game.Interactable.PieMenu.Menu_Item;
using Game.Interactable.PieMenu.Menu_Toggler;
using Game.Interactable.PieMenu.References;
using Game.Interactable.PieMenu.Settings;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Interactable.PieMenu.UI
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
            MenuItemsTracker.Initialize(PieMenuElements.MenuItemsDir);
            MenuItemSelector.Initialize(this, PieMenuSettingsModel);
            GeneralSettings.Initialize(this);
            ReadDataAndInfoFields();

            PieMenuToggler.SetActive(this, true);
            MenuItemTemplate.transform.SetActive(false);
        }

        private void ReadDataAndInfoFields()
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