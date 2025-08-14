using System;
using System.Collections.Generic;
using Core.Attributes;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Fertilizers.Descriptor
{
    [CreateAssetMenu(fileName = "FertilizersDescriptor", menuName = "Dacha/Descriptors/FertilizersDescriptor")]
    [Descriptor("Descriptors/" + nameof(FertilizersDescriptor))]
    public class FertilizersDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<FertilizerDescriptorModel> Fertilizers { get; private set; } = new();

        private void OnValidate()
        {
            foreach (FertilizerDescriptorModel fert in Fertilizers) {
                string newName = fert.Name.Trim();
                string[] nameSplitted = newName.Split(" ");

                string result = "";
                for (int i = 0; i < nameSplitted.Length; i++) {
                    string part = nameSplitted[i];
                    if (i > 0) {
                        result += part.ToUpperFirst();
                    } else {
                        result += part;
                    }
                }

                fert.Id = result;
            }
        }
    }
}