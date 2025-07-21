using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace Game.Utils
{
    public static class TweenUtils
    {
        public static TweenerCore<Color, Color, ColorOptions> DOFade(this TMP_Text target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions>? tweener = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            tweener.SetTarget(target);
            return tweener;
        }

        public static UniTask DoFade(this TMP_Text target, float endValue, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            return DOFade(target, endValue, (float) duration.TotalSeconds).AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken: cancellationToken);
        }

        public static async UniTask DoMoveAsync(this Rigidbody rigidbody, Vector3 target, TimeSpan duration)
        {
            Vector3 startPosition = rigidbody.transform.position;
            float durationSeconds = (float) duration.TotalSeconds;
            float step = 0.0f;

            while (step < durationSeconds) {
                if (!rigidbody) {
                    return;
                }

                step += Time.fixedDeltaTime;
                float t = step / durationSeconds;
                Vector3 newPosition = Vector3.Lerp(startPosition, target, t);
                rigidbody.MovePosition(newPosition);
                await UniTask.WaitForFixedUpdate();
            }

            if (!rigidbody) {
                return;
            }

            rigidbody.MovePosition(target);
        }

        public static async UniTask DoMoveBezierAsync(this Rigidbody rigidbody, Vector3 targetPosition, float height, TimeSpan duration)
        {
            Vector3 startPosition = rigidbody.transform.position;
            float durationSeconds = (float) duration.TotalSeconds;
            float step = 0.0f;

            while (step < durationSeconds) {
                if (!rigidbody) {
                    return;
                }

                step += Time.fixedDeltaTime;
                float t = step / durationSeconds;

                float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
                float y = MathUtils.Bezier(startPosition.y, height, targetPosition.y, t);
                float z = Mathf.Lerp(startPosition.z, targetPosition.z, t);
                Vector3 newPosition = new(x, y, z);
                rigidbody.MovePosition(newPosition);
                await UniTask.WaitForFixedUpdate();
            }

            if (!rigidbody) {
                return;
            }

            rigidbody.MovePosition(targetPosition);
        }

        public static async UniTask Sequence(UniTask task1, UniTask task2, UniTask task3, UniTask task4, CancellationToken token = default)
        {
            await task1;
            token.ThrowIfCancellationRequested();
            await task2;
            token.ThrowIfCancellationRequested();
            await task3;
            token.ThrowIfCancellationRequested();
            await task4;
            token.ThrowIfCancellationRequested();
        }
    }
}