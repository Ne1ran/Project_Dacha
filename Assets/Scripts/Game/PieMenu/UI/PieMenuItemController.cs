using Core.Reactive;
using Core.Resources.Binding.Attributes;
using Game.PieMenu.Model;
using Game.PieMenu.Utils;
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
        [ComponentBinding]
        private Animator _animator = null!;
        [ComponentBinding("Icon")]
        private Image _itemImage = null!;
        [ComponentBinding]
        private Image _fillImage = null!;

        [Header("Menu Item")]
        [ReadOnly]
        [SerializeField]
        private int _id;

        private PieMenuGeneralSettings _generalSettings = null!;
        private PieMenuItemModel _itemModel = null!;

        private string _detailsText = null!;
        private string _headerText = null!;

        private bool _idAssigned;
        private bool _mouseOverButton;
        private float _fillAmount;

        public ReactiveTrigger<PieMenuItemModel> OnClickedTrigger { get; private set; } = null!;

        public void Initialize(PieMenuItemModel model, PieMenuGeneralSettings settings, ReactiveTrigger<PieMenuItemModel> onClickedTrigger)
        {
            _generalSettings = settings;
            _itemModel = model;
            OnClickedTrigger = onClickedTrigger;
            SetHeader(model.Title);
            SetDetails(model.Description);
            ItemImage = model.Icon;
            SetItemIconToCenter();
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

        public void ChangeFillAmount(float fillAmount)
        {
            _fillAmount = fillAmount;
            _fillImage.fillAmount = fillAmount;
            SetItemIconToCenter();
        }

        public void ChangeRotation(Vector3 rotation)
        {
            transform.Rotate(rotation);
        }

        public void SetItemIconToCenter()
        {
            int itemSize = _generalSettings.ItemSize;
            Vector2 iconSize = _itemImage.rectTransform.sizeDelta;
            float iconSizeValue = Mathf.Max(iconSize.x, iconSize.y);
            float startPositionX = -(itemSize / 2f - iconSizeValue);
            float startPositionY = iconSizeValue / 2f;

            float circleDegrees = PieMenuUtils.CircleDegreesF * _fillAmount;
            float centerDegrees = circleDegrees / 2f;

            float sin = Mathf.Sin(centerDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(centerDegrees * Mathf.Deg2Rad);
            float positionX = startPositionX * cos;
            float positionY = (startPositionY / (1f - sin)) - iconSizeValue / 4f;
            
            _itemImage.rectTransform.anchoredPosition = new(positionX, positionY);
        }

        public bool Interactable => _button.interactable;

        public int Id => _id;

        private Sprite? ItemImage
        {
            set => _itemImage.sprite = value;
        }
    }
}