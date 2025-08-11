using Simple_Pie_Menu.Scripts;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using SimplePieMenu;
using UnityEngine;

namespace Simple_Pie_Menu.Demos.Displaying_a_Pie_Menu.New_Input_System
{
    public class DisplayWithNewInputSystemDemo : MonoBehaviour
    {
        [SerializeField] PieMenu pieMenu;

        private PieMenuDisplayer displayer;
        private PieMenuDemoControls demoControls;

        private void Awake()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("This demo won't work if you are using only the Old Input System." +
                " Please set 'Active Input Handling' to the New Input System or use the 'Both' option."); 
#endif

            displayer = GetComponent<PieMenuDisplayer>();

            demoControls = new PieMenuDemoControls();
            demoControls.Enable();
        }

        private void Update()
        {
            Display();
        }

        private void OnDisable()
        {
            demoControls.Disable();
        }

        private void Display()
        {
            if(demoControls.PieMenu.Display.WasPressedThisFrame())
                displayer.ShowPieMenu(pieMenu);
        }
    }
}

