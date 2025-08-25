using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [CreateAssetMenu(fileName = "DiseasesDescriptor", menuName = "Dacha/Descriptors/DiseasesDescriptor")]
    [Descriptor("Descriptors/" + nameof(DiseasesDescriptor))]
    public class DiseasesDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<DiseaseModelDescriptor> Items { get; private set; } = new();
    }
}