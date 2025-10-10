using System;
using System.Collections.Generic;
using Core.Parameters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Conditions.Checker
{
    [Serializable]
    public class ConditionParam
    {
        [field: SerializeField, ValueDropdown("GetAllKeys")]
        public string Key { get; set; } = string.Empty;
        [field: SerializeField]
        public string Value { get; set; } = string.Empty;

        public List<string> GetAllKeys()
        {
            return ParameterNames.ALL_PARAMETERS;
        }
    }
}