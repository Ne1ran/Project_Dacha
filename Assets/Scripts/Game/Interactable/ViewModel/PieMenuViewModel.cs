using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Reactive;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.PieMenu.Model;
using Game.PieMenu.UI;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.Utils;
using UnityEngine;

namespace Game.Interactable.ViewModel
{
    public class PieMenuViewModel
    {
        public Dictionary<int, PieMenuItemController> PieMenuItems { get; private set; } = new();

        private PieMenuModel _pieMenuModel = null!;
        private PieMenuGeneralSettings _generalSettings = null!;
        private PieMenuController _pieMenuController = null!;
        private int _menuItemCount;
        private int _menuItemSpacing;

        private IResourceService _resourceService = null!;
        
        public ReactiveTrigger<PieMenuItemModel> OnClickedTrigger { get; private set; } = new();

        public void Initialize(IResourceService resourceService, PieMenuController pieMenuController)
        {
            _resourceService = resourceService;
            _pieMenuController = pieMenuController;
            
            _generalSettings = _pieMenuController.PieMenuSettingsModel.GeneralSettings;
            _pieMenuModel = _pieMenuController.PieMenuModel;
        }

        public async UniTask AddAsync(List<PieMenuItemModel> menuItems, Transform itemsHolder, CancellationToken cancellationToken = default)
        {
            MenuItemSelector selectionHandler = _pieMenuController.PieMenuSettingsModel.MenuItemSelector;

            selectionHandler.ToggleSelection(false);

            List<UniTask<PieMenuItemController>> tasks = new();
            foreach (PieMenuItemModel item in menuItems) {
                tasks.Add(CreateControllerAsync(item, itemsHolder, cancellationToken));
            }

            PieMenuItemController[] items = await UniTask.WhenAll(tasks);
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            int currentItemsCount = PieMenuItems.Count;
            for (int i = 0; i < items.Length; i++) {
                PieMenuItems.Add(i + currentItemsCount, items[i]);
            }

            Redraw();
            selectionHandler.ToggleSelection(true);
        }

        private async UniTask<PieMenuItemController> CreateControllerAsync(PieMenuItemModel itemModel,
                                                                           Transform itemsHolder,
                                                                           CancellationToken cancellationToken = default)
        {
            PieMenuItemController pieItem = await _resourceService.LoadObjectAsync<PieMenuItemController>(itemsHolder.transform);
            pieItem.Initialize(itemModel, _pieMenuController, OnClickedTrigger);
            return pieItem;
        }

        public void Redraw()
        {
            _menuItemCount = PieMenuItems.Count;

            _menuItemSpacing = _pieMenuModel.MenuItemSpacing;
            int rotation = _pieMenuModel.Rotation;

            _generalSettings.ChangeRotation(0);
            _generalSettings.UpdateButtons(_menuItemCount, _menuItemSpacing);
            ManageMenuItemSpacing(_pieMenuController);
            _generalSettings.ChangeRotation(rotation);
        }

        public void RemoveItems()
        {
            foreach (PieMenuItemController itemController in PieMenuItems.Values) {
                itemController.DestroyObject();
            }

            PieMenuItems.Clear();
            Redraw();
        }

        private void ManageMenuItemSpacing(PieMenuController pieMenu)
        {
            int preservedSpacing = _pieMenuModel.MenuItemPreservedSpacing;
            int newSpacing;
            if (_menuItemCount == 1) {
                _pieMenuModel.SetPreservedSpacing(_menuItemSpacing);
                newSpacing = 0;
                _generalSettings.UpdateButtons(_menuItemCount, newSpacing);
            } else if (preservedSpacing != -1) {
                newSpacing = preservedSpacing;
                _pieMenuModel.SetPreservedSpacing(-1);
                _generalSettings.UpdateButtons(_menuItemCount, newSpacing);
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