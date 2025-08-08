using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.GameMap.Map.Service
{
    [UsedImplicitly]
    public class MapService
    {
        private readonly TileService _tileService;
        private readonly WorldTileService _worldTileService;

        public MapService(TileService tileService, WorldTileService worldTileService)
        {
            _tileService = tileService;
            _worldTileService = worldTileService;
        }

        public async UniTask InitializeMapAsync()
        {
            List<SingleTileModel> tiles = _tileService.GetTiles();
            if (tiles.Count == 0) {
                List<Vector3> mapTilesPositions = CreateMapTilesPositions(Vector3.zero, 1f, 10, 10);
                tiles = _tileService.CreateTiles(mapTilesPositions);
            }

            await _worldTileService.CreateTilesInWorldAsync(tiles);
        }

        public List<Vector3> CreateMapTilesPositions(Vector3 centerPosition, float step, int length, int width)
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