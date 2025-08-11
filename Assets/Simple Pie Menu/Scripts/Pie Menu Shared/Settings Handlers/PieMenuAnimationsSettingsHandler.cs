using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Settings_Handlers
{
    [ExecuteInEditMode]
    public class PieMenuAnimationsSettingsHandler : MonoBehaviour
    {
        public const string TriggerActiveTrue = "ActiveTrue";
        public const string TriggerActiveFalse = "ActiveFalse";

        [SerializeField] List<AnimatorOverrideController> animatorOverrideControllers;

        public List<AnimatorOverrideController> AnimatorOverrideControllers
        {
            get { return animatorOverrideControllers; }
        }


        public void SwapAnimationControllers(PieMenu pieMenu, int controllerIndex)
        {
            AnimatorOverrideController overrideController = GetAnimator(controllerIndex);

            pieMenu.PieMenuElements.Animator.runtimeAnimatorController = overrideController;

            pieMenu.PieMenuInfo.SetAnimation(GetAnimation(overrideController));
        }

        public AnimatorOverrideController GetAnimator(int controllerIndex)
        {
            return animatorOverrideControllers[controllerIndex];
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
    }
}
