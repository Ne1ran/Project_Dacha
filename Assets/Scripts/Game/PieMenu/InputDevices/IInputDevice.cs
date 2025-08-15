
using UnityEngine;

namespace Game.PieMenu.InputDevices
{
    public interface IInputDevice
    {
        Vector2 GetPosition(Vector2 anchoredPosition);

        bool IsSelectionButtonPressed();

        bool IsCloseButtonPressed();
        bool ScrollingBackwards();

        bool ScrollingForward();
    }
}
