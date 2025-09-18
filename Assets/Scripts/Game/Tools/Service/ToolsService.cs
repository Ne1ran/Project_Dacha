using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Parameters;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Tools.Component;
using Game.Tools.Descriptors;
using UnityEngine;

namespace Game.Tools.Service
{
    [Service]
    public class ToolsService
    {
        private readonly ToolUseHandlerFactory _toolUseHandlerFactory;
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;

        public ToolsService(ToolUseHandlerFactory toolUseHandlerFactory, IDescriptorService descriptorService, IResourceService resourceService)
        {
            _toolUseHandlerFactory = toolUseHandlerFactory;
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

            ToolController toolController = await _resourceService.InstantiateAsync<ToolController>(toolsDescriptorModel.ToolPrefab.AssetGUID);
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

            ToolController toolController = await _resourceService.InstantiateAsync<ToolController>(toolsDescriptorModel.ToolPrefab.AssetGUID);
            toolController.transform.SetParent(parent);
            toolController.name = toolsDescriptorModel.ToolId;
            return toolController;
        }

        public async UniTask UseToolAsync(string toolId, Parameters parameters)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            List<ToolsDescriptorModel> tools = toolsDescriptor.ToolsDescriptors;
            ToolsDescriptorModel toolsDescriptorModel = tools.Find(tool => tool.ToolId == toolId);
            if (toolsDescriptorModel == null) {
                throw new ArgumentException($"Tool not found with id={toolId}");
            }

            await _toolUseHandlerFactory.Create(toolsDescriptorModel.UseHandler).UseAsync(parameters);
        }
    }
}