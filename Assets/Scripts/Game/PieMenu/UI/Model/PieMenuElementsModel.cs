using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PieMenu.UI.Model
{
    public class PieMenuElementsModel : MonoBehaviour
    {
        [SerializeField]
        private Transform _menuItemsDir = null!;

        [SerializeField]
        private Image _background = null!;

        [SerializeField]
        private Transform _infoPanel = null!;

        [SerializeField]
        private TextMeshProUGUI _header = null!;

        [SerializeField]
        private TextMeshProUGUI _details = null!;

        [SerializeField]
        private Animator _animator = null!;

        [SerializeField]
        private AudioSource _mouseClickAudioSource = null!;

        public Transform MenuItemsDir => _menuItemsDir;

        public Image Background => _background;

        public Transform InfoPanel => _infoPanel;

        public TextMeshProUGUI Header => _header;

        public TextMeshProUGUI Details => _details;

        public Animator Animator => _animator;

        public AudioSource MouseClickAudioSource => _mouseClickAudioSource;
    }
}