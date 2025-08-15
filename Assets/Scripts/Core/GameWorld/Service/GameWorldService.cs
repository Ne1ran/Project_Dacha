using Core.Attributes;
using Core.GameWorld.Component;
using Game.Utils;
using UnityEngine;

namespace Core.GameWorld.Service
{
    [Service]
    public class GameWorldService
    {
        public GameWorldComponent GameWorld { get; private set; } = null!;
        
        public Transform MapObject { get; private set; } = null!;
        public Transform WorldObjects { get; private set; } = null!;
        
        public void Initialize()
        {
            GameObject gameWorld = new("GameWorld");
            GameWorldComponent gameWorldComponent = gameWorld.AddComponent<GameWorldComponent>();
            gameWorldComponent.transform.position = Vector3.zero;
            GameWorld = gameWorldComponent;
            
            MapObject = new GameObject("Map").transform;
            WorldObjects = new GameObject("Objects").transform;
            
            GameWorld.transform.AddChild(MapObject);
            GameWorld.transform.AddChild(WorldObjects);
        }
    }
}