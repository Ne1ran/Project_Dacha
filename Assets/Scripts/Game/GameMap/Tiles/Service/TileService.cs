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

        public SingleTileModel CreateTile(Vector3 position, int id)
        {
            TilesModel tilesModel = _tileRepo.Require(); ;
            SingleTileModel newTileModel = new(id.ToString(), position);
            tilesModel.AddTile(newTileModel);
            _tileRepo.Save(tilesModel);
            return newTileModel;
        }

        public List<SingleTileModel> CreateTiles(List<Vector3> position)
        {
            List<SingleTileModel> newTiles = new();

            TilesModel tilesModel = _tileRepo.Require();

            for (int i = 0; i < position.Count; i++) {
                Vector3 tilePosition = position[i];
                string id = i.ToString();
                SingleTileModel newTileModel = new(id, tilePosition);
                newTiles.Add(newTileModel);
            }

            tilesModel.AddTileRange(newTiles);
            _tileRepo.Save(tilesModel);
            return newTiles;
        }

        public void RemoveTile(string id)
        {
            TilesModel tilesModel = _tileRepo.Require();
            if (!tilesModel.TryRemoveTile(id)) {
                Debug.LogWarning($"Tried to remove tile but couldn't found it. id={id}");
            }
        }

        public List<SingleTileModel> GetTiles()
        {
            return _tileRepo.Require().Tiles;
        }

        private SingleTileModel RequireTileModel(string id, TilesModel tilesModel)
        {
            SingleTileModel tileModel = tilesModel.Tiles.Find(tile => tile.Id == id);
            if (tileModel == null) {
                throw new KeyNotFoundException($"No tile found for id={id}");
            }

            return tileModel;
        }
    }
}