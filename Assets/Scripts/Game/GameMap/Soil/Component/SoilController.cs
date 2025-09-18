using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.Utils;
using UnityEngine;
using VContainer;

namespace Game.GameMap.Soil.Component
{
    [NeedBinding("Prefabs/Soil/pfSoilController")]
    public class SoilController : MonoBehaviour
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        [Inject]
        private readonly IResourceService _resourceService = null!;

        private Transform _skinHolder = null!;

        private SoilVisualDescriptor _visualDescriptor = null!;
        private SoilModel? _soilModel;
        private string _soilId = null!;
        private string _currentSkinPath = null!;

        private void Awake()
        {
            _skinHolder = this.RequireComponentInChild<Transform>("Skin");
        }

        public async UniTask InitializeAsync(string soilId, SoilVisualDescriptor visualDescriptor)
        {
            _visualDescriptor = visualDescriptor;
            _soilId = soilId;

            _soilModel = _soilService.GetSoil(_soilId);
            _currentSkinPath = _soilModel == null
                                       ? _visualDescriptor.BaseViewPrefab
                                       : _visualDescriptor.GetPrefabPath(_soilModel.State, _soilModel.DugRecently, _soilModel.WellWatered);
            Transform skin = await _resourceService.LoadObjectAsync<Transform>(_currentSkinPath);
            skin.SetParent(_skinHolder, false);
        }

        public async UniTask UpdateSkinAsync()
        {
            SoilModel? newSoilModel = _soilService.GetSoil(_soilId);
            if (newSoilModel == null) {
                return;
            }
            
            string newSkinPath = _visualDescriptor.GetPrefabPath(newSoilModel.State, newSoilModel.DugRecently, newSoilModel.WellWatered);
            if (newSkinPath == _currentSkinPath) {
                return;
            }

            _currentSkinPath = newSkinPath;
            for (int i = 0; i < _skinHolder.childCount; i++) {
                Transform child = _skinHolder.GetChild(i);
                child.DestroyObject();
            }

            Transform skin = await _resourceService.LoadObjectAsync<Transform>(_currentSkinPath);
            skin.SetParent(_skinHolder, false);
            _soilModel = newSoilModel;
        }
    }
}