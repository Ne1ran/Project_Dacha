using System;
using System.Collections.Generic;

namespace Game.Diseases.Model
{
    public class DiseaseModel
    {
        public string Id { get; }
        public int Stage { get; set; }
        public float CurrentGrowth { get; set; }
        public List<string> KnownSymptoms { get; set; } = new();

        public DiseaseModel(string id, int stage, float currentGrowth)
        {
            Id = id;
            Stage = stage;
            CurrentGrowth = currentGrowth;
        }

        public void AddSymptoms(List<string> symptoms)
        {
            foreach (string symptom in symptoms) {
                if (KnownSymptoms.Contains(symptom)) {
                    continue;
                }
                
                KnownSymptoms.Add(symptom);
            }
        }
    }
}