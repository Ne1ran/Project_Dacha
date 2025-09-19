using System;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Player.Controller;
using Game.Player.Service;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Drop.Service
{
    [Service]
    public class DropService : IInitializable, IDisposable
    {
        private readonly PlayerService _playerService;
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;
        private readonly ISubscriber<string, InventoryChangedEvent> _inventoryChangedSubscriber;

        private IDisposable? _disposable;

        private readonly LayerMask _layerMask;

        public DropService(PlayerService playerService,
                           IResourceService resourceService,
                           IDescriptorService descriptorService,
                           ISubscriber<string, InventoryChangedEvent> inventoryChangedSubscriber)
        {
            _inventoryChangedSubscriber = inventoryChangedSubscriber;
            _playerService = playerService;
            _resourceService = resourceService;
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
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptorModel = itemsDescriptor.FindById(inventoryItem.Id);
            Vector3 dropPosition = GetDropPosition(player.transform.position, player.Forward, itemDescriptorModel.DropOffsetMultiplier);
            ItemController itemController =
                    await _resourceService.InstantiateAsync<ItemController>(itemDescriptorModel.WorldPrefab.AssetGUID, dropPosition);
            itemController.Initialize(itemDescriptorModel.Id);
            return itemController;
        }

        private Vector3 GetDropPosition(Vector3 spawnPosition, Vector3 direction, float distance)
        {
            bool result = Physics.Raycast(spawnPosition, direction, distance, _layerMask);
            return result ? spawnPosition + new Vector3(0f, 2.5f, 0f) : spawnPosition + direction * distance;
        }
    }
}