using System.Collections.Generic;
using Core.Attributes;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using Game.Plants.Service;
using Game.Soil.Service;

namespace Game.Testing.Service
{
    [Service]
    public class TestGameService
    {
        private readonly TileService _tileService;
        private readonly SoilService _soilService;
        private readonly PlantsService _plantsService;

        public TestGameService(SoilService soilService, TileService tileService, PlantsService plantsService)
        {
            _soilService = soilService;
            _tileService = tileService;
            _plantsService = plantsService;
        }

        public void TiltAll()
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _soilService.TryTiltSoil(tileId);
            }
        }

        public void RemoveAllPlants()
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _plantsService.RemovePlant(tileId);
            }
        }

        public void PlantAll(string plantId, float startHealth, float startImmunity)
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                if (_soilService.TrySowSeed(tileId)) {
                    _plantsService.CreatePlant(plantId, tileId, startHealth, startImmunity);
                }
            }
        }

        public void ShovelAll()
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _soilService.TryShovelSoil(tileId);
            }
        }

        public void WaterAll(float waterAmount)
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _soilService.WaterSoil(tileId, waterAmount);
            }
        }

        public void UseFertilizer(string fertilizerId, float amount)
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _soilService.AddFertilizer(tileId, fertilizerId, amount);
            }
        }

        public void SetWaterAmount(float waterAmount)
        {
            List<SingleTileModel> allTiles = _tileService.GetTiles();
            foreach (SingleTileModel tileModel in allTiles) {
                string tileId = tileModel.Id;
                _soilService.SetSoilWater(tileId, waterAmount);
            }
        }
    }
}