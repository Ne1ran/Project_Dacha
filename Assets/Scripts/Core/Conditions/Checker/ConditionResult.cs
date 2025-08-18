namespace Core.Conditions.Checker
{
    public class ConditionResult
    {
        public bool IsAllowed { get; }
        public string Result { get; }

        public ConditionResult(bool isAllowed, string result)
        {
            IsAllowed = isAllowed;
            Result = result;
        }
    }
}