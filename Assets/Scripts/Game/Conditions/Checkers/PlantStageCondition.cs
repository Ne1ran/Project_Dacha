using System;
using Core.Common.Model;
using Core.Conditions.Checker;
using Core.Parameters;
using Game.Common.Handlers;
using Game.Plants.Model;
using Game.Plants.Service;
using VContainer;

namespace Game.Conditions.Checkers
{
    [Handler("PlantStageCondition")]
    public class PlantStageCondition : Condition
    {
        [Inject]
        private readonly PlantsService _plantsService = null!;
        
        private string _plantTileId = null!;
        private PlantGrowStage _growStage;
        private MathOperationType _operationType;

        protected override void Initialize(Parameters parameters)
        {
            _plantTileId = parameters.Require<string>(ParameterNames.TileId);
            _growStage = parameters.Get<PlantGrowStage>(ParameterNames.GrowStage);
            _operationType = parameters.Get<MathOperationType>(ParameterNames.Operation);
        }

        protected override ConditionResult CheckInternal()
        {
            PlantModel? plantModel = _plantsService.GetPlant(_plantTileId);
            if (plantModel == null) {
                return new(false, string.Empty);
            }

            return _operationType switch {
                    MathOperationType.Equal => new(_growStage == plantModel.CurrentStage, string.Empty),
                    MathOperationType.Lesser => new(_growStage < plantModel.CurrentStage, string.Empty),
                    MathOperationType.Greater => new(_growStage > plantModel.CurrentStage, string.Empty),
                    MathOperationType.LesserOrEqual => new(_growStage <= plantModel.CurrentStage, string.Empty),
                    MathOperationType.GreaterOrEqual => new(_growStage >= plantModel.CurrentStage, string.Empty),
                    _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}