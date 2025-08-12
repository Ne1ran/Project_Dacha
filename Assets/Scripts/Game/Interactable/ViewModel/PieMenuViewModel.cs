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
        public Dictionary<int, PieMenuItemController> PieMenuItems { get; private set; } = new();

        private PieMenuModel _pieMenuModel = null!;
        private int _menuItemCount;
        private int _menuItemSpacing;
        private PieMenuGeneralSettings _generalSettings = null!;

        private IResourceService _resourceService = null!;

        public void Initialize(IResourceService resourceService)
        {
            _resourceService = resourceService;
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

            int currentItemsCount = PieMenuItems.Count;
            for (int i = 0; i < items.Length; i++) {
                PieMenuItems.Add(i + currentItemsCount, items[i]);
            }

            Redraw(pieMenu);
            selectionHandler.ToggleSelection(true);
        }

        private async UniTask<PieMenuItemController> CreateControllerAsync(PieMenuItemModel itemModel,
                                                                           PieMenuController pieMenu,
                                                                           CancellationToken cancellationToken = default)
        {
            PieMenuItemController pieItem = await _resourceService.LoadObjectAsync<PieMenuItemController>();
            pieItem.Initialize(itemModel, pieMenu);
            return pieItem;
        }

        public void Redraw(PieMenuController pieMenu)
        {
            _menuItemCount = PieMenuItems.Count;

            _generalSettings = pieMenu.PieMenuSettingsModel.GeneralSettings;
            _pieMenuModel = pieMenu.PieMenuModel;
            _menuItemSpacing = _pieMenuModel.MenuItemSpacing;
            int rotation = _pieMenuModel.Rotation;

            _generalSettings.HandleRotationChange(pieMenu, 0);
            _generalSettings.UpdateButtons(pieMenu, _menuItemCount, _menuItemSpacing);
            ManageMenuItemSpacing(pieMenu);
            _generalSettings.HandleRotationChange(pieMenu, rotation);
        }

        public void RemoveItems(PieMenuController pieMenu)
        {
            foreach (PieMenuItemController itemController in PieMenuItems.Values) {
                itemController.DestroyObject();
            }

            PieMenuItems.Clear();
            Redraw(pieMenu);
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

        public void InitializeMenuItem(PieMenuItemController menuItem, int itemIndex)
        {
            menuItem.SetId(itemIndex);
            PieMenuItems.Add(itemIndex, menuItem);
        }

        public void RemoveMenuItem(int itemIndex)
        {
            PieMenuItemController? pieMenuItem = GetMenuItem(itemIndex);
            if (pieMenuItem == null) {
                return;
            }

            pieMenuItem.DestroyObject();
            PieMenuItems.Remove(itemIndex);
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