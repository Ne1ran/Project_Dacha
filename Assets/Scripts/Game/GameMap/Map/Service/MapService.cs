using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Tiles.Component;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using UnityEngine;

namespace Game.GameMap.Map.Service
{
    [Service]
    public class MapService
    {
        private readonly TileService _tileService;
        private readonly WorldTileService _worldTileService;
        private readonly IDescriptorService _descriptorService;

        public MapService(TileService tileService, WorldTileService worldTileService, IDescriptorService descriptorService)
        {
            _tileService = tileService;
            _worldTileService = worldTileService;
            _descriptorService = descriptorService;
        }

        public async UniTask InitializeMapAsync()
        {
            MapDescriptor mapDescriptor = _descriptorService.Require<MapDescriptor>();

            List<SingleTileModel> tiles = _tileService.GetTiles();
            if (tiles.Count == 0) {
                List<Vector3> mapTilesPositions = CreateMapTilesPositions(mapDescriptor.TileMainPoint, mapDescriptor.TileLength, mapDescriptor.Length, mapDescriptor.Width);
                tiles = _tileService.CreateTiles(mapTilesPositions);
            }

            await _worldTileService.CreateTilesInWorldAsync(tiles);
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