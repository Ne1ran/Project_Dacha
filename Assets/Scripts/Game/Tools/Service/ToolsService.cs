using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Service;
using Game.Tools.Component;
using Game.Tools.Descriptors;
using UnityEngine;

namespace Game.Tools.Service
{
    [Service]
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

        public async UniTask<ToolController> CreateTool(string toolId, Transform parent)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            List<ToolsDescriptorModel> tools = toolsDescriptor.ToolsDescriptors;
            ToolsDescriptorModel toolsDescriptorModel = tools.Find(tool => tool.ToolId == toolId);
            if (toolsDescriptorModel == null) {
                throw new ArgumentException($"Tool not found with id={toolId}");
            }

            ToolController toolController = await _resourceService.LoadObjectAsync<ToolController>(toolsDescriptorModel.ToolPrefab);
            toolController.transform.SetParent(parent);
            toolController.name = toolsDescriptorModel.ToolId;
            return toolController;
        }
    }
}