using System.Collections.Generic;
using Game.PieMenu.UI;
using UnityEngine;

namespace Game.PieMenu.Utils
{
    public static class PieMenuUtils
    {
        public const string InputDevice = "InputDevice";
        public const int CircleDegrees = 360;
        public const float CircleDegreesF = 360f;

        public static int CalculateItemAngle(int menuItemCount, int menuItemSpacing)
        {
            if (menuItemCount == 0) {
                Debug.Log("Can't divide by zero! Ensure menu items != 0");
                return 0;
            }
            

            int totalSpacing = CalculateTotalSpacing(menuItemCount, menuItemSpacing);

            int totalMenuItemsAngle = CircleDegrees - totalSpacing;
            int menuItemAngle = totalMenuItemsAngle / menuItemCount;

            if (menuItemAngle < 0) {
                menuItemAngle = 0;
            }

            return menuItemAngle;
        }
        
        public static int CalculateTotalSpacing(int menuItemCount, int menuItemSpacing)
        {
            return menuItemCount * menuItemSpacing;
        }

        public static float CalculateTotalSpacingPercentage(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacing = CalculateTotalSpacing(menuItemCount, menuItemSpacing);
            float totalSpacingToPercentage = totalSpacing / CircleDegrees;
            return totalSpacingToPercentage;
        }
        
        public static int CalculateNewRotation(int menuItemCount, int menuItemSpacing)
        {
            // The following method calculates a new rotation to ensure symmetry based on the provided number of Menu Items
            int rotation;
            int circleDegrees = 360;
            if (menuItemCount % 2 != 0) {
                rotation = circleDegrees - 90 / menuItemCount - menuItemSpacing / 2;
            } else {
                rotation = circleDegrees - menuItemSpacing / 2;
            }

            return rotation;
        }

        public static float CalculatePieMenuScale(int initialSize, int size)
        {
            return (float) size / initialSize;
        }

        // public static void RotateFirstElement(Transform menuItem, Quaternion iconDirRotation)
        // {
        //     Quaternion firstIconRotation = Quaternion.Euler(0f, 0f, Mathf.Abs(iconDirRotation.z));
        //     Transform firstIconDir = menuItem.GetChild(0).transform;
        //     Transform firstIcon = firstIconDir.GetChild(0).transform;
        //     firstIcon.rotation = firstIconRotation;
        // }
        //
        // public static void RotateOtherElements(Dictionary<int, PieMenuItemController> menuItems, Quaternion iconDirRotation)
        // {
        //     foreach (KeyValuePair<int, PieMenuItemController> menuItem in menuItems) {
        //         float menuItemRotationZ = menuItem.Value.transform.rotation.z;
        //         float iconRotationZ = -(menuItemRotationZ - iconDirRotation.z);
        //         Transform iconDir = menuItem.Value.transform.GetChild(0).transform;
        //         Transform icon = iconDir.GetChild(0).transform;
        //         icon.rotation = Quaternion.Euler(0f, 0f, iconRotationZ);
        //     }
        // }
    }
}