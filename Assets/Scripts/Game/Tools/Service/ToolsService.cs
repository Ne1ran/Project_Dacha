using Core.Attributes;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Tools.Descriptors;

namespace Game.Tools.Service
{
    [Service]
    public class ToolsService
    {
        private readonly ToolUseHandlerFactory _toolUseHandlerFactory;
        private readonly ToolsDescriptor _toolsDescriptor;

        public ToolsService(ToolUseHandlerFactory toolUseHandlerFactory, ToolsDescriptor toolsDescriptor)
        {
            _toolUseHandlerFactory = toolUseHandlerFactory;
            _toolsDescriptor = toolsDescriptor;
        }

        public async UniTask UseToolAsync(string toolId, Parameters parameters)
        {
            await _toolUseHandlerFactory.Create(_toolsDescriptor.Require(toolId).UseHandler).UseAsync(parameters);
        }
    }
}