
using UnityEngine;

namespace Game.Interactable.PieMenu.Input_Devices
{
    public interface IInputDevice
    {
        Vector2 GetPosition(Vector2 anchoredPosition);

        bool IsSelectionButtonPressed();

        bool IsCloseButtonPressed();
    }
}
