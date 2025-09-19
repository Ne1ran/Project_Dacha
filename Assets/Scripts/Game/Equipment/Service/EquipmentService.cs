using System;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Equipment.Event;
using Game.Equipment.Repo;
using Game.Items.Descriptors;
using Game.Items.Model;
using MessagePipe;
using UnityEngine;

namespace Game.Equipment.Service
{
    [Service]
    public class EquipmentService
    {
        private readonly EquipmentRepo _equipmentRepo;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, EquipmentChangedEvent> _equipmentChangedPublisher;

        private IDisposable? _disposable;
        
        public EquipmentService(IDescriptorService descriptorService,
                                IPublisher<string, EquipmentChangedEvent> equipmentChangedPublisher,
                                EquipmentRepo equipmentRepo)
        {
            _descriptorService = descriptorService;
            _equipmentChangedPublisher = equipmentChangedPublisher;
            _equipmentRepo = equipmentRepo;
        }
        
        public bool TryEquipItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) {
                throw new ArgumentException("Can't equip item with empty id!");
            }

            ItemModel? equippedItem = _equipmentRepo.Get();

            if (equippedItem?.ItemId == itemId) {
                Debug.Log($"Unequipped tool={itemId}");
                Unequip();
                return false;
            }

            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel? descriptorModel = itemsDescriptor.Items.Find(descriptor => descriptor.Id == itemId);
            if (descriptorModel == null) {
                throw new ArgumentException($"Descriptor for item not found! ItemId={itemId}");
            }

            ItemModel newItem = new(descriptorModel.Id, descriptorModel.WorldPrefab, descriptorModel.Type, descriptorModel.DropOffsetMultiplier,
                                    descriptorModel.Stackable, descriptorModel.ShowInHand, descriptorModel.MaxStack);
            _equipmentChangedPublisher.Publish(EquipmentChangedEvent.EQUIPMENT_CHANGED, new(equippedItem, newItem));
            _equipmentRepo.Save(newItem);
            Debug.Log($"Equipped tool={itemId}");
            return true;
        }

        public void Unequip()
        {
            _equipmentChangedPublisher.Publish(EquipmentChangedEvent.EQUIPMENT_CHANGED, new(_equipmentRepo.Get()));
            _equipmentRepo.Clear();
        }

        public ItemModel? CurrentlyEquippedItem => _equipmentRepo.Get();
    }
}