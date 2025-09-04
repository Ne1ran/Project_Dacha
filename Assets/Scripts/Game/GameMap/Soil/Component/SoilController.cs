using Core.Resources.Binding.Attributes;
using Game.Utils;
using UnityEngine;

namespace Game.GameMap.Soil.Component
{
    [PrefabPath("Prefabs/Soil/pfSoilController")]
    public class SoilController : MonoBehaviour
    {
        private Transform _skinHolder = null!;
        
        private void Awake()
        {
            _skinHolder = this.RequireComponentInChild<Transform>("Skin");
        }

        public void Initialize(Transform skin)
        {
            skin.SetParent(_skinHolder);
        }
    }
}