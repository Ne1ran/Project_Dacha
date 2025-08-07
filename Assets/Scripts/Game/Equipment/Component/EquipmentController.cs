using Game.Utils;
using UnityEngine;

namespace Game.Equipment.Component
{
    public class EquipmentController : MonoBehaviour
    {
        private Transform _rightHand = null!;
        private Transform _leftHand = null!;

        public void Init(Transform rightHand, Transform leftHand)
        {
            _rightHand = rightHand;
            _leftHand = leftHand;
        }

        public void EquipItemRightHand(Transform item)
        {
            UnequipInHand(_rightHand);
            item.SetParent(_rightHand, false);
        }

        public void EquipItemLeftHand(Transform item)
        {
            UnequipInHand(_leftHand);
            item.SetParent(_leftHand, false);
        }

        public void ClearEquipment()
        {
            UnequipInHand(_rightHand);
            UnequipInHand(_leftHand);
        }

        private void UnequipInHand(Transform hand)
        {
            for (int i = 0; i < hand.childCount; i++) {
                hand.GetChild(i).DestroyObject();
            }
        }
    }
}