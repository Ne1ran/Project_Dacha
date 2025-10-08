using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Tools.Descriptors;

namespace Game.Tools.Service
{
    [Service]
    public class ToolsService
    {
        private readonly ToolUseHandlerFactory _toolUseHandlerFactory;
        private readonly IDescriptorService _descriptorService;

        public ToolsService(ToolUseHandlerFactory toolUseHandlerFactory, IDescriptorService descriptorService)
        {
            _toolUseHandlerFactory = toolUseHandlerFactory;
            _descriptorService = descriptorService;
        }

        public async UniTask UseToolAsync(string toolId, Parameters parameters)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            await _toolUseHandlerFactory.Create(toolsDescriptor.Require(toolId).UseHandler).UseAsync(parameters);
        }
    }
}