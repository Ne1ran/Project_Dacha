using UnityEngine;

namespace Game.Tools.Component
{
    public class ToolController : MonoBehaviour
    {
        public string GetName => gameObject.name;
    }
}