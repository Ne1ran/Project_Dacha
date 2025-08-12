using System;
using System.Collections.Generic;
using Core.Resources.Service;
using Game.GameMap.Soil.Model;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Repo;
using JetBrains.Annotations;
using UnityEngine;
using VContainer.Unity;

namespace Game.GameMap.Tiles.Service
{
    [UsedImplicitly]
    public class TileService : IInitializable
    {
        private readonly TileRepo _tileRepo;

        public TileService(TileRepo tileRepo, IResourceService resourceService)
        {
            _tileRepo = tileRepo;
        }

        public void Initialize()
        {
            if (_tileRepo.Exists()) {
                return;
            }

            _tileRepo.Save(new());
        }

        public SingleTileModel CreateTile(Vector3 position)
        {
            TilesModel tilesModel = _tileRepo.Require();
            string tileGuid = Guid.NewGuid().ToString();
            SingleTileModel newTileModel = new(tileGuid, position);
            tilesModel.AddTile(newTileModel);
            _tileRepo.Save(tilesModel);
            return newTileModel;
        }

        public List<SingleTileModel> CreateTiles(List<Vector3> position)
        {
            TilesModel tilesModel = _tileRepo.Require();
            List<SingleTileModel> newTiles = new();

            foreach (Vector3 tilePosition in position) {
                string tileGuid = Guid.NewGuid().ToString();
                SingleTileModel newTileModel = new(tileGuid, tilePosition);
                newTiles.Add(newTileModel);
            }

            tilesModel.AddTileRange(newTiles);
            _tileRepo.Save(tilesModel);
            return newTiles;
        }

        public void UpdateSoil(string guid, SoilModel soilModel)
        {
            TilesModel tilesModel = _tileRepo.Require();
            SingleTileModel tileModel = tilesModel.Tiles.Find(tile => tile.Id == guid);
            if (tileModel == null) {
                Debug.LogWarning($"Tile not found with guid={guid}");
                return;
            }
            
            tileModel.Soil = soilModel;
            _tileRepo.Save(tilesModel);
        }

        public void RemoveTile(string guid)
        {
            TilesModel tilesModel = _tileRepo.Require();
            if (!tilesModel.TryRemoveTile(guid)) {
                Debug.LogWarning($"Tried to remove tile but couldn't found it. Guid={guid}");
            }
        }

        public List<SingleTileModel> GetTiles()
        {
            return _tileRepo.Require().Tiles;
        }
    }
}