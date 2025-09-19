using Core.Attributes;
using Core.Descriptors.Service;
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
        private readonly IDescriptorService _descriptorService;
        private readonly IResourceService _resourceService;

        public WorldItemService(IDescriptorService descriptorService, IResourceService resourceService)
        {
            _descriptorService = descriptorService;
            _resourceService = resourceService;
        }

        public async UniTask<T> CreateItemInWorldAsync<T>(string itemId, Vector3 position)
                where T : ItemController
        {
            ItemsDescriptor descriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptorModel = descriptor.FindById(itemId);

            T itemController = await _resourceService.InstantiateAsync<T>(itemDescriptorModel.WorldItemPrefab.AssetGUID);
            itemController.transform.position = position;
            return itemController;
        }

        public async UniTask<T> CreateItemInWorldAsync<T>(string itemId, Transform parent)
                where T : ItemController
        {
            ItemsDescriptor descriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptorModel = descriptor.FindById(itemId);

            T itemController = await _resourceService.InstantiateAsync<T>(itemDescriptorModel.WorldItemPrefab.AssetGUID);
            itemController.transform.SetParent(parent);
            return itemController;
        }

        public async UniTask<ItemController> CreateItemInWorldAsync(string itemId, Vector3? position = null, Transform? parent = null)
        {
            ItemsDescriptor descriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptorModel = descriptor.FindById(itemId);
            ItemController itemController = await _resourceService.InstantiateAsync<ItemController>(itemDescriptorModel.WorldItemPrefab.AssetGUID);
            if (parent != null) {
                itemController.transform.SetParent(parent);
            }

            itemController.transform.position = position ?? Vector3.zero;
            return itemController;
        }
    }
}