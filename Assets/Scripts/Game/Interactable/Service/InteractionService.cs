using Game.Interactable.Handlers;
using JetBrains.Annotations;

namespace Game.Interactable.Service
{
    [UsedImplicitly]
    public class InteractionService
    {
        private readonly InteractionHandlerFactory _factory;

        public InteractionService(InteractionHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Interact(string handlerName)
        {
            IInteractionHandler interactionHandler = _factory.Create(handlerName);
            interactionHandler.Interact();
        }
    }
}