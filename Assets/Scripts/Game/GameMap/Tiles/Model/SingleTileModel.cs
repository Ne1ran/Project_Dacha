using Core.Common.Model;
using Game.GameMap.Soil.Model;
using UnityEngine;

namespace Game.GameMap.Tiles.Model
{
    public class SingleTileModel
    {
        public string Id { get; }
        public SimpleVector3 Position { get; }
        public SoilModel? Soil { get; set; }

        public SingleTileModel(string id, Vector3 position, SoilModel? soil = null)
        {
            Id = id;
            Position = new(position.x, position.y, position.z);
            Soil = soil;
        }
    }
}