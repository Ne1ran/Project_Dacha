using System;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Equipment.Event;
using Game.Harvest.Service;
using Game.Inventory.Event;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Items.Model;
using Game.Player.Controller;
using Game.Player.Service;
using Game.Seeds.Service;
using MessagePipe;
using UnityEngine;
using IInitializable = VContainer.Unity.IInitializable;

namespace Game.Equipment.Service
{
    [Manager]
    public class WorldEquipmentManager : IInitializable, IDisposable
    {
        private readonly PlayerService _playerService;
        private readonly EquipmentService _equipmentService;
        private readonly IDescriptorService _descriptorService;
        private readonly IResourceService _resourceService;
        private readonly ISubscriber<string, EquipmentChangedEvent> _equipmentChangedSubscriber;
        private readonly ISubscriber<string, InventoryChangedEvent> _inventoryChangedSubscriber;

        private IDisposable? _disposable;

        public WorldEquipmentManager(PlayerService playerService,
                                     EquipmentService equipmentService,
                                     ISubscriber<string, EquipmentChangedEvent> equipmentChangedSubscriber,
                                     ISubscriber<string, InventoryChangedEvent> inventoryChangedSubscriber,
                                     SeedsService seedsService,
                                     PlantHarvestService harvestService,
                                     IDescriptorService descriptorService,
                                     IResourceService resourceService)
        {
            _playerService = playerService;
            _equipmentService = equipmentService;
            _equipmentChangedSubscriber = equipmentChangedSubscriber;
            _inventoryChangedSubscriber = inventoryChangedSubscriber;
            _descriptorService = descriptorService;
            _resourceService = resourceService;
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
            ItemController item = await CreateItemInHandsAsync(newItem.ItemId, Vector3.zero);
            item.IsKinematic = true;
            PlayerController player = _playerService.Player;
            player.EquipItem(item);
        }

        private async UniTask<ItemController> CreateItemInHandsAsync(string itemId, Vector3? position = null)
        {
            ItemsDescriptor descriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptorModel = descriptor.FindById(itemId);
            ItemController itemController = await _resourceService.InstantiateAsync<ItemController>(itemDescriptorModel.HandsPrefab.AssetGUID);
            itemController.transform.position = position ?? Vector3.zero;
            itemController.Initialize(itemId);
            return itemController;
        }
    }
}