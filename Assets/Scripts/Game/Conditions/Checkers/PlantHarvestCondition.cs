using Core.Common.Model;
using Core.Conditions.Checker;
using Core.Parameters;
using Game.Common.Handlers;
using Game.Plants.Model;
using Game.Plants.Service;
using VContainer;

namespace Game.Conditions.Checkers
{
    [Handler("PlantHarvestCondition")]
    public class PlantHarvestCondition : Condition
    {
        [Inject]
        private readonly PlantsService _plantsService = null!;

        private string _plantTileId = null!;
        private int _harvestCount = 0;
        private MathOperationType _operationType;

        protected override void Initialize(Parameters parameters)
        {
            _plantTileId = parameters.Require<string>(ParameterNames.TileId);
            _harvestCount = parameters.Require<int>(ParameterNames.HarvestCount);
        }

        protected override ConditionResult CheckInternal()
        {
            PlantModel? plantModel = _plantsService.GetPlant(_plantTileId);
            if (plantModel == null) {
                return new(false, string.Empty);
            }

            bool check = plantModel.Harvest.Count >= _harvestCount;
            return new(check, string.Empty);
        }
    }
}