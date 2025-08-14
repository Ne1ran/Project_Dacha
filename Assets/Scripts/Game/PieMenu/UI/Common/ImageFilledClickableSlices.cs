using UnityEngine;
using UnityEngine.UI;

// It calculates the area of the image that is not visible due to fillAmount settings.
namespace Game.PieMenu.UI.Common
{
    public class ImageFilledClickableSlices : Image
    {
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            bool result = base.IsRaycastLocationValid(screenPoint, eventCamera);
            if (!result) {
                return false;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 localPoint);

            float clickAngle = Vector2.SignedAngle(localPoint, Vector2.left);
            return (clickAngle >= 0) && (clickAngle < (360f * fillAmount));
        }
    }
}