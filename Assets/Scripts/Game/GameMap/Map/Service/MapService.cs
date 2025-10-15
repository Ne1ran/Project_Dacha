using System.Collections.Generic;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using Game.Difficulty.Model;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using Game.Plants.Service;
using Game.Soil.Service;
using UnityEngine;

namespace Game.GameMap.Map.Service
{
    [Service]
    public class MapService
    {
        private readonly TileService _tileService;
        private readonly WorldTileService _worldTileService;
        private readonly WorldSoilService _worldSoilService;
        private readonly WorldPlantsService _worldPlantsService;
        private readonly MapDescriptor _mapDescriptor;

        public MapService(TileService tileService,
                          WorldTileService worldTileService,
                          WorldSoilService worldSoilService,
                          WorldPlantsService worldPlantsService,
                          MapDescriptor mapDescriptor)
        {
            _tileService = tileService;
            _worldTileService = worldTileService;
            _worldSoilService = worldSoilService;
            _worldPlantsService = worldPlantsService;
            _mapDescriptor = mapDescriptor;
        }

        public async UniTask InitializeMapAsync()
        {
            MapModelDescriptor mapModelDescriptor = _mapDescriptor.Require(DachaPlaceType.Middle);
            List<SingleTileModel> tiles = _tileService.GetTiles();
            if (tiles.Count == 0) {
                List<Vector3> mapTilesPositions = CreateMapTilesPositions(mapModelDescriptor.TileMainPoint, mapModelDescriptor.TileLength,
                                                                          mapModelDescriptor.Length, mapModelDescriptor.Width);
                tiles = _tileService.CreateTiles(mapTilesPositions);
            }

            await _worldTileService.CreateTilesInWorldAsync(tiles);
            // never change sides. subscriptions won't work.
            await _worldSoilService.CreateSoilControllers(tiles);
            await _worldPlantsService.CreatePlants(tiles);
        }

        private List<Vector3> CreateMapTilesPositions(Vector3 centerPosition, float step, int length, int width)
        {
            List<Vector3> result = new();

            for (int i = 0; i < length; i++) {
                for (int j = 0; j < width; j++) {
                    result.Add(new(centerPosition.x + i * step, centerPosition.y, centerPosition.z + j * step));
                }
            }

            return result;
        }
    }
}