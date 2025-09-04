using System.Collections.Generic;
using Game.Diseases.Model;
using Game.Fertilizers.Model;
using Game.Plants.Model;
using UnityEngine;

namespace Game.GameMap.Soil.Model
{
    public class SoilModel
    {
        public SoilType Type { get; set; }
        public float Ph { get; set; }
        public float Salinity { get; set; }
        public float Breathability { get; set; }
        public float Humus { get; set; }
        public float Mass { get; set; }
        public float WaterAmount { get; set; }
        public SoilState State { get; set; }
        public ElementsModel Elements { get; set; }
        public List<SavedDiseaseModel> SavedDiseases { get; set; } = new();
        public List<SoilFertilizationModel> UsedFertilizers { get; set; } = new();
        public Dictionary<int, PlantFamilyType> CropRotations { get; set; } = new();
        public bool DugRecently { get; set; } = false;

        public SoilModel(SoilType type,
                         float ph,
                         float salinity,
                         float breathability,
                         float humus,
                         float mass,
                         float waterAmount,
                         ElementsModel elements)
        {
            Type = type;
            Ph = ph;
            Salinity = salinity;
            Breathability = breathability;
            Humus = humus;
            Mass = mass;
            WaterAmount = waterAmount;
            Elements = elements;
            State = SoilState.None;
        }

        public void ApplyFertilizer(FertilizerSoilModel model)
        {
            Mass += model.Mass;
            Salinity += model.Mass / Mass;
            Ph += model.PhChange;
            Breathability += model.BreathabilityChange;
            Humus += model.HumusMass;
            Elements.Nitrogen += model.NitrogenMass;
            Elements.Potassium += model.PotassiumMass;
            Elements.Phosphorus += model.PhosphorusMass;
            Debug.LogWarning($"Fertilizer used. New soil data = {Mass}, {Ph}, {Salinity}, {Breathability}, {Humus}, {Elements.Nitrogen}, {Elements.Potassium}, {Elements.Phosphorus}");
        }

        public bool TryConsume(float water, ElementsModel elements)
        {
            if (WaterAmount < water) {
                return false;
            }

            WaterAmount -= water;

            if (Elements.HasEnoughElements(elements)) {
                Elements.Subtract(elements);
                return true;
            }

            float totalMass = elements.TotalMass();
            if (Humus > totalMass) {
                Humus -= totalMass;
                return true;
            }

            return false;
        }

        public bool TryConsumeHumus(float humus)
        {
            if (Humus < humus) {
                return false;
            }

            Humus -= humus;
            return true;
        }

        public bool WellWatered => WaterAmount > 1f && SoilHumidity > 50f;
        public float SoilHumidity => WaterAmount * Breathability / 100f;
    } 
}