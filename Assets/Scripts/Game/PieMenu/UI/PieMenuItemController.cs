using Core.Reactive;
using Core.Resources.Binding.Attributes;
using Game.PieMenu.Model;
using Game.PieMenu.Settings;
using Game.PieMenu.UI.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PieMenu.UI
{
    [PrefabPath("UI/Dialogs/PlayMode/pfPieMenuItem")]
    public class PieMenuItemController : MonoBehaviour
    {
        private static readonly int _mouseExit = Animator.StringToHash(MouseExitTrigger);
        private static readonly int _mouseEnter = Animator.StringToHash(MouseEnterTrigger);
        private const string MouseEnterTrigger = "MouseEnter";
        private const string MouseExitTrigger = "MouseExit";

        [ComponentBinding]
        private Button _button = null!;

        [Header("Menu Item")]
        [ReadOnly]
        [SerializeField]
        private int _id;

        public AudioSource HoverAudioSource { get; private set; } = null!;

        private Animator _animator = null!;
        private IMenuItemClickHandler? _clickHandler;

        private PieMenuGeneralSettings _generalSettings = null!;

        private RectTransform _pieRectTransform = null!;
        private PieMenuController _pieMenuController = null!;
        private PieMenuItemModel _itemModel = null!;

        private string _details = null!;
        private string _header = null!;

        private bool _idAssigned;
        private bool _mouseOverButton;

        public ReactiveTrigger<PieMenuItemModel> OnClickedTrigger { get; private set; } = null!;
        
        

        private void Awake()
        {
            _pieRectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(PieMenuItemModel model, PieMenuController pieMenuController, ReactiveTrigger<PieMenuItemModel> onClickedTrigger)
        {
            _itemModel = model;
            _pieMenuController = pieMenuController;
            OnClickedTrigger = onClickedTrigger;
            HoverAudioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _clickHandler = GetComponent<IMenuItemClickHandler>();

            PieMenuSettingsModel settingsModel = pieMenuController.PieMenuSettingsModel;
            _generalSettings = settingsModel.GeneralSettings;
        }

        public void SetId(int newId)
        {
            if (_idAssigned) {
                return;
            }

            _id = newId;
            _idAssigned = true;
        }

        public void SetHeader(string newHeader)
        {
            _header = newHeader;
        }

        public void SetDetails(string newDetails)
        {
            _details = newDetails;
        }

        public void DisplayHeader()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyHeader(_pieMenuController, _header);
            }
        }

        public void DisplayDetails()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyDetails(_pieMenuController, _details);
            }
        }

        public void OnPointerEnter()
        {
            if (_mouseOverButton) {
                return;
            }

            _button.Select();
            _mouseOverButton = true;

            // can play sound here
            _animator.SetTrigger(_mouseEnter);

            DisplayHeader();
            DisplayDetails();
        }

        public void BeforePointerExit()
        {
            _mouseOverButton = false;
        }

        public void OnPointerExit()
        {
            if (_mouseOverButton) {
                return;
            }

            _animator.SetTrigger(_mouseExit);
        }

        public void OnClick()
        {
            if (_clickHandler != null) {
                BeforePointerExit();
                _clickHandler.Handle();
            } else {
                Debug.Log("To handle clicks, you need to create a new script in which you implement the IMenuItemClickHandler interface."
                          + " Then, attach it to the appropriate Menu Item. Check the documentation to learn more.");
            }

            OnClickedTrigger.Set(_itemModel);
        }

        public void Change(ColorBlock newColors)
        {
            ColorBlock colors = _button.colors;

            colors.normalColor = newColors.normalColor;
            colors.highlightedColor = newColors.highlightedColor;
            colors.selectedColor = newColors.selectedColor;
            colors.disabledColor = newColors.disabledColor;

            _button.colors = colors;
        }

        public bool Interactable => _button.interactable;

        public int Id => _id;

        public string Header => _header;

        public string Details => _details;

        public float SizeDeltaX => _pieRectTransform.sizeDelta.x;
    }
}