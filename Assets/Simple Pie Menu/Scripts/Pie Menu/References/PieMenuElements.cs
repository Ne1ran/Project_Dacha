using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Pie_Menu.Scripts.Pie_Menu.References
{
    public class PieMenuElements : MonoBehaviour
    {
        [SerializeField]
        private Transform menuItemsDir = null!;

        [SerializeField]
        private Transform hiddenMenuItemsDir = null!;

        [SerializeField]
        private Image background = null!;

        [SerializeField]
        private Transform infoPanel = null!;

        [SerializeField]
        private TextMeshProUGUI header = null!;

        [SerializeField]
        private TextMeshProUGUI details = null!;

        [SerializeField]
        private Animator animator = null!;

        [SerializeField]
        private AudioSource mouseClickAudioSource = null!;

        public Transform MenuItemsDir => menuItemsDir;

        public Transform HiddenMenuItemsDir => hiddenMenuItemsDir;

        public Image Background => background;

        public Transform InfoPanel => infoPanel;

        public TextMeshProUGUI Header => header;

        public TextMeshProUGUI Details => details;

        public Animator Animator => animator;

        public AudioSource MouseClickAudioSource => mouseClickAudioSource;
    }
}