using System.Collections.Generic;
using Game.Diseases.Model;
using Game.Stress.Model;

namespace Game.Plants.Model
{
    public class PlantInspectionModel
    {
        public string PlantId { get; }
        public float Health { get; }
        public float Immunity { get; }
        public float StageGrowth { get; }
        public PlantGrowStage CurrentStage { get; }
        public Dictionary<StressType, StressModel> Stress { get; }
        public List<DiseaseModel> Diseases { get; }

        public PlantInspectionModel(PlantModel plantModel)
        {
            PlantId = plantModel.PlantId;
            Health = plantModel.Health;
            Immunity = plantModel.Immunity;
            StageGrowth = plantModel.StageGrowth;
            CurrentStage = plantModel.CurrentStage;
            Stress = plantModel.Stress;
            Diseases = plantModel.DiseaseModels;
        }
    }
}