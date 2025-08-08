using Game.Soil.Model;
using UnityEngine;

namespace Game.Tiles.Model
{
    public class SingleTileModel
    {
        public string Id { get; }
        public SoilModel? Soil { get; }
        public Vector3 Position { get; }

        public SingleTileModel(string id, SoilModel? soil, Vector3 position)
        {
            Id = id;
            Soil = soil;
            Position = position;
        }
    }
}