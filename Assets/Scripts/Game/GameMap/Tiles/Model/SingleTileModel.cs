using Core.Common.Model;
using UnityEngine;

namespace Game.GameMap.Tiles.Model
{
    public class SingleTileModel
    {
        public string Id { get; }
        public SimpleVector3 Position { get; }
        public SingleTileModel(string id, Vector3 position)
        {
            Id = id;
            Position = new(position.x, position.y, position.z);
        }
    }
}