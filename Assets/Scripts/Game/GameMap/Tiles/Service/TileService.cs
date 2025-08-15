using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Repo;
using Game.TimeMove.Event;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.GameMap.Tiles.Service
{
    [Service]
    public class TileService : IInitializable, IDisposable
    {
        private readonly TileRepo _tileRepo;
        private readonly IDescriptorService _descriptorService;
        private readonly SoilService _soilService;

        private IDisposable? _disposable;

        public TileService(TileRepo tileRepo,
                           IDescriptorService descriptorService,
                           SoilService soilService,
                           ISubscriber<string, DayChangedEvent> dayFinishedSubscriber)
        {
            _tileRepo = tileRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;

            DisposableBagBuilder bagBuilder = DisposableBag.CreateBuilder();
            bagBuilder.Add(dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_FINISHED, OnDayFinished));
            bagBuilder.Add(dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_STARTED, OnDayStarted));
            _disposable = bagBuilder.Build();
        }

        public void Initialize()
        {
            if (_tileRepo.Exists()) {
                return;
            }

            _tileRepo.Save(new());
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public SingleTileModel CreateTile(Vector3 position)
        {
            TilesModel tilesModel = _tileRepo.Require();
            string tileGuid = Guid.NewGuid().ToString();
            SoilType mapSoilType = _descriptorService.Require<MapDescriptor>().SoilType;
            SoilModel soilModel = _soilService.CreateSoil(mapSoilType);
            SingleTileModel newTileModel = new(tileGuid, position, soilModel);
            tilesModel.AddTile(newTileModel);
            _tileRepo.Save(tilesModel);
            return newTileModel;
        }

        public List<SingleTileModel> CreateTiles(List<Vector3> position)
        {
            List<SingleTileModel> newTiles = new();

            TilesModel tilesModel = _tileRepo.Require();
            SoilType mapSoilType = _descriptorService.Require<MapDescriptor>().SoilType;

            foreach (Vector3 tilePosition in position) {
                string tileGuid = Guid.NewGuid().ToString();
                SoilModel soilModel = _soilService.CreateSoil(mapSoilType);
                SingleTileModel newTileModel = new(tileGuid, tilePosition, soilModel);
                newTiles.Add(newTileModel);
            }

            tilesModel.AddTileRange(newTiles);
            _tileRepo.Save(tilesModel);
            return newTiles;
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            TilesModel tilesModel = _tileRepo.Require();

            foreach (SingleTileModel singleTileModel in tilesModel.Tiles) {
                foreach (SoilFertilizationModel usedFertilizer in singleTileModel.Soil.UsedFertilizers) {
                    usedFertilizer.CurrentDecomposeDay += 1;
                }
            }

            _tileRepo.Save(tilesModel);
        }

        private void OnDayStarted(DayChangedEvent evt)
        {
            ActivateFertilizers();
        }

        private void ActivateFertilizers()
        {
            TilesModel tilesModel = _tileRepo.Require();

            foreach (SingleTileModel singleTileModel in tilesModel.Tiles) {
                SoilModel soilModel = singleTileModel.Soil;
                _soilService.ActivateUsedFertilizers(soilModel);
            }

            _tileRepo.Save(tilesModel);
        }

        public void AddFertilizer(string tileId, string fertilizerId, float portionMassGramms)
        {
            TilesModel tilesModel = _tileRepo.Require();
            SingleTileModel tileModel = RequireTileModel(tileId, tilesModel);
            _soilService.AddFertilizer(tileModel.Soil, fertilizerId, portionMassGramms / 1000f);
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