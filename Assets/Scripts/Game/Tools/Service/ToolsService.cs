using System;
using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Descriptors;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Tools.Component;
using Game.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Tools.Service
{
    [UsedImplicitly]
    public class ToolsService
    {
        private readonly InventoryService _inventoryService;
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;

        public ToolsService(InventoryService inventoryService, IDescriptorService descriptorService, IResourceService resourceService)
        {
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;
            _resourceService = resourceService;
        }

        public async UniTask<ToolController> CreateTool(string toolId, Vector3 position)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            List<ToolsDescriptorModel> tools = toolsDescriptor.ToolsDescriptors;
            ToolsDescriptorModel toolsDescriptorModel = tools.Find(tool => tool.ToolId == toolId);
            if (toolsDescriptorModel == null) {
                throw new ArgumentException($"Tool not found with id={toolId}");
            }

            ToolController toolController = await _resourceService.LoadObjectAsync<ToolController>(toolsDescriptorModel.ToolPrefab);
            toolController.transform.position = position;
            toolController.name = toolsDescriptorModel.ToolId;
            return toolController;
        }

        public void PickUpTool(ToolController toolController)
        {
            string toolId = toolController.GetName;
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            List<ToolsDescriptorModel> tools = toolsDescriptor.ToolsDescriptors;
            ToolsDescriptorModel toolsDescriptorModel = tools.Find(tool => tool.ToolId == toolId);
            if (toolsDescriptorModel == null) {
                throw new ArgumentException($"Tool not found with id={toolId}");
            }

            if (_inventoryService.TryAddToolToInventory(toolId)) {
                toolController.gameObject.DestroyObject();
            }
        }
    }
}