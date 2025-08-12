using UnityEngine;

namespace Game.PieMenu.UI.Model
{
    public class PieMenuModel
    {
        public bool IsActive { get; private set; }
        public bool SelectionConstrained { get; private set; }
        public int Rotation { get; private set; }
        public float Scale { get; private set; }
        public Vector2 AnchoredPosition { get; private set; }

        public bool InfoPanelEnabled { get; private set; }
        public float MenuItemFillAmount { get; private set; }
        public float MenuItemDegrees { get; private set; }
        public int MenuItemSpacing { get; private set; }
        public int MenuItemPreservedSpacing { get; private set; } = -1;
        public int MenuItemInitialSize { get; private set; }
        public int MenuItemSize { get; private set; }

        public AnimationClip? Animation { get; private set; }
        public AudioClip? MouseHover { get; private set; }
        public AudioClip? MouseClick { get; private set; }

        public void SetStartValues()
        {
            IsActive = true;
            
        }

        public void SetActiveState(bool isActive)
        {
            IsActive = isActive;
        }
        
        public void SetSelectionConstraintState(bool selectionConstrained)
        {
            SelectionConstrained = selectionConstrained;
        }
        
        public void SetRotation(int rotation)
        {
            Rotation = rotation;
        }
        
        public void SetScale(float scale)
        {
            Scale = scale;
        }
        
        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            AnchoredPosition = anchoredPosition;
        }
        
        public void SetInfoPanelEnabled(bool isEnabled)
        {
            InfoPanelEnabled = isEnabled;
        }
        
        public void SetFillAmount(float fillAmount)
        {
            MenuItemFillAmount = fillAmount;
        }
        
        public void SetMenuItemAngle(float angle)
        {
            MenuItemDegrees = angle;
        }
        
        public void SetSpacing(int spacing)
        {
            MenuItemSpacing = spacing;
        }
        
        public void SetPreservedSpacing(int spacingToPreserve)
        {
            MenuItemPreservedSpacing = spacingToPreserve;
        }
        
        public void SetMenuItemInitialSize(int size)
        {
            MenuItemInitialSize = size;
        }
        
        public void SetMenuItemSize(int size)
        {
            MenuItemSize = size;
        }
        
        public void SetAnimation(AnimationClip animationClip)
        {
            Animation = animationClip;
        }
        
        public void SetMouseHoverClip(AudioClip audioClip)
        {
            MouseHover = audioClip;
        }
        
        public void SetMouseClickClip(AudioClip audioClip)
        {
            MouseClick = audioClip;
        }
    }
}