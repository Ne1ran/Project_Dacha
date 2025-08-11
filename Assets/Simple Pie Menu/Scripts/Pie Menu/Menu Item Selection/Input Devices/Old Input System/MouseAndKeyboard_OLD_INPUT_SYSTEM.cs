using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection.Input_Devices.Old_Input_System
{
    public class MouseAndKeyboard_OLD_INPUT_SYSTEM : MonoBehaviour, IInputDevice
    {
        public Vector2 GetPosition(Vector2 anchoredPosition)
        {
            Vector2 mouseInput;

            mouseInput.x = Input.mousePosition.x - (Screen.width / 2f) - anchoredPosition.x;
            mouseInput.y = Input.mousePosition.y - (Screen.height / 2f) - anchoredPosition.y;

            return mouseInput;
        }

        public bool IsSelectionButtonPressed()
        {
            return Input.GetKeyDown(KeyCode.Mouse0);
        }

        public bool IsCloseButtonPressed()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
}
