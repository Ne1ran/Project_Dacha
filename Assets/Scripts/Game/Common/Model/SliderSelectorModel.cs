namespace Game.Common.Model
{
    public readonly struct SliderSelectorModel
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float StartValue { get; }
        public float StepsCount { get; }
        public int RoundDigits { get; }

        public SliderSelectorModel(float minValue, float maxValue, float startValue, float stepsCount, int roundDigits)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            StartValue = startValue;
            StepsCount = stepsCount;
            RoundDigits = roundDigits;
        }
    }
}