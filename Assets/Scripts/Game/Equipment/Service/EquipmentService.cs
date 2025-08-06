using Core.Attributes;
using UnityEngine;

namespace Game.Equipment.Service
{
    [UsedImplicitly]
    public class EquipmentService
    {
        private string? _equippedTool; // todo neiran redo to repo
        
        public bool TryEquip(string toolId)
        {
            if (_equippedTool == toolId) {
                Debug.Log($"Unequipped tool={toolId}");
                _equippedTool = null;
                return false;
            }
            
            _equippedTool = toolId;
            Debug.Log($"Equipped tool={toolId}");
            return true;
        }
    }
}