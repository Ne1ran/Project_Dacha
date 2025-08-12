using Core.Reactive;
using Core.Resources.Binding.Attributes;
using Game.PieMenu.Model;
using Game.PieMenu.Settings;
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

        private PieMenuGeneralSettings _generalSettings = null!;

        private RectTransform _pieRectTransform = null!;
        private PieMenuController _pieMenuController = null!;
        private PieMenuItemModel _itemModel = null!;

        private string _detailsText = null!;
        private string _headerText = null!;

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
            _headerText = newHeader;
        }

        public void SetDetails(string newDetails)
        {
            _detailsText = newDetails;
        }

        public void DisplayHeader()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyHeaderText(_headerText);
            }
        }

        public void DisplayDetails()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyDetailsText(_detailsText);
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
            BeforePointerExit();
            OnClickedTrigger.Set(_itemModel);
        }

        public void ChangeColor(ColorBlock newColors)
        {
            ColorBlock colors = _button.colors;

            colors.normalColor = newColors.normalColor;
            colors.highlightedColor = newColors.highlightedColor;
            colors.selectedColor = newColors.selectedColor;
            colors.disabledColor = newColors.disabledColor;

            _button.colors = colors;
        }

        public void ChangeShape(Sprite shape)
        {
            // do changes
        }

        public bool Interactable => _button.interactable;

        public int Id => _id;

        public string HeaderText => _headerText;

        public string DetailsText => _detailsText;

        public float SizeDeltaX => _pieRectTransform.sizeDelta.x;
    }
}