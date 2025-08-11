using Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.Icon_Settings_Handler;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers.General_Settings_Handler
{
    public class PieMenuRotationHandler : MonoBehaviour
    {
        public void Handle(PieMenu pieMenu, MenuItemIconsSettingsHandler iconsSettingsHandler, int rotation)
        {
            ChangeRotation(pieMenu, rotation);
            ApplyRotationForIcons(pieMenu, iconsSettingsHandler, rotation);
        }


        private void ChangeRotation(PieMenu pieMenu, int rotation)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            Quaternion newRotation = Quaternion.Euler(0f, 0f, (float)rotation);
            menuItemsDir.rotation = newRotation;
            pieMenu.PieMenuInfo.SetRotation(rotation);
        }

        private void ApplyRotationForIcons(PieMenu pieMenu, MenuItemIconsSettingsHandler iconsSettingsHandler,
            int rotation)
        {
            bool iconsEnabled = pieMenu.PieMenuInfo.IconsEnabled;

            if (iconsEnabled)
            {
                IconEnableHandler iconEnableHandler = iconsSettingsHandler.EnableHandler;

                Quaternion iconDirRotation = iconEnableHandler.CalculateIconDirRotation(pieMenu, iconsEnabled);
                Quaternion additionalRotation = Quaternion.Euler(0f, 0f, (float)rotation);

                iconDirRotation *= additionalRotation;

                iconEnableHandler.IconRotator.Rotate(pieMenu.GetMenuItems(), iconDirRotation);
            }
        }
    }
}
