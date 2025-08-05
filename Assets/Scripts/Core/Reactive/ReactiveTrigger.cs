namespace Core.Reactive
{
    public class ReactiveTrigger
    {
        public delegate void TriggeredHandler();

        public event TriggeredHandler? Triggered;

        public void Set()
        {
            Triggered?.Invoke();
        }
    }

    public class ReactiveTrigger<T>
    {
        public delegate void TriggeredHandler(T value);

        public event TriggeredHandler? Triggered;

        public void Set(T value)
        {
            Triggered?.Invoke(value);
        }
    }
}