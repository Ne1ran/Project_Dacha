using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class LayerUtils
    {
        private const string Static = "Static";
        
        public static Dictionary<string, int> LayerMasks { get; } = new() {
                { "static", LayerMask.GetMask(Static) }
        };
        
        public static int StaticLayer => LayerMask.NameToLayer(Static);
        
        public static string GetLayerName(int layer)
        {
            return LayerMask.LayerToName(layer);
        }
        
        /// <summary>
        /// Проверяет, что указанный слой подходит под маску слоев.
        /// </summary>
        /// <param name="layerMask">Маска слоев.</param>
        /// <param name="layerIndex">Индекс слоя из коллекции слоев проекта.</param>
        public static bool ContainsLayer(int layerMask, int layerIndex)
        {
            int checkingMask = 1 << layerIndex;
            return (layerMask & checkingMask) != 0;
        }
    }
}