using System;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Difficulty.Model;
using Game.Soil.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.GameMap.Map.Descriptor
{
    [CreateAssetMenu(fileName = "MapDescriptor", menuName = "Dacha/Descriptors/MapDescriptor")]
    [Descriptor("Descriptors/" + nameof(MapDescriptor))]
    [Serializable]
    public class MapDescriptor : Descriptor<DachaPlaceType, MapModelDescriptor>
    {
        
    }
}