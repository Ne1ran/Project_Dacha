using System;
using System.Collections.Generic;
using Core.Attributes;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Repo;
using UnityEngine;
using VContainer.Unity;

namespace Game.GameMap.Tiles.Service
{
    [Service]
    public class TileService : IInitializable
    {
        private readonly TileRepo _tileRepo;

        public TileService(TileRepo tileRepo)
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
            List<SingleTileModel> newTiles = new();

            TilesModel tilesModel = _tileRepo.Require();

            foreach (Vector3 tilePosition in position) {
                string tileGuid = Guid.NewGuid().ToString();
                SingleTileModel newTileModel = new(tileGuid, tilePosition);
                newTiles.Add(newTileModel);
            }

            tilesModel.AddTileRange(newTiles);
            _tileRepo.Save(tilesModel);
            return newTiles;
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

        private SingleTileModel RequireTileModel(string guid, TilesModel tilesModel)
        {
            SingleTileModel tileModel = tilesModel.Tiles.Find(tile => tile.Id == guid);
            if (tileModel == null) {
                throw new KeyNotFoundException($"No tile found for guid={guid}");
            }

            return tileModel;
        }
    }
}