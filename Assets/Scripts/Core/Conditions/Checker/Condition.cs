namespace Core.Conditions.Checker
{
    public abstract class Condition : IConditionChecker
    {
        private bool _inverted;

        public void Initialize(bool inverted, Parameters.Parameters parameters)
        {
            _inverted = inverted;
            Initialize(parameters);
        }

        public ConditionResult Check()
        {
            ConditionResult checkInternal = CheckInternal();
            bool isSatisfied = (_inverted && !checkInternal.IsAllowed) || (!_inverted && checkInternal.IsAllowed);

            ConditionResult responseModel = new(isSatisfied, checkInternal.Result);
            return responseModel;
        }

        protected abstract void Initialize(Parameters.Parameters parameters);

        protected abstract ConditionResult CheckInternal();
    }
}