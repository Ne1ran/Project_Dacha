using Core.Attributes;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Interactable.Handlers;
using Game.PieMenu.Model;

namespace Game.Interactable.Service
{
    [Service]
    public class PieMenuInteractionService
    {
        private readonly InteractionHandlerFactory _factory;

        public PieMenuInteractionService(InteractionHandlerFactory factory)
        {
            _factory = factory;
        }

        public async UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            IInteractionHandler interactionHandler = _factory.Create(itemModel.InteractionName);
            await interactionHandler.InteractAsync(itemModel, parameters);
        }
    }
}