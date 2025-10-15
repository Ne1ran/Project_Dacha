using System;
using UnityEngine;

namespace Game.Common.Descriptors
{
    [Serializable]
    public class SelectorDescriptorModel
    {
        [field: SerializeField]
        public float MinValue { get; set; }
        [field: SerializeField]
        public float MaxValue { get; set; }
        [field: SerializeField]
        public float StartValue { get; set; }
        [field: SerializeField]
        public float StepValue { get; set; }
        [field: SerializeField]
        public int RoundDigits { get; set; }
    }
}