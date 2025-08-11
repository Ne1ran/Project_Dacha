using System.Collections.Generic;
using Game.Interactable.PieMenu.Menu_Item_Selection;
using Game.Interactable.PieMenu.Menu_Item;
using Game.Interactable.PieMenu.References;
using Game.Interactable.PieMenu.Settings;
using Game.Interactable.PieMenu.UI;
using UnityEngine;

namespace Game.Interactable.ViewModel
{
    public class PieMenuDrawer
    {
        private PieMenuModel _pieMenuModel = null!;
        private int _menuItemCount;
        private int _menuItemSpacing;
        private PieMenuGeneralSettings _generalSettings = null!;

        private readonly Dictionary<int, PieMenuItem> _restoredMenuItems = new();
        private PieMenuElements _pieMenuElements = null!;
        private MenuItemsTracker _tracker = null!;

        private Dictionary<int, PieMenuItem> _menuItems = new();

        public void Initialize(PieMenuSettingsModel settingsModel)
        {
            _pieMenuElements = settingsModel.PieMenuElements;
            _tracker = settingsModel.MenuItemsTracker;
            _menuItems = _tracker.PieMenuItems;
        }

        public void Add(PieMenuMain pieMenu, List<PieMenuItemController> menuItems)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;

            selectionHandler.ToggleSelection(false);

            foreach (PieMenuItemController item in menuItems) {
                Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;
                // Instantiate(item, menuItemsDir);
                pieMenu.MenuItemsTracker.Initialize(menuItemsDir);
            }

            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        public void Redraw(PieMenuMain pieMenu)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            _generalSettings = pieMenu.PieMenuSettingsModel.GeneralSettings;
            _pieMenuModel = pieMenu.PieMenuModel;
            _menuItemCount = menuItemsDir.childCount;
            _menuItemSpacing = _pieMenuModel.MenuItemSpacing;
            int rotation = _pieMenuModel.Rotation;

            _generalSettings.HandleRotationChange(pieMenu, 0);

            pieMenu.MenuItemsTracker.Initialize(menuItemsDir);

            _generalSettings.UpdateButtons(pieMenu, _menuItemCount, _menuItemSpacing);

            ManageMenuItemSpacing(pieMenu);
            _generalSettings.HandleRotationChange(pieMenu, rotation);
        }

        public void Hide(PieMenuMain pieMenu, List<int> menuItemIds)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            HideMenuItems(pieMenu, menuItemIds);
            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        public void Restore(PieMenuMain pieMenu, List<int>? menuItemIds = null)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            RestoreMenuItems(menuItemIds);
            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        private void HideMenuItems(PieMenuMain pieMenu, List<int> menuItemIds)
        {
            foreach (int id in menuItemIds) {
                MenuItemsTracker tracker = pieMenu.MenuItemsTracker;

                PieMenuItem menuItem = tracker.GetMenuItem(id);

                if (menuItem == null) {
                    Debug.Log("Could not find menu item with given id");
                } else {
                    if (tracker.HiddenMenuItems.ContainsKey(id)) {
                        continue;
                    }

                    if (pieMenu.MenuItemsTracker.PieMenuItems.Count != 1) {
                        tracker.HiddenMenuItems.Add(id, menuItem);
                        menuItem.transform.SetParent(pieMenu.PieMenuElements.HiddenMenuItemsDir);
                    } else {
                        throw new($"You are trying to hide the last Menu Item. This is not allowed as one must always remain.");
                    }
                }
            }
        }

        public void RestoreMenuItems(List<int>? menuItemIds = null)
        {
            _restoredMenuItems.Clear();
            MoveMenuItems(_menuItems, _pieMenuElements.HiddenMenuItemsDir);

            Dictionary<int, PieMenuItem> hiddenMenuItems = _tracker.HiddenMenuItems;
            int menuItemsCount = _menuItems.Count + hiddenMenuItems.Count;

            if (menuItemIds == null) {
                RestoreAllHiddenMenuItems(hiddenMenuItems, menuItemsCount);
            } else {
                RestoreSomeHiddenMenuItems(hiddenMenuItems, menuItemsCount, menuItemIds);
            }
        }

        private void RestoreAllHiddenMenuItems(Dictionary<int, PieMenuItem> hiddenMenuItems, int menuItemsCount)
        {
            for (int i = 0; i < menuItemsCount; i++) {
                if (hiddenMenuItems.ContainsKey(i)) {
                    Restore(hiddenMenuItems, i);
                } else {
                    _restoredMenuItems.Add(i, _tracker.GetMenuItem(i));
                }
            }

            MoveMenuItems(_restoredMenuItems, _pieMenuElements.MenuItemsDir);
        }

        private void RestoreSomeHiddenMenuItems(Dictionary<int, PieMenuItem> hiddenMenuItems, int menuItemsCount, List<int> menuItemIds)
        {
            ValidateMenuItemIds(hiddenMenuItems, menuItemIds);
            menuItemIds.Sort();

            int menuItemIdsCurrentIndex = 0;
            for (int i = 0; i < menuItemsCount; i++) {
                // This condition checks if the current 'i' value matches one of the 'menuItemIds'
                // and ensures that 'menuItemIdsCurrentIndex' is within a valid range to avoid errors.
                if (menuItemIdsCurrentIndex < menuItemIds.Count && i == menuItemIds[menuItemIdsCurrentIndex]) {
                    Restore(hiddenMenuItems, i);
                    menuItemIdsCurrentIndex++;
                } else {
                    PieMenuItem menuItem = _tracker.GetMenuItem(i);
                    if (menuItem != null) {
                        _restoredMenuItems.Add(i, menuItem);
                    }
                }
            }

            MoveMenuItems(_restoredMenuItems, _pieMenuElements.MenuItemsDir);
        }

        private void ValidateMenuItemIds(Dictionary<int, PieMenuItem> hiddenMenuItems, List<int> menuItemIds)
        {
            if (menuItemIds.Count > hiddenMenuItems.Count) {
                throw new("The list of Menu Items to restore you provided is longer than the actual number of hidden Menu Items.");
            }
        }

        private void Restore(Dictionary<int, PieMenuItem> hiddenMenuItems, int index)
        {
            if (hiddenMenuItems.ContainsKey(index)) {
                _restoredMenuItems.Add(index, hiddenMenuItems[index]);
                hiddenMenuItems.Remove(index);
            } else {
                throw new($"The Menu Item with id: {index} is not hidden.");
            }
        }

        private void MoveMenuItems(Dictionary<int, PieMenuItem> menuItems, Transform newParent)
        {
            foreach (KeyValuePair<int, PieMenuItem> menuItem in menuItems) {
                menuItem.Value.transform.SetParent(newParent);
            }
        }

        private void ManageMenuItemSpacing(PieMenuMain pieMenu)
        {
            int preservedSpacing = _pieMenuModel.MenuItemPreservedSpacing;
            int newSpacing;
            if (_menuItemCount == 1) {
                _pieMenuModel.SetPreservedSpacing(_menuItemSpacing);
                newSpacing = 0;
                _generalSettings.HandleButtonSpacingChange(pieMenu, _menuItemCount, newSpacing);
            } else if (preservedSpacing != -1) {
                newSpacing = preservedSpacing;
                _pieMenuModel.SetPreservedSpacing(-1);
                _generalSettings.HandleButtonSpacingChange(pieMenu, _menuItemCount, newSpacing);
            }
        }
    }
}