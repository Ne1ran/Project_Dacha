namespace Core.Conditions.Checker
{
    public interface IConditionChecker
    {
        void Initialize(bool inverted, Parameters.Parameters parameters);

        ConditionResult Check();
    }
}