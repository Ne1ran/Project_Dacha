using System;
using UnityEngine;

namespace Core.Parameters
{
    [Serializable]
    public class ParametersPair
    {
        [field: SerializeField]
        public string ParamName { get; set; } = string.Empty;
        [field: SerializeField]
        public string ParamValue { get; set; } = string.Empty;
    }
}