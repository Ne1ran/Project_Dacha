using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Diseases.Descriptor
{
    [CreateAssetMenu(fileName = "DiseasesDescriptor", menuName = "Dacha/Descriptors/DiseasesDescriptor")]
    [Descriptor("Descriptors/" + nameof(DiseasesDescriptor))]
    public class DiseasesDescriptor : Descriptor<string, DiseaseModelDescriptor>
    {
        
    }
}