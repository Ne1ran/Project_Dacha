using Simple_Pie_Menu.Scripts;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Demos.Displaying_a_Pie_Menu.Old_Input_System
{
    public class DisplayWithOldInputSystemDemo : MonoBehaviour
    {
        [SerializeField] KeyCode displayButton;
        [SerializeField] PieMenu pieMenu;

        private PieMenuDisplayer displayer;

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            Debug.Log("This demo won't work if you are using only the New Input System." +
                " Please set 'Active Input Handling' to the Old Input Manager or use the 'Both' option.");         
#endif

            displayer = GetComponent<PieMenuDisplayer>();
        }

        private void Update()
        {
            Display();
        }

        private void Display()
        {
            if (Input.GetKeyDown(displayButton))
            {
                displayer.ShowPieMenu(pieMenu);
            }
        }
    }
}