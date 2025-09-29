using System.Collections.Generic;

namespace Game.Stress.Model
{
    public class StressModel
    {
        public float StressValue { get; set; }
        public List<string> StressSymptoms { get; set; } = new();

        public StressModel(float stressValue)
        {
            StressValue = stressValue;
        }
    }
}