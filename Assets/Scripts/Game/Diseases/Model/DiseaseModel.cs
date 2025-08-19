using System.Collections.Generic;

namespace Game.Diseases.Model
{
    public class DiseaseModel
    {
        public string Id { get; }
        public int Stage { get; set; }
        public float CurrentGrowth { get; set; }
        public List<string> CurrentSymptoms { get; set; } = new();

        public DiseaseModel(string id, int stage, float currentGrowth)
        {
            Id = id;
            Stage = stage;
            CurrentGrowth = currentGrowth;
        }
    }
}