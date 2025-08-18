using System.Collections.Generic;
using Core.Attributes;
using Core.Conditions.Checker;
using Core.Conditions.Descriptor;

namespace Core.Conditions.Service
{
    [Service]
    public class ConditionService
    {
        private readonly ConditionFactory _conditionFactory;

        public IReadOnlyList<IConditionChecker> Create(List<ConditionDescriptor> descriptors)
        {
            List<IConditionChecker> result = new(descriptors.Count);
            foreach (ConditionDescriptor descriptor in descriptors) {
                IConditionChecker condition = _conditionFactory.Create(descriptor.Id);
                condition.Initialize(descriptor.Inverted, descriptor.GetParameters());
                result.Add(condition);
            }
            
            return result;
        }

        public ConditionResult Check(IReadOnlyList<IConditionChecker> conditions)
        {
            foreach (IConditionChecker? condition in conditions) {
                ConditionResult conditionResult = condition.Check();
                if (!conditionResult.IsAllowed) {
                    return conditionResult;
                }
            }

            return new(true, string.Empty);
        }

        public ConditionResult Check(List<ConditionDescriptor> conditionDescriptors)
        {
            IReadOnlyList<IConditionChecker> conditions = Create(conditionDescriptors);

            foreach (IConditionChecker condition in conditions) {
                ConditionResult conditionResult = condition.Check();
                if (!conditionResult.IsAllowed) {
                    return conditionResult;
                }
            }

            return new(true, string.Empty);
        }

        public ConditionService(ConditionFactory conditionFactory)
        {
            _conditionFactory = conditionFactory;
        }
    }
}