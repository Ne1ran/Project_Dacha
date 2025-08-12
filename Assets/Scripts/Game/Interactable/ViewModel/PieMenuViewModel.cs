using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.PieMenu.Model;
using Game.PieMenu.Settings;
using Game.PieMenu.UI;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Interactable.ViewModel
{
    public class PieMenuViewModel
    {
        public Dictionary<int, Button> ButtonComponents { get; private set; } = new();
        public Dictionary<int, PieMenuItemController> PieMenuItems { get; private set; } = new();

        public Dictionary<int, PieMenuItemController> HiddenMenuItems { get; private set; } = new();

        private PieMenuModel _pieMenuModel = null!;
        private int _menuItemCount;
        private int _menuItemSpacing;
        private PieMenuGeneralSettings _generalSettings = null!;

        private readonly Dictionary<int, PieMenuItemController> _restoredMenuItems = new();
        private PieMenuElements _pieMenuElements = null!;

        private IResourceService _resourceService = null!;

        public void Initialize(PieMenuSettingsModel settingsModel, IResourceService resourceService)
        {
            _resourceService = resourceService;
            _pieMenuElements = settingsModel.PieMenuElements;
        }

        public async UniTask AddAsync(PieMenuController pieMenu, List<PieMenuItemModel> menuItems, CancellationToken cancellationToken = default)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;

            selectionHandler.ToggleSelection(false);

            List<UniTask<PieMenuItemController>> tasks = new();

            foreach (PieMenuItemModel item in menuItems) {
                tasks.Add(CreateControllerAsync(item, pieMenu, cancellationToken));
            }

            PieMenuItemController[] items = await UniTask.WhenAll(tasks);
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        private async UniTask<PieMenuItemController> CreateControllerAsync(PieMenuItemModel itemModel, PieMenuController pieMenu, CancellationToken cancellationToken = default)
        {
            PieMenuItemController pieItem = await _resourceService.LoadObjectAsync<PieMenuItemController>();
            pieItem.Initialize(itemModel, pieMenu);
            return pieItem;
        }

        public void Redraw(PieMenuController pieMenu)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            _generalSettings = pieMenu.PieMenuSettingsModel.GeneralSettings;
            _pieMenuModel = pieMenu.PieMenuModel;
            _menuItemCount = menuItemsDir.childCount;
            _menuItemSpacing = _pieMenuModel.MenuItemSpacing;
            int rotation = _pieMenuModel.Rotation;

            _generalSettings.HandleRotationChange(pieMenu, 0);
            _generalSettings.UpdateButtons(pieMenu, _menuItemCount, _menuItemSpacing);

            ManageMenuItemSpacing(pieMenu);
            _generalSettings.HandleRotationChange(pieMenu, rotation);
        }

        public void Hide(PieMenuController pieMenu, List<int> menuItemIds)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            HideMenuItems(pieMenu, menuItemIds);
            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        public void Restore(PieMenuController pieMenu, List<int>? menuItemIds = null)
        {
            MenuItemSelector selectionHandler = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selectionHandler.ToggleSelection(false);
            RestoreMenuItems(menuItemIds);
            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        private void HideMenuItems(PieMenuController pieMenu, List<int> menuItemIds)
        {
            foreach (int id in menuItemIds) {
                PieMenuItemController? menuItem = GetMenuItem(id);
                if (menuItem == null) {
                    Debug.Log("Could not find menu item with given id");
                } else {
                    if (HiddenMenuItems.ContainsKey(id)) {
                        continue;
                    }

                    if (PieMenuItems.Count != 1) {
                        HiddenMenuItems.Add(id, menuItem);
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
            MoveMenuItems(PieMenuItems, _pieMenuElements.HiddenMenuItemsDir);

            Dictionary<int, PieMenuItemController> hiddenMenuItems = HiddenMenuItems;
            int menuItemsCount = PieMenuItems.Count + hiddenMenuItems.Count;

            if (menuItemIds == null) {
                RestoreAllHiddenMenuItems(hiddenMenuItems, menuItemsCount);
            } else {
                RestoreSomeHiddenMenuItems(hiddenMenuItems, menuItemsCount, menuItemIds);
            }
        }

        private void RestoreAllHiddenMenuItems(Dictionary<int, PieMenuItemController> hiddenMenuItems, int menuItemsCount)
        {
            for (int i = 0; i < menuItemsCount; i++) {
                if (hiddenMenuItems.ContainsKey(i)) {
                    Restore(hiddenMenuItems, i);
                } else {
                    PieMenuItemController? pieMenuItem = GetMenuItem(i);
                    if (pieMenuItem != null) {
                        _restoredMenuItems.Add(i, pieMenuItem);
                    }
                }
            }

            MoveMenuItems(_restoredMenuItems, _pieMenuElements.MenuItemsDir);
        }

        private void RestoreSomeHiddenMenuItems(Dictionary<int, PieMenuItemController> hiddenMenuItems, int menuItemsCount, List<int> menuItemIds)
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
                    PieMenuItemController? menuItem = GetMenuItem(i);
                    if (menuItem != null) {
                        _restoredMenuItems.Add(i, menuItem);
                    }
                }
            }

            MoveMenuItems(_restoredMenuItems, _pieMenuElements.MenuItemsDir);
        }

        private void ValidateMenuItemIds(Dictionary<int, PieMenuItemController> hiddenMenuItems, List<int> menuItemIds)
        {
            if (menuItemIds.Count > hiddenMenuItems.Count) {
                throw new("The list of Menu Items to restore you provided is longer than the actual number of hidden Menu Items.");
            }
        }

        private void Restore(Dictionary<int, PieMenuItemController> hiddenMenuItems, int index)
        {
            if (hiddenMenuItems.ContainsKey(index)) {
                _restoredMenuItems.Add(index, hiddenMenuItems[index]);
                hiddenMenuItems.Remove(index);
            } else {
                throw new($"The Menu Item with id: {index} is not hidden.");
            }
        }

        private void MoveMenuItems(Dictionary<int, PieMenuItemController> menuItems, Transform newParent)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in menuItems) {
                menuItem.Value.transform.SetParent(newParent);
            }
        }

        private void ManageMenuItemSpacing(PieMenuController pieMenu)
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

        public void InitializeMenuItem(Transform menuItem, int itemIndex)
        {
            PieMenuItemController pieMenuItemController = menuItem.GetComponent<PieMenuItemController>();
            pieMenuItemController.SetId(itemIndex);
            PieMenuItems.Add(itemIndex, pieMenuItemController);
            Button button = menuItem.GetComponent<Button>();
            ButtonComponents.Add(itemIndex, button);
        }

        public void RemoveMenuItem(int itemIndex)
        {
            PieMenuItemController? pieMenuItem = GetMenuItem(itemIndex);
            if (pieMenuItem == null) {
                return;
            }
            
            Transform menuItem = pieMenuItem.transform;
            PieMenuItems.Remove(itemIndex);
            ButtonComponents.Remove(itemIndex);
            menuItem.DestroyObject();
        }

        public PieMenuItemController? GetMenuItem(int id)
        {
            if (PieMenuItems.ContainsKey(id)) {
                return PieMenuItems[id].Id == id ? PieMenuItems[id] : SearchForMenuItem(id);
            }

            return SearchForMenuItem(id);
        }

        private PieMenuItemController? SearchForMenuItem(int id)
        {
            // this code may be executed after using Hide Menu Item functionality, where dict index and Menu Item Id can be shifted.
            KeyValuePair<int, PieMenuItemController> pair = PieMenuItems.FirstOrDefault(item => item.Value.Id == id);
            return pair.Value == null ? null : pair.Value;
        }
    }
}