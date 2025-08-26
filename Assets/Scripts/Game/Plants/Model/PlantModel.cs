using System.Collections.Generic;
using Game.Diseases.Model;
using Game.GameMap.Soil.Model;

namespace Game.Plants.Model
{
    public class PlantModel
    {
        public string PlantId { get; }
        public PlantFamilyType FamilyType { get; }
        public PlantGrowStage CurrentStage { get; set; }
        public ElementsModel TakenElements { get; set; }
        public float Health { get; set; }
        public float Immunity { get; set; }
        public float StageGrowth { get; set; }
        public List<DiseaseModel> DiseaseModels { get; set; }
        public bool InspectedToday { get; set; }

        public PlantModel(string plantId, PlantFamilyType familyType, PlantGrowStage stage, float health, float immunity)
        {
            PlantId = plantId;
            FamilyType = familyType;
            CurrentStage = stage;
            Health = health;
            Immunity = immunity;
            TakenElements = new(0f, 0f, 0f);
            DiseaseModels = new();
        }

        public void AddElements(ElementsModel elements)
        {
            TakenElements.Nitrogen = elements.Nitrogen;
            TakenElements.Potassium = elements.Potassium;
            TakenElements.Phosphorus = elements.Phosphorus;
        }

        public void DealDamage(float damage)
        {
            if (Immunity > damage) {
                Immunity -= damage;
                return;
            }

            float damageToHealth = damage - Immunity;
            Health -= damageToHealth;
            Immunity = 0;

            if (Health <= 0) {
                CurrentStage = PlantGrowStage.DEAD;
                // todo neiran throw event?
            }
        }
    }
}