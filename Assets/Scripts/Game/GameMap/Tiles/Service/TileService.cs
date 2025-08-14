using System;
using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
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
        private readonly SoilService _soilService;
        private readonly IDescriptorService _descriptorService;

        public TileService(TileRepo tileRepo, SoilService soilService, IDescriptorService descriptorService)
        {
            _tileRepo = tileRepo;
            _soilService = soilService;
            _descriptorService = descriptorService;
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
        
        public void ChangeTileSoil(string tileGuid)
        {
            TilesModel tilesModel = _tileRepo.Require();
            SingleTileModel tileModel = RequireTileModel(tileGuid, tilesModel);
            
            if (tileModel.Soil == null) {
                SoilType mapSoilType = _descriptorService.Require<MapDescriptor>().SoilType;
                SoilModel soilModel = _soilService.CreateSoil(mapSoilType);
                tileModel.Soil = soilModel;
            }
            
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