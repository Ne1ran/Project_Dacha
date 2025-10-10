using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [CreateAssetMenu(fileName = "DiseasesDescriptor", menuName = "Dacha/Descriptors/DiseasesDescriptor")]
    [Descriptor("Descriptors/" + nameof(DiseasesDescriptor))]
    public class DiseasesDescriptor : Descriptor<string, DiseaseModelDescriptor>
    {
        
    }
}