using System.Collections.Generic;
using Game.Fertilizers.Model;
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

        public SoilElementsModel Elements { get; set; }

        public List<SoilFertilizationModel> UsedFertilizers { get; set; } = new();

        public SoilModel(SoilType type,
                         float ph,
                         float salinity,
                         float breathability,
                         float humus,
                         float mass,
                         float waterAmount,
                         SoilElementsModel elements)
        {
            Type = type;
            Ph = ph;
            Salinity = salinity;
            Breathability = breathability;
            Humus = humus;
            Mass = mass;
            WaterAmount = waterAmount;
            Elements = elements;
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
    }
}