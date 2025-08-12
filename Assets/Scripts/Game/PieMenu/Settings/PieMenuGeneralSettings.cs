using System.Collections.Generic;
using Game.PieMenu.Model;
using Game.PieMenu.UI;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.PieMenu.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PieMenu.Settings
{
    public class PieMenuGeneralSettings : MonoBehaviour
    {
        public const float MaxInfoPanelScale = 2f;

        [SerializeField]
        private bool _infoPanelEnabled;

        [SerializeField]
        private Color _headerColor;

        [SerializeField]
        private Color _detailsColor;

        [Range(0f, MaxInfoPanelScale)]
        [SerializeField]
        private float _scale;

        [SerializeField]
        private AvailableInputDevices _inputDevice;

        [Header("Left/Right")]
        [Range(-1000, 1000)]
        [SerializeField]
        private int _horizontalPosition;

        [Header("Up/Down")]
        [Range(1000, -1000)]
        [SerializeField]
        private int _verticalPosition;

        [Tooltip("This option disables selection in the center of your Pie Menu. "
                 + "This can be especially useful when you have a large number of options, "
                 + "where even slight mouse movements can easily change the selection. ")]
        [SerializeField]
        private bool _selectionConstrained;

        [Tooltip("This setting will allow the player to close the menu by clicking the 'Close' button. "
                 + "By default, this is set to 'Esc' for mouse input devices. For more information, please refer to the documentation.")]
        [SerializeField]
        private bool _closeable;

        [SerializeField]
        private bool _backgroundEnabled;

        [SerializeField]
        private Color _backgroundColor;

        [Range(1, 10)]
        [SerializeField]
        private int _menuItemCount;

        [Range(0, 100)]
        [SerializeField]
        private int _menuItemSpacing;

        [Range(0, 360)]
        [SerializeField]
        private int _rotation;

        [Range(250, 1000)]
        [SerializeField]
        private int _size;

        [SerializeField]
        private bool _addIcons;

        [Range(-500f, 0f)]
        [SerializeField]
        private int _offsetFromCenter;

        [Range(0f, 3f)]
        [SerializeField]
        private float _iconScale;

        [SerializeField]
        private GameObject _iconDir;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _selectedColor;

        [SerializeField]
        private Color _disabledColor;

        [SerializeField]
        public List<Sprite> _shapes = new();

        [SerializeField]
        private List<AnimatorOverrideController> _animatorOverrideControllers;

        public GameObject IconDir => _iconDir;

        private PieMenuController _pieMenuController = null!;

        public int ShapeDropdownListIndex { get; private set; }
        public List<string> ShapeNames { get; private set; } = new();
        private Image _background = null!;

        public const string TriggerActiveTrue = "ActiveTrue";
        public const string TriggerActiveFalse = "ActiveFalse";

        public List<AnimatorOverrideController> AnimatorOverrideControllers => _animatorOverrideControllers;

        public int AnimationDropdownList { get; private set; }
        public List<string> AnimationNames { get; private set; } = new();

        private Animator _animator = null!;
        private AnimationClip _clip = null!;
        private double _previewStartTime;

        public void OnCountChange()
        {
            RestoreRotationToDeafault();
            ValidateFields();
            HandleMenuItemCountChange(_pieMenuController, _menuItemCount, _menuItemSpacing);
            _rotation = CalculateNewRotation(_menuItemCount, _menuItemSpacing);
            HandleRotationChange(_pieMenuController, _rotation);
        }

        public void OnSpacingChange()
        {
            RestoreRotationToDeafault();
            ValidateFields();
            HandleButtonSpacingChange(_pieMenuController, _menuItemCount, _menuItemSpacing);
            _rotation = _rotation = CalculateNewRotation(_menuItemCount, _menuItemSpacing);
            HandleRotationChange(_pieMenuController, _rotation);
        }

        public void OnRotationChange()
        {
            HandleRotationChange(_pieMenuController, _rotation);
        }

        public void OnSizeChange()
        {
            HandleSizeChange(_pieMenuController, _size);
        }

        private void RestoreRotationToDeafault()
        {
            _rotation = 0;
            OnRotationChange();
        }

        private void InitializeGeneralSettings()
        {
            _size = _pieMenuController.PieMenuModel.MenuItemSize;
            _pieMenuController.PieMenuModel.SetSpacing(_menuItemSpacing);
        }

        private void ValidateFields()
        {
            if (_menuItemCount == 1) {
                _menuItemSpacing = 0;
            }
        }

        private const string Name = "Menu Item ";

        public static int CalculateNewRotation(int menuItemCount, int menuItemSpacing)
        {
            // The following method calculates a new rotation to ensure symmetry based on the provided number of Menu Items
            int rotation;
            int circleDegrees = 360;
            if (menuItemCount % 2 != 0) {
                rotation = circleDegrees - 90 / menuItemCount - menuItemSpacing / 2;
            } else {
                rotation = circleDegrees - menuItemSpacing / 2;
            }

            return rotation;
        }

        public void HandleMenuItemCountChange(PieMenuController pieMenu, int menuItemCount, int menuItemSpacing)
        {
            HandleButtonCountChange(pieMenu, menuItemCount);
            UpdateButtons(pieMenu, menuItemCount, menuItemSpacing);
        }

        public void HandleButtonSpacingChange(PieMenuController pieMenu, int menuItemCount, int menuItemSpacing)
        {
            UpdateButtons(pieMenu, menuItemCount, menuItemSpacing);
        }

        public void HandleRotationChange(PieMenuController pieMenu, int rotation)
        {
            HandleRotationChanged(pieMenu, rotation);
        }

        public void HandleSizeChange(PieMenuController pieMenu, int size)
        {
            HandleSizeChanged(pieMenu, size);
        }

        public void UpdateButtons(PieMenuController pieMenu, int menuItemCount, int menuItemSpacing)
        {
            if (menuItemCount <= 0) {
                return;
            }

            pieMenu.PieMenuModel.SetSpacing(menuItemSpacing);
            UpdateFillAmount(pieMenu, menuItemCount, menuItemSpacing);
            PieMenuUtils.CalculateItemAngle(pieMenu);
            bool iconsEnabled = pieMenu.PieMenuModel.IconsEnabled;
            HandleRotation(pieMenu, iconsEnabled);
            Change(pieMenu.GetMenuItems());
        }

        public void HandleRotationChanged(PieMenuController pieMenu, int rotation)
        {
            ChangeRotation(pieMenu, rotation);
            ApplyRotationForIcons(pieMenu, rotation);
        }

        private void ChangeRotation(PieMenuController pieMenu, int rotation)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            Quaternion newRotation = Quaternion.Euler(0f, 0f, rotation);
            menuItemsDir.rotation = newRotation;
            pieMenu.PieMenuModel.SetRotation(rotation);
        }

        private void ApplyRotationForIcons(PieMenuController pieMenu, int rotation)
        {
            bool iconsEnabled = pieMenu.PieMenuModel.IconsEnabled;
            if (!iconsEnabled) {
                return;
            }

            Quaternion iconDirRotation = CalculateIconDirRotation(pieMenu, iconsEnabled);
            Quaternion additionalRotation = Quaternion.Euler(0f, 0f, rotation);
            iconDirRotation *= additionalRotation;
            Rotate(pieMenu.GetMenuItems(), iconDirRotation);
        }

        public static float CalculatePieMenuScale(PieMenuController pieMenu, int size)
        {
            int initialSize = pieMenu.PieMenuModel.MenuItemInitialSize;

            float newScale = (float) size / initialSize;
            return newScale;
        }

        public void HandleSizeChanged(PieMenuController pieMenu, int size)
        {
            pieMenu.PieMenuModel.SetMenuItemSize(size);
            ChangeMenuItemsSize(pieMenu, size);
            float newScale = CalculatePieMenuScale(pieMenu, size);
            pieMenu.PieMenuModel.SetScale(newScale);
            Resize(pieMenu, newScale);
            UpdateSelectionHandler(pieMenu);
        }

        private void UpdateSelectionHandler(PieMenuController pieMenu)
        {
            if (!pieMenu.PieMenuModel.SelectionConstrained) {
                return;
            }

            float maxDistance = CalculateConstraintMaxDistance(pieMenu);
            pieMenu.PieMenuSettingsModel.MenuItemSelector.SetConstraintMaxDistance((int) maxDistance);
        }

        private void ChangeMenuItemsSize(PieMenuController pieMenu, int size)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                ChangeSize(menuItem.Value.transform, size);
            }
        }

        private void ChangeSize(Transform menuItem, int size)
        {
            Image image = menuItem.GetComponent<Image>();
            RectTransform rectTransform = image.rectTransform;
            rectTransform.sizeDelta = new(size, size);
        }

        public void Change(Dictionary<int, PieMenuItemController> menuItems)
        {
            int iteration = 0;
            foreach (KeyValuePair<int, PieMenuItemController> item in menuItems) {
                item.Value.transform.name = Name + iteration;
                iteration++;
            }
        }

        public void HandleButtonCountChange(PieMenuController pieMenu, int newMenuItemCount)
        {
            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;
            int currentMenuItemsCount = menuItemsDir.childCount;

            if (currentMenuItemsCount > newMenuItemCount) {
                Remove(pieMenu, newMenuItemCount);
            } else if (currentMenuItemsCount < newMenuItemCount) {
                Create(pieMenu, pieMenu.MenuItemControllerTemplate.gameObject, newMenuItemCount);
            }
        }

        public void UpdateFillAmount(PieMenuController pieMenu, int menuItemCount, int menuItemSpacing)
        {
            float fillAmount = CalculateMenuItemFillAmount(menuItemCount, menuItemSpacing);

            pieMenu.PieMenuModel.SetFillAmount(fillAmount);
            ModifyFillAmount(pieMenu.GetMenuItems(), fillAmount, menuItemSpacing);
        }

        public void Remove(PieMenuController pieMenu, int menuItemsCount)
        {
            int currentMenuItemsCount = pieMenu.ViewModel.PieMenuItems.Count;
            int itemsToRemove = currentMenuItemsCount - menuItemsCount;

            int lastItemIndex = currentMenuItemsCount - 1;
            for (int i = 0; i < itemsToRemove; i++) {
                pieMenu.ViewModel.RemoveMenuItem(lastItemIndex);
                lastItemIndex--;
            }
        }

        public void Create(PieMenuController pieMenu, GameObject templateObj, int newMenuItemsCount)
        {
            Dictionary<int, PieMenuItemController> menuItems = pieMenu.ViewModel.PieMenuItems;

            int menuItemsToCreate = newMenuItemsCount - menuItems.Count;

            Transform menuItemsDir = pieMenu.PieMenuElements.MenuItemsDir;

            for (int i = 0; i < menuItemsToCreate; i++) {
                GameObject newMenuItem = Instantiate(templateObj, menuItemsDir);
                int menuItemIndex = menuItems.Count;
                pieMenu.ViewModel.InitializeMenuItem(newMenuItem.transform, menuItemIndex);
            }
        }

        private const float CircleDegrees = 360f;

        public void ModifyFillAmount(Dictionary<int, PieMenuItemController> menuItems, float fillAmount, float menuItemSpacing)
        {
            int iteration = 0;
            foreach (KeyValuePair<int, PieMenuItemController> item in menuItems) {
                Image image = item.Value.GetComponent<Image>();
                image.fillAmount = fillAmount;

                float zAxisRotation = (fillAmount * iteration * CircleDegrees) + (menuItemSpacing * iteration);
                item.Value.transform.rotation = Quaternion.Euler(0, 0, zAxisRotation);
                iteration++;
            }
        }

        public float CalculateMenuItemFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacingPercentage = PieMenuUtils.CalculateTotalSpacingPercentage(menuItemCount, menuItemSpacing);

            float maxFillAmount = 1f;
            float fillAmountLeft = maxFillAmount - totalSpacingPercentage;
            float fillAmount = fillAmountLeft / menuItemCount;
            return fillAmount;
        }

        public void Initialize(PieMenuController pieMenuController)
        {
            _pieMenuController = pieMenuController;
            InitializeMenuItemColorSettings();
            InitializeIconsSettings();
            InitializeGeneralSettings();
            OnSelectionSettingsInitialized();
            InitializeAnimationsSettings();
            OnCloseSettingsInitialized();
            InitializeInfoPanelSettings();
            SetCurrentlyUsedShape();
            _background = _pieMenuController.PieMenuElements.Background;
            _pieMenuController.PieMenuModel.SetBackgroundEnabled(_backgroundEnabled);
        }

        public void OnColorValueChange()
        {
            ColorBlock colors = new() {
                    normalColor = _normalColor,
                    highlightedColor = _normalColor,
                    selectedColor = _selectedColor,
                    disabledColor = _disabledColor
            };

            HandleColorChange(_pieMenuController, colors);
        }

        private void InitializeMenuItemColorSettings()
        {
            SetColorFields();
        }

        private void SetColorFields()
        {
            Transform menuItem = _pieMenuController.MenuItemControllerTemplate.transform;
            ColorBlock colors = menuItem.GetComponent<Button>().colors;
            _normalColor = colors.normalColor;
            _selectedColor = colors.selectedColor;
            _disabledColor = colors.disabledColor;
        }

        public void HandleColorChange(PieMenuController pieMenu, ColorBlock newColors)
        {
            ChangeMenuItemsColors(pieMenu, newColors);
        }

        private void ChangeMenuItemsColors(PieMenuController pieMenu, ColorBlock newColors)
        {
            foreach (KeyValuePair<int, Button> button in pieMenu.ViewModel.ButtonComponents) {
                Change(button.Value, newColors);
            }
        }

        private void Change(Button button, ColorBlock newColors)
        {
            ColorBlock colors = button.colors;

            colors.normalColor = newColors.normalColor;
            colors.highlightedColor = newColors.highlightedColor;
            colors.selectedColor = newColors.selectedColor;
            colors.disabledColor = newColors.disabledColor;

            button.colors = colors;
        }

        public void OnEnableSettingChange()
        {
            HandleIconEnableSettingChange(_pieMenuController, _addIcons);

            if (_addIcons) {
                return;
            }

            _offsetFromCenter = 0;
            _iconScale = 0f;
        }

        public void OnMoveValueChange()
        {
            if (!_addIcons) {
                return;
            }
            HandleIconOffsetChange(_pieMenuController, _offsetFromCenter);
        }

        public void OnScaleValueChange()
        {
            if (!_addIcons) {
                return;
            }
            HandleIconScaleChange(_pieMenuController, _iconScale);
        }

        public void SetOffset(int newOffset)
        {
            _offsetFromCenter = newOffset;
        }

        private void InitializeIconsSettings()
        {
            _pieMenuController.PieMenuModel.SetIconsEnabled(_addIcons);
        }

        public void HandleIconEnableSettingChange(PieMenuController pieMenu, bool addIcons)
        {
            HandleIconEnabled(pieMenu, addIcons);
        }

        public void HandleIconOffsetChange(PieMenuController pieMenu, int offsetFromCenter)
        {
            Handle(pieMenu, offsetFromCenter);
        }

        public void HandleIconScaleChange(PieMenuController pieMenu, float iconScale)
        {
            Handle(pieMenu, iconScale);
        }

        //Positioning the icon within the button involves calculating the appropriate rotation for the icon directory.
        //This result is also needed to calculate the rotation for individual icons.

        public Quaternion Calculate(float buttonFillAmount)
        {
            float circleDegrees = 360f;
            float buttonSize = (buttonFillAmount * circleDegrees);
            float centreOfTheButton = buttonSize / 2f;

            float zAxisRotation = -centreOfTheButton;

            Quaternion iconDirRotation = Quaternion.Euler(0f, 0f, zAxisRotation);

            return iconDirRotation;
        }

        public void Rotate(Transform menuItem, Quaternion rotation)
        {
            int iconDirIndex = 0;
            Transform iconDir = menuItem.GetChild(iconDirIndex);

            Quaternion parentRotation = menuItem.rotation;
            Quaternion newRotation = parentRotation * rotation;

            iconDir.rotation = newRotation;
        }

        public void CreateOrDestroyIcon(bool addIcons, Transform menuItem)
        {
            if (addIcons) {
                CreateIcon(menuItem);
            } else {
                DestroyIcon(menuItem);
            }
        }

        private void CreateIcon(Transform menuItem)
        {
            GameObject iconGo = Instantiate(_iconDir, menuItem);
            iconGo.name = "IconDir";
        }

        private static void DestroyIcon(Transform menuItem)
        {
            if (menuItem.childCount > 0) {
                GameObject iconDir = menuItem.GetChild(0).gameObject;
                DestroyImmediate(iconDir);
            }
        }

        public void Rotate(Dictionary<int, PieMenuItemController> menuItems, Quaternion iconDirRotation)
        {
            //The first element in the list of icons requires a different mathematical pattern than the rest.
            RotateFirstElement(menuItems[0].transform, iconDirRotation);
            RotateOtherElements(menuItems, iconDirRotation);
        }

        private static void RotateFirstElement(Transform menuItem, Quaternion iconDirRotation)
        {
            Quaternion firstIconRotation = Quaternion.Euler(0f, 0f, Mathf.Abs(iconDirRotation.z));
            Transform firstIconDir = menuItem.GetChild(0).transform;
            Transform firstIcon = firstIconDir.GetChild(0).transform;
            firstIcon.rotation = firstIconRotation;
        }

        private static void RotateOtherElements(Dictionary<int, PieMenuItemController> menuItems, Quaternion iconDirRotation)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in menuItems) {
                float menuItemRotationZ = menuItem.Value.transform.rotation.z;
                float iconRotationZ = -(menuItemRotationZ - iconDirRotation.z);
                Transform iconDir = menuItem.Value.transform.GetChild(0).transform;
                Transform icon = iconDir.GetChild(0).transform;
                icon.rotation = Quaternion.Euler(0f, 0f, iconRotationZ);
            }
        }

        public void ResizeIcon(PieMenuController pieMenu, float newScale)
        {
            if (!pieMenu.PieMenuModel.IconsEnabled) {
                return;
            }

            SetScale(newScale);
            HandleIconScaleChange(pieMenu, newScale);
            int initialOffset = GetInitialOffset(IconDir.transform);
            int newOffset = CalculateNewOffset(pieMenu, initialOffset);
            SetOffset(newOffset);
            HandleIconOffsetChange(pieMenu, newOffset);
        }

        public int GetInitialOffset(Transform iconDir)
        {
            Transform icon = iconDir.GetChild(0);
            int currentOffset = (int) icon.localPosition.x;
            return currentOffset;
        }

        private int CalculateNewOffset(PieMenuController pieMenu, float initialOffset)
        {
            int initialSize = pieMenu.PieMenuModel.MenuItemInitialSize;
            int currentSize = pieMenu.PieMenuModel.MenuItemSize;

            float newOffset = (initialOffset / initialSize) * currentSize;
            return Mathf.RoundToInt(newOffset);
        }

        public void HandleIconEnabled(PieMenuController pieMenu, bool addIcons)
        {
            pieMenu.PieMenuModel.SetIconsEnabled(addIcons);

            ApplyChangesToExistingMenuItems(pieMenu, addIcons);

            Resize(pieMenu, pieMenu.PieMenuModel.Scale);
        }

        public void HandleRotation(PieMenuController pieMenu, bool iconsEnabled)
        {
            if (iconsEnabled) {
                Quaternion iconDirRotation = CalculateIconDirRotation(pieMenu, iconsEnabled);

                foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                    Rotate(menuItem.Value.transform, iconDirRotation);
                }

                Rotate(pieMenu.GetMenuItems(), iconDirRotation);
            }
        }

        public Quaternion CalculateIconDirRotation(PieMenuController pieMenu, bool iconsEnabled)
        {
            Quaternion iconDirRotation = Quaternion.identity;

            if (iconsEnabled) {
                iconDirRotation = Calculate(pieMenu.PieMenuModel.MenuItemFillAmount);
            }

            return iconDirRotation;
        }

        private void ApplyChangesToExistingMenuItems(PieMenuController pieMenu, bool addIcons)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                CreateOrDestroyIcon(addIcons, menuItem.Value.transform);
            }

            HandleRotation(pieMenu, addIcons);
        }

        public void Resize(PieMenuController pieMenu, float newScale)
        {
            if (!pieMenu.PieMenuModel.InfoPanelEnabled) {
                return;
            }

            ChangeScale(pieMenu, newScale);
            SetScale(newScale);
        }

        public void Move(Transform icon, int offsetFromCenter)
        {
            RectTransform rectTransform = icon.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector3(offsetFromCenter, 0f, 0f);
        }

        public void Handle(PieMenuController pieMenu, int offsetFromCenter)
        {
            ChangeIconsPosition(pieMenu, offsetFromCenter);
        }

        private void ChangeIconsPosition(PieMenuController pieMenu, int offsetFromCenter)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                Move(GetIcon(menuItem.Value.transform), offsetFromCenter);
            }
        }

        public void ChangeScale(Transform icon, float newScale)
        {
            icon.localScale = new(newScale, newScale, newScale);
        }

        public void Handle(PieMenuController pieMenu, float iconScale)
        {
            ChangeMenuItemsScale(pieMenu, iconScale);
        }

        private void ChangeMenuItemsScale(PieMenuController pieMenu, float iconScale)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                ChangeScale(GetIcon(menuItem.Value.transform), iconScale);
            }
        }

        public bool CheckIfIconsAreEnabled(Transform menuItem)
        {
            return menuItem.transform.childCount > 0;
        }

        public Transform GetIcon(Transform menuItem)
        {
            Transform itemDir = menuItem.transform.GetChild(0);
            Transform icon = itemDir.transform.GetChild(0);
            return icon;
        }

        public void ChangeIcon(Transform menuItem, Sprite sprite)
        {
            GetIcon(menuItem).GetComponent<Image>().sprite = sprite;
        }

        public void CreateAnimationsDropdownList(int list)
        {
            AnimationDropdownList = list;
        }

        public void SwapAnimations()
        {
            SwapAnimationControllers(_pieMenuController, AnimationDropdownList);
        }

#if UNITY_EDITOR
        public void StartPreview()
        {
            _previewStartTime = EditorApplication.timeSinceStartup;
            PreparePreview();
            EditorApplication.update += DoPreview;
        }

        private void DoPreview()
        {
            double timeElapsed = EditorApplication.timeSinceStartup - _previewStartTime;
            _clip.SampleAnimation(_animator.gameObject, (float) timeElapsed);

            if (!(timeElapsed >= _clip.length)) {
                return;
            }

            EditorApplication.update -= DoPreview;

            if (!_infoPanelEnabled) {
                return;
            }
            _infoPanelEnabled = false;
            SetActive(_pieMenuController, true);
        }
#endif

        private void PreparePreview()
        {
            AnimatorOverrideController overrideController = GetAnimator(AnimationDropdownList);
            _clip = GetAnimation(overrideController);

            _infoPanelEnabled = _pieMenuController.PieMenuModel.InfoPanelEnabled;
            if (_infoPanelEnabled) {
                SetActive(_pieMenuController, false);
            }
        }

        private void InitializeAnimationsSettings()
        {
            _animator = _pieMenuController.PieMenuElements.Animator;

            InitializeList();
            GetSelectedAnimation();
        }

        private void InitializeList()
        {
            AnimationNames = new();
            foreach (AnimatorOverrideController overrideController in AnimatorOverrideControllers) {
                AnimationClip animationClip = GetAnimation(overrideController);

                AnimationNames.Add(animationClip.name);
            }
        }

        private void GetSelectedAnimation()
        {
            RuntimeAnimatorController runtimeController = _animator.runtimeAnimatorController;
            AnimationClip animationClip = GetAnimation(runtimeController);
            string currentAnimationName = animationClip.name;

            //if the List<T>.FindIndex method doesn't find a matching element, it returns -1
            int index = AnimationNames.FindIndex(name => name == currentAnimationName);

            if (index == -1) {
                throw new("It's likely that you've added a new animation to your menu incorrectly."
                          + " Please refer to the documentation to learn more.");
            }

            CreateAnimationsDropdownList(index);
            _pieMenuController.PieMenuModel.SetAnimation(animationClip);
        }

        public void SwapAnimationControllers(PieMenuController pieMenu, int controllerIndex)
        {
            AnimatorOverrideController overrideController = GetAnimator(controllerIndex);

            pieMenu.PieMenuElements.Animator.runtimeAnimatorController = overrideController;

            pieMenu.PieMenuModel.SetAnimation(GetAnimation(overrideController));
        }

        public AnimatorOverrideController GetAnimator(int controllerIndex)
        {
            return _animatorOverrideControllers[controllerIndex];
        }

        public AnimationClip GetAnimation(RuntimeAnimatorController runtimeController)
        {
            int animationIndex = 0;
            return runtimeController.animationClips[animationIndex];
        }

        public void PlayAnimation(Animator animator, string trigger)
        {
            animator.SetTrigger(trigger);
        }

        public void OnBackgroundEnableValueChange()
        {
            _background.gameObject.SetActive(_backgroundEnabled);
        }

        public void OnColorChange()
        {
            _background.color = _backgroundColor;
        }

        public void OnCloseableValueChange()
        {
            if (Application.isPlaying) {
                Handle(_pieMenuController, _closeable);
            }
        }

        public void Handle(PieMenuController pieMenu, bool closeable)
        {
            pieMenu.PieMenuModel.SetCloseableState(closeable);
        }

        private void OnCloseSettingsInitialized()
        {
            OnCloseableValueChange();
        }

        public void OnInfoPanelEnableValueChange()
        {
            HandleEnableValueChange(_pieMenuController, _infoPanelEnabled);
        }

        public void OnHeaderColorChange()
        {
            ChangeHeaderColor(_pieMenuController, _headerColor);
        }

        public void OnDetailsColorChange()
        {
            ChangeDetailsColor(_pieMenuController, _detailsColor);
        }

        public void OnScaleChange()
        {
            ChangeScale(_pieMenuController, _scale);
        }

        public void SetScale(float scale)
        {
            _scale = scale;
        }

        private void InitializeInfoPanelSettings()
        {
            SetInspectorFields();
            _pieMenuController.PieMenuModel.SetInfoPanelEnabled(_infoPanelEnabled);
        }

        private void SetInspectorFields()
        {
            if (_headerColor != null) {
                return;
            }

            PieMenuElements pieMenuElements = _pieMenuController.PieMenuElements;
            _headerColor = pieMenuElements.Header.color;
            _detailsColor = pieMenuElements.Details.color;
            _scale = pieMenuElements.InfoPanel.localScale.x;
        }

        public void HandleEnableValueChange(PieMenuController pieMenu, bool enabled)
        {
            pieMenu.PieMenuModel.SetInfoPanelEnabled(enabled);
            Resize(pieMenu, pieMenu.PieMenuModel.Scale);
            SetActive(pieMenu, enabled);
        }

        public void SetActive(PieMenuController pieMenu, bool isActive)
        {
            pieMenu.PieMenuElements.InfoPanel.gameObject.SetActive(isActive);
        }

        public void ChangeHeaderColor(PieMenuController pieMenu, Color newColor)
        {
            pieMenu.PieMenuElements.Header.color = newColor;
        }

        public void ChangeDetailsColor(PieMenuController pieMenu, Color newColor)
        {
            pieMenu.PieMenuElements.Details.color = newColor;
        }

        public void ChangeScale(PieMenuController pieMenu, float scale)
        {
            pieMenu.PieMenuElements.InfoPanel.localScale = new(scale, scale, scale);
        }

        public void ModifyHeader(PieMenuController pieMenu, string newHeader)
        {
            pieMenu.PieMenuElements.Header.text = newHeader;
        }

        public void ModifyDetails(PieMenuController pieMenu, string newMessage)
        {
            pieMenu.PieMenuElements.Details.text = newMessage;
        }

        public void RestoreDefaultInfoPanelText(PieMenuController pieMenu)
        {
            string placeholderHeaderText = "Header";
            string placeholderDetailsText = "Test text";
            ModifyHeader(pieMenu, placeholderHeaderText);
            ModifyDetails(pieMenu, placeholderDetailsText);
        }

        public void OnInputDeviceChange()
        {
            PlayerPrefs.SetInt(PieMenuUtils.InputDevice, (int) _inputDevice);
        }

        public void OnPositionChange()
        {
            Handle(_pieMenuController, _horizontalPosition, _verticalPosition);
        }

        public void ResetPosition()
        {
            _horizontalPosition = 0;
            _verticalPosition = 0;
            Handle(_pieMenuController, 0, 0);
        }

        public void Handle(PieMenuController pieMenu, int horizontalPosition, int verticalPosition)
        {
            RectTransform rectTransform = pieMenu.GetComponent<RectTransform>();
            Vector2 anchoredPosition = rectTransform.anchoredPosition;

            anchoredPosition.Set(horizontalPosition, verticalPosition);
            rectTransform.anchoredPosition = anchoredPosition;
            pieMenu.PieMenuModel.SetAnchoredPosition(rectTransform);
        }

        public void OnConstraintValueChange()
        {
            if (Application.isPlaying) {
                ConstraintSelection(_pieMenuController, _selectionConstrained);
            }
        }

        private void OnSelectionSettingsInitialized()
        {
            OnConstraintValueChange();
        }

        public void ConstraintSelection(PieMenuController pieMenu, bool selectionConstrained)
        {
            pieMenu.PieMenuModel.SetSelectionConstraintState(selectionConstrained);

            MenuItemSelector selector = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            selector.ToggleSelectionConstraint(selectionConstrained);

            if (!selectionConstrained) {
                return;
            }

            float maxDistance = CalculateConstraintMaxDistance(pieMenu);
            selector.SetConstraintMaxDistance((int) maxDistance);
        }

        public float CalculateConstraintMaxDistance(PieMenuController pieMenu)
        {
            return pieMenu.PieMenuModel.MenuItemSize * 0.10f;
        }

        public void HandleShapeChange(PieMenuController pieMenu, int shapeIndex)
        {
            foreach (KeyValuePair<int, PieMenuItemController> menuItem in pieMenu.GetMenuItems()) {
                ChangeShape(menuItem.Value.transform, shapeIndex);
            }
        }

        private void ChangeShape(Transform menuItem, int shapeIndex)
        {
            ImageFilledClickableSlices menuItemImage = menuItem.GetComponent<ImageFilledClickableSlices>();
            menuItemImage.sprite = Shapes[shapeIndex];
        }

        public void CreateDropdownShapesList(int list)
        {
            ShapeDropdownListIndex = list;
        }

        private void SetCurrentlyUsedShape()
        {
            Sprite sprite = _pieMenuController.MenuItemControllerTemplate.GetComponent<ImageFilledClickableSlices>().sprite;
            int index = Shapes.IndexOf(sprite);
            if (index != -1) {
                CreateDropdownShapesList(index);
            }
        }

        public List<Sprite> Shapes => _shapes;
    }
}