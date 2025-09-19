using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.PieMenu.Model;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.PieMenu.Utils;
using Game.Utils;
using TMPro;
using UnityEngine;

namespace Game.PieMenu.UI
{
    public class PieMenuGeneralSettings : MonoBehaviour
    {
        private const float MaxInfoPanelScale = 2f;
        private const float MaxFillAmount = 1f;

        [SerializeField]
        private bool _infoPanelEnabled;

        [Tooltip("This option disables selection in the center of your Pie Menu. "
                 + "This can be especially useful when you have a large number of options, "
                 + "where even slight mouse movements can easily change the selection. "), SerializeField]
        private bool _selectionConstrained;

        [Range(0f, MaxInfoPanelScale), SerializeField]
        private float _scale;

        [Range(0, 100), SerializeField]
        private int _menuItemSpacing;

        [Range(0, 360), SerializeField]
        private int _globalRotation;

        [Range(250, 1000), SerializeField]
        private int _itemSize;

        [Range(-500f, 0f), SerializeField]
        private int _offsetFromCenter;

        [SerializeField]
        private AvailableInputDevices _inputDevice;

        [SerializeField]
        private Transform _infoPanel = null!;

        [SerializeField]
        private TextMeshProUGUI _header = null!;

        [SerializeField]
        private TextMeshProUGUI _details = null!;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _selectedColor;

        [SerializeField]
        private Color _disabledColor;

        [SerializeField]
        private Color _headerColor;

        [SerializeField]
        private Color _detailsColor;

        public Transform InfoPanel => _infoPanel;

        public TextMeshProUGUI Header => _header;

        public TextMeshProUGUI Details => _details;

        private PieMenuController _pieMenuController = null!;
        private PieMenuModel _pieMenuModel = null!;

        private double _previewStartTime;

        public void Initialize(PieMenuController pieMenuController)
        {
            _pieMenuController = pieMenuController;
            _pieMenuModel = pieMenuController.PieMenuSettingsModel.PieMenuModel;

            ConstraintSelection(_selectionConstrained);
        }

        private void RestoreRotationToDefault()
        {
            _globalRotation = 0;
            ChangeRotation(_globalRotation);
        }

        public void UpdateButtons(int menuItemCount, int menuItemSpacing)
        {
            if (menuItemCount <= 0) {
                return;
            }

            UpdateFillAmount(menuItemCount, menuItemSpacing);
            _pieMenuModel.SetMenuItemAngle(PieMenuUtils.CalculateItemAngle(menuItemCount, menuItemSpacing));
        }

        public void ChangeRotation(int rotation)
        {
            _pieMenuModel.SetRotation(rotation);
        }

        public void UpdateFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float fillAmount = CalculateMenuItemFillAmount(menuItemCount, menuItemSpacing);
            _pieMenuModel.SetFillAmount(fillAmount);
            ModifyFillAmount(_pieMenuController.GetMenuItems(), fillAmount, menuItemSpacing);
        }

        public void ModifyFillAmount(Dictionary<int, PieMenuItemController> menuItems, float fillAmount, float menuItemSpacing)
        {
            int iteration = 0;
            // We also remove this angle, because spacing starts not at 0 z-rotation, but x-menuItemSpacing. We remove half so it will correspond properly
            float additionalAngle = menuItemSpacing / 2f;
            foreach (PieMenuItemController? item in menuItems.Values) {
                item.ChangeFillAmount(fillAmount);
                float zAxisRotation = (fillAmount * iteration * PieMenuUtils.CircleDegreesF) + (menuItemSpacing * iteration) - additionalAngle;
                item.ChangeRotation(new(0, 0, zAxisRotation));
                iteration++;
            }
        }

        public float CalculateMenuItemFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacingPercentage = PieMenuUtils.CalculateTotalSpacingPercentage(menuItemCount, menuItemSpacing);
            float fillAmountLeft = MaxFillAmount - totalSpacingPercentage;
            float fillAmount = fillAmountLeft / menuItemCount;
            return fillAmount;
        }

        public void SetOffset(int newOffset)
        {
            _offsetFromCenter = newOffset;
        }

        public void Rotate(Transform menuItem, Quaternion rotation)
        {
            int iconDirIndex = 0;
            Transform iconDir = menuItem.GetChild(iconDirIndex);

            Quaternion parentRotation = menuItem.rotation;
            Quaternion newRotation = parentRotation * rotation;

            iconDir.rotation = newRotation;
        }

        public void Rotate(Dictionary<int, PieMenuItemController> menuItems, Quaternion iconDirRotation)
        {
            //The first element in the list of icons requires a different mathematical pattern than the rest.
            // PieMenuUtils.RotateFirstElement(menuItems[0].transform, iconDirRotation);
            // PieMenuUtils.RotateOtherElements(menuItems, iconDirRotation);
        }

        public void Resize(float newScale)
        {
            if (!_pieMenuModel.InfoPanelEnabled) {
                return;
            }

            ChangeInfoPanelScale(newScale);
            SetScale(newScale);
        }

        // todo neiran dont forget to redo or pick
        public void MoveIcon(Transform icon, int offsetFromCenter)
        {
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(offsetFromCenter, 0f, 0f);
        }

        public int GetInitialOffset(Transform iconDir)
        {
            Transform icon = iconDir.GetChild(0);
            int currentOffset = (int) icon.localPosition.x;
            return currentOffset;
        }

        private int CalculateNewOffset(float initialOffset)
        {
            int initialSize = _pieMenuModel.MenuItemInitialSize;
            int currentSize = _pieMenuModel.MenuItemSize;

            float newOffset = (initialOffset / initialSize) * currentSize;
            return Mathf.RoundToInt(newOffset);
        }

        public AnimationClip GetAnimation(RuntimeAnimatorController runtimeController)
        {
            return runtimeController.animationClips[0];
        }

        public async UniTask PlayAnimationAsync(bool isOpen, CancellationToken token)
        {
            Transform pieMenuTransform = _pieMenuController.transform;

            if (isOpen) {
                pieMenuTransform.localScale = Vector3.zero;
                await pieMenuTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).ToUniTask(cancellationToken: token);
            } else {
                pieMenuTransform.localScale = Vector3.one;
                await pieMenuTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask(cancellationToken: token);
            }
        }

        public void SetScale(float scale)
        {
            _scale = scale;
        }

        public void SetActiveInfoPanel(bool isActive)
        {
            InfoPanel.SetActive(isActive);
        }

        public void ChangeHeaderColor(Color newColor)
        {
            Header.color = newColor;
        }

        public void ChangeDetailsColor(Color newColor)
        {
            Details.color = newColor;
        }

        public void ChangeInfoPanelScale(float scale)
        {
            InfoPanel.localScale = new(scale, scale, scale);
        }

        public void ModifyHeaderText(string newHeader)
        {
            Header.text = newHeader;
        }

        public void ModifyDetailsText(string newMessage)
        {
            Details.text = newMessage;
        }

        public void ConstraintSelection(bool selectionConstrained)
        {
            _pieMenuModel.SetSelectionConstraintState(selectionConstrained);

            MenuItemSelector selector = _pieMenuController.PieMenuSettingsModel.MenuItemSelector;
            selector.ToggleSelectionConstraint(selectionConstrained);
            if (!selectionConstrained) {
                return;
            }

            float maxDistance = CalculateConstraintMaxDistance();
            selector.SetConstraintMaxDistance((int) maxDistance);
        }

        public float CalculateConstraintMaxDistance()
        {
            return ItemSize * 0.10f;
        }

        public float Scale => _scale;
        public int MenuItemSpacing => _menuItemSpacing;
        public int GlobalRotation => _globalRotation;
        public int ItemSize => _itemSize;
        public int OffsetFromCenter => _offsetFromCenter;
    }
}