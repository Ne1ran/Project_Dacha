using System;
using System.Collections.Generic;
using Core.Conditions.Checker;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Conditions.Descriptor
{
    [Serializable]
    public class ConditionDescriptor
    {
        [field: SerializeField]
        public string Id { get; set; } = string.Empty;
        [field: SerializeField]
        public bool Inverted { get; set; }
        [field: SerializeField]
        [TableList]
        public List<ConditionParam> Params { get; set; } = new();

        public Parameters.Parameters GetParameters()
        {
            Dictionary<string, object> parameters = new();

            foreach (ConditionParam conditionParam in Params) {
                parameters.Add(conditionParam.Key, conditionParam.Value);
            }

            return new(parameters);
        }
    }
}