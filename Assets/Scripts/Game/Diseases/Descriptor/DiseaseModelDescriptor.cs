using System;
using Game.Diseases.Model;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class DiseaseModelDescriptor
    {
        [field: SerializeField]
        public string Id { get; set; } = string.Empty;
        [field: SerializeField]
        public string Name { get; set; } = string.Empty;
        [field: SerializeField]
        public DiseaseType DiseaseType { get; set; }
        [field: SerializeField]
        public DiseaseInfectionModel InfectionModel { get; set; } = null!;
    }
}