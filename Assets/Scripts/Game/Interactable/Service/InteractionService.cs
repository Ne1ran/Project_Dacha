using Core.Attributes;
using Core.Parameters;
using Game.Interactable.Handlers;

namespace Game.Interactable.Service
{
    [Service]
    public class InteractionService
    {
        private readonly InteractionHandlerFactory _factory;

        public InteractionService(InteractionHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Interact(string handlerName, Parameters parameters)
        {
            IInteractionHandler interactionHandler = _factory.Create(handlerName);
            interactionHandler.Interact(parameters);
        }
    }
}