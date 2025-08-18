using Core.Attributes;
using Core.Conditions.Checker;
using Game.Common.Handlers;

namespace Core.Conditions.Service
{
    [Factory]
    public class ConditionFactory : HandlerFactory<IConditionChecker>
    {
    }
}