using System;
using UnityEngine;

namespace Core.Conditions.Checker
{
    [Serializable]
    public class ConditionParam
    {
        [field: SerializeField]
        public string Key { get; set; } = string.Empty;
        [field: SerializeField]
        public string Value { get; set; } = string.Empty;
    }
}