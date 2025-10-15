using Core.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Items.Controller;
using Game.Items.Descriptors;
using UnityEngine;

namespace Game.Items.Service
{
    [Service]
    public class WorldItemService
    {
        private readonly ItemsDescriptor _itemsDescriptor;
        private readonly IResourceService _resourceService;

        public WorldItemService(IResourceService resourceService, ItemsDescriptor itemsDescriptor)
        {
            _resourceService = resourceService;
            _itemsDescriptor = itemsDescriptor;
        }

        public async UniTask<T> CreateItemInWorldAsync<T>(string itemId, Vector3 position)
                where T : ItemController
        {
            ItemDescriptorModel itemDescriptorModel = _itemsDescriptor.Require(itemId);

            T itemController = await _resourceService.InstantiateAsync<T>(itemDescriptorModel.WorldPrefab.AssetGUID);
            itemController.transform.position = position;
            itemController.Initialize(itemId);
            return itemController;
        }

        public async UniTask<T> CreateItemInWorldAsync<T>(string itemId, Transform parent)
                where T : ItemController
        {
            ItemDescriptorModel itemDescriptorModel = _itemsDescriptor.Require(itemId);

            T itemController = await _resourceService.InstantiateAsync<T>(itemDescriptorModel.WorldPrefab.AssetGUID);
            itemController.transform.SetParent(parent);
            itemController.Initialize(itemId);
            return itemController;
        }

        public async UniTask<ItemController> CreateItemInWorldAsync(string itemId, Vector3? position = null, Transform? parent = null)
        {
            ItemDescriptorModel itemDescriptorModel = _itemsDescriptor.Require(itemId);
            ItemController itemController = await _resourceService.InstantiateAsync<ItemController>(itemDescriptorModel.WorldPrefab.AssetGUID);
            if (parent != null) {
                itemController.transform.SetParent(parent);
            }

            itemController.transform.position = position ?? Vector3.zero;
            itemController.Initialize(itemId);
            return itemController;
        }
    }
}