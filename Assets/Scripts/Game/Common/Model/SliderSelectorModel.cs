namespace Game.Common.Model
{
    public readonly struct SliderSelectorModel
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float StartValue { get; }
        public float StepValue { get; }
        public int RoundDigits { get; }

        public SliderSelectorModel(float minValue, float maxValue, float startValue, float stepValue, int roundDigits)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            StartValue = startValue;
            StepValue = stepValue;
            RoundDigits = roundDigits;
        }
    }
}