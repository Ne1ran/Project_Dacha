using System;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Items.Service;
using Game.Player.Controller;
using Game.Player.Service;
using Game.Tools.Service;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Drop.Service
{
    [UsedImplicitly]
    public class DropService : IInitializable, IDisposable
    {
        private readonly ISubscriber<string, InventoryChangedEvent> _inventoryChangedSubscriber;
        private readonly PlayerService _playerService;
        private readonly PickUpItemService _pickUpItemService;
        private readonly IDescriptorService _descriptorService;

        private IDisposable? _disposable;

        private readonly LayerMask _layerMask;

        public DropService(ISubscriber<string, InventoryChangedEvent> inventoryChangedSubscriber,
                           PlayerService playerService,
                           PickUpItemService pickUpItemService,
                           IDescriptorService descriptorService)
        {
            _inventoryChangedSubscriber = inventoryChangedSubscriber;
            _playerService = playerService;
            _pickUpItemService = pickUpItemService;
            _descriptorService = descriptorService;
            _layerMask = LayerMask.GetMask("Default", "Ground"); // todo neiran to combined layer mask to workaround
        }

        public void Initialize()
        {
            _disposable = _inventoryChangedSubscriber.Subscribe(InventoryChangedEvent.REMOVED, OnItemRemovedFromInventory);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnItemRemovedFromInventory(InventoryChangedEvent evt)
        {
            CreateItemInWorldAsync(evt.Item).Forget();
        }

        private async UniTask<ItemController> CreateItemInWorldAsync(InventoryItem inventoryItem)
        {
            PlayerController player = _playerService.Player;
            switch (inventoryItem.ItemType) {
                case ItemType.TOOL:
                    ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
                    ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(item => item.ItemId == inventoryItem.Id);
                    if (itemDescriptorModel == null) {
                        throw new ArgumentException($"Descriptor not found for item with id={inventoryItem.Id}");
                    }

                    Vector3 dropPosition = GetDropPosition(player.transform.position, player.Forward, itemDescriptorModel.DropOffsetMultiplier);
                    return (await _pickUpItemService.DropItemAsync(itemDescriptorModel.ItemId, itemDescriptorModel.ItemType, dropPosition));
                default:
                    throw new NotImplementedException("Need to implement other item types");
            }
        }

        private Vector3 GetDropPosition(Vector3 spawnPosition, Vector3 direction, float distance)
        {
            bool result = Physics.Raycast(spawnPosition, direction, distance, _layerMask);
            return result ? spawnPosition + new Vector3(0f, 2.5f, 0f) : spawnPosition + direction * distance;
        }
    }
}