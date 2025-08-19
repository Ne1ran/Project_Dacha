using System.Collections.Generic;
using Core.Reactive;
using Core.Resources.Binding.Attributes;
using Game.PieMenu.Model;
using Game.PieMenu.Utils;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PieMenu.UI
{
    [PrefabPath("UI/Dialogs/PieMenu/pfPieMenuItem")]
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

        private string _startDetailText = null!;
        private string _startHeaderText = null!;
        
        private string _currentDetailText = null!;
        private string _currentHeaderText = null!;

        private bool _idAssigned;
        private bool _mouseOverButton;
        private float _fillAmount;

        public ReactiveTrigger<PieMenuItemModel> OnClickedTrigger { get; private set; } = null!;

        public void Initialize(PieMenuItemModel model, PieMenuGeneralSettings settings, ReactiveTrigger<PieMenuItemModel> onClickedTrigger)
        {
            _generalSettings = settings;
            _itemModel = model;
            OnClickedTrigger = onClickedTrigger;
            _startHeaderText = model.Title;
            _startDetailText = model.BaseDescription;
            SetItemIconToCenter();
            
            _currentDetailText = _startDetailText;
            _currentHeaderText = _startHeaderText;

            UseSelectionItemModel(0);
        }

        public void SetId(int newId)
        {
            if (_idAssigned) {
                return;
            }

            _id = newId;
            _idAssigned = true;
        }

        public void DisplayHeader()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyHeaderText(_currentHeaderText);
            }
        }

        public void DisplayDetails()
        {
            if (_generalSettings != null) {
                _generalSettings.ModifyDetailsText(_currentDetailText);
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

        public void OnScrollForward()
        {
            UseSelectionItemModel(_itemModel.CurrentSelectionIndex + 1);
        }

        public void OnScrollBackwards()
        {
            UseSelectionItemModel(_itemModel.CurrentSelectionIndex - 1);
        }

        private void UseSelectionItemModel(int selectionIndex)
        {
            List<PieMenuItemSelectionModel> selectionModels = _itemModel.SelectionModels;
            if (selectionModels.Count == 0) {
                return;
            }
            
            if (selectionIndex >= selectionModels.Count) {
                selectionIndex = 0;
            }

            if (selectionIndex < 0) {
                selectionIndex = selectionModels.Count - 1;
            }

            if (_itemModel.CurrentSelectionIndex == selectionIndex) {
                return;
            }

            PieMenuItemSelectionModel itemSelectionModel = selectionModels[selectionIndex];
            
            ItemImage = itemSelectionModel.Icon;

            _currentDetailText = _startDetailText.Substitute("{substitute}", itemSelectionModel.DescriptionSubstituteText);
            DisplayDetails();
            _itemModel.CurrentSelectionIndex = selectionIndex;
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
            float circleRadius = itemSize / 2f - iconSizeValue;

            float circleDegrees = PieMenuUtils.CircleDegreesF * _fillAmount;
            float centerDegrees = circleDegrees / 2f;

            float deg2Rad = centerDegrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(deg2Rad);
            float cos = Mathf.Cos(deg2Rad);
            float positionX = -circleRadius * cos;
            float positionY = circleRadius * sin;
            
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