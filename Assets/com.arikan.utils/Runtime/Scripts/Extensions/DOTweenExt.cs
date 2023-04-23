using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DG.Tweening
{
    public static class DOTweenExt
    {
#if DOTween
        public static Tweener DOCounter(this Text text, int target, float duration)
        {
            if (!int.TryParse(text.text, out var first))
                first = 0;
            return DOTween.To(
            () =>
            {
                return first;
            },
            x =>
            {
                first = x;
                text.text = x.ToString();
            },
            target,
            duration);
        }

        public static Tweener DOCounter(this TMPro.TextMeshProUGUI text, int target, float duration)
        {
            if (!int.TryParse(text.text, out var first))
                first = 0;
            return DOTween.To(
            () =>
            {
                return first;
            },
            x =>
            {
                first = x;
                text.text = x.ToString();
            },
            target,
            duration);
        }

        public static Sequence Jump(this Transform t, Vector3 endValue, float offset, float duration, bool snapping = false)
        {
            return DOTween.Sequence().Append(
                t.DOMove(endValue + Vector3.up * offset, duration / 3, snapping).SetEase(Ease.OutCubic)
            ).Append(
                t.DOMove(endValue, (duration / 3) * 2, snapping).SetEase(Ease.OutBounce)
            );
        }

        public static async UniTask<Tweener> ToUniTask(this Tweener t)
        {
            await UniTask.WaitUntil(() => t == null || !t.active);
            return t;
        }

        public static async UniTask<Sequence> ToUniTask(this Sequence t)
        {
            await UniTask.WaitUntil(() => t == null || !t.active);
            return t;
        }
#endif
    }
}