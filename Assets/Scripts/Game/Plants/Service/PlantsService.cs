using System;
using Core.Attributes;
using Game.Plants.Repo;
using UnityEngine;

namespace Game.Plants.Service
{
    [Service]
    public class PlantsService : IDisposable
    {
        private readonly PlantsRepo _plantsRepo;

        public PlantsService(PlantsRepo plantsRepo)
        {
            _plantsRepo = plantsRepo;
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public void CreatePlant(string seedId, string tileId)
        {
            Debug.LogWarning($"Sowing seed={seedId} in tile={tileId}");
        }
    }
}