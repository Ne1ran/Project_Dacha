using System.Collections.Generic;
using Game.Diseases.Model;
using Game.GameMap.Soil.Model;

namespace Game.Plants.Model
{
    public class PlantModel
    {
        public string PlantId { get; }
        public PlantGrowStage CurrentStage { get; set; }
        public SoilElementsModel TakenElements { get; set; }
        public float Health { get; set; }
        public float Immunity { get; set; }
        public float StageGrowth { get; set; }
        public List<DiseaseModel> DiseaseModels { get; set; }

        public PlantModel(string plantId, PlantGrowStage stage, float health, float immunity)
        {
            PlantId = plantId;
            CurrentStage = stage;
            Health = health;
            Immunity = immunity;
            TakenElements = new(0f, 0f, 0f);
            DiseaseModels = new();
        }
    }
}