
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection.Input_Devices
{
    public interface IInputDevice
    {
        Vector2 GetPosition(Vector2 anchoredPosition);

        bool IsSelectionButtonPressed();

        bool IsCloseButtonPressed();
    }
}
