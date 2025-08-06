namespace Core.Reactive
{
    public class ReactiveProperty<T>
    {
        public delegate void ValueChangedHandler(T oldValue, T newValue);

        private T _value;

        public event ValueChangedHandler? Changed;

        public T Value
        {
            get => _value;
            set
            {
                if (value == null && _value == null) {
                    return;
                }

                if (value != null && value.Equals(_value)) {
                    return;
                }

                T oldValue = _value;
                _value = value;
                Changed?.Invoke(oldValue, _value);
            }
        }

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        public ReactiveProperty(T value)
        {
            _value = value;
        }
    }
}