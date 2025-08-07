using System;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using Game.Equipment.Event;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Items.Controller;
using Game.Items.Model;
using Game.Player.Controller;
using Game.Player.Service;
using Game.Tools.Service;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Equipment.Service
{
    [UsedImplicitly]
    public class WorldEquipmentManager : IInitializable, IDisposable
    {
        private readonly PlayerService _playerService;
        private readonly ToolsService _toolsService;
        private readonly EquipmentService _equipmentService;
        private readonly ISubscriber<string, EquipmentChangedEvent> _equipmentChangedSubscriber;
        private readonly ISubscriber<string, InventoryChangedEvent> _inventoryChangedSubscriber;

        private IDisposable? _disposable;

        public WorldEquipmentManager(PlayerService playerService,
                                     ToolsService toolsService,
                                     EquipmentService equipmentService,
                                     ISubscriber<string, EquipmentChangedEvent> equipmentChangedSubscriber,
                                     ISubscriber<string, InventoryChangedEvent> inventoryChangedSubscriber)
        {
            _playerService = playerService;
            _toolsService = toolsService;
            _equipmentService = equipmentService;
            _equipmentChangedSubscriber = equipmentChangedSubscriber;
            _inventoryChangedSubscriber = inventoryChangedSubscriber;
        }

        public void Initialize()
        {
            DisposableBagBuilder builder = DisposableBag.CreateBuilder();
            builder.Add(_equipmentChangedSubscriber.Subscribe(EquipmentChangedEvent.EQUIPMENT_CHANGED, OnEquipmentChanged));
            builder.Add(_inventoryChangedSubscriber.Subscribe(InventoryChangedEvent.REMOVED, OnInventoryItemRemoved));
            _disposable = builder.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnEquipmentChanged(EquipmentChangedEvent evt)
        {
            ItemModel? newItem = evt.NewItem;
            if (newItem == null) {
                PlayerController player = _playerService.Player;
                player.UnequipItem();
                return;
            }

            EquipItemAsync(newItem).Forget();
        }

        private void OnInventoryItemRemoved(InventoryChangedEvent evt)
        {
            if (_equipmentService.CurrentlyEquippedItem?.ItemId == evt.Item.Id) {
                _playerService.Player.UnequipItem();
            }
        }

        private async UniTaskVoid EquipItemAsync(ItemModel newItem)
        {
            ItemController item = await CreateItemInWorldAsync(newItem);
            item.IsKinematic = true;
            PlayerController player = _playerService.Player;
            player.EquipItem(item);
        }

        private async UniTask<ItemController> CreateItemInWorldAsync(ItemModel oldItem)
        {
            return oldItem.ItemType switch {
                    ItemType.TOOL => (await _toolsService.CreateTool(oldItem.ItemId, Vector3.zero)),
                    _ => throw new NotImplementedException("Need to implement other item types")
            };
        }
    }
}