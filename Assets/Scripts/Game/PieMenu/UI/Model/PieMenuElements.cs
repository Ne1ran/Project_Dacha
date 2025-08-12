using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.PieMenu.UI.Model
{
    public class PieMenuElements : MonoBehaviour
    {
        [FormerlySerializedAs("menuItemsDir"), SerializeField]
        private Transform _menuItemsDir = null!;

        [FormerlySerializedAs("hiddenMenuItemsDir"), SerializeField]
        private Transform _hiddenMenuItemsDir = null!;

        [FormerlySerializedAs("background"), SerializeField]
        private Image _background = null!;

        [FormerlySerializedAs("infoPanel"), SerializeField]
        private Transform _infoPanel = null!;

        [FormerlySerializedAs("header"), SerializeField]
        private TextMeshProUGUI _header = null!;

        [FormerlySerializedAs("details"), SerializeField]
        private TextMeshProUGUI _details = null!;

        [FormerlySerializedAs("animator"), SerializeField]
        private Animator _animator = null!;

        [FormerlySerializedAs("mouseClickAudioSource"), SerializeField]
        private AudioSource _mouseClickAudioSource = null!;

        public Transform MenuItemsDir => _menuItemsDir;

        public Transform HiddenMenuItemsDir => _hiddenMenuItemsDir;

        public Image Background => _background;

        public Transform InfoPanel => _infoPanel;

        public TextMeshProUGUI Header => _header;

        public TextMeshProUGUI Details => _details;

        public Animator Animator => _animator;

        public AudioSource MouseClickAudioSource => _mouseClickAudioSource;
    }
}