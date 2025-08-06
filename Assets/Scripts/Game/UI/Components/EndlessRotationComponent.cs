using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.UI.Components
{
    public class EndlessRotationComponent : MonoBehaviour
    {
        private void Start()
        {
            transform.DORotate(new(0f, 360f, 360f), 1f, RotateMode.FastBeyond360)
                     .SetLoops(-1, LoopType.Restart)
                     .SetEase(Ease.Linear)
                     .AsyncWaitForCompletion()
                     .AsUniTask()
                     .AttachExternalCancellation(destroyCancellationToken)
                     .Forget();
        }
    }
}