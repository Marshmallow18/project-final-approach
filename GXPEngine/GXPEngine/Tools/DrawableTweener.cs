﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace GXPEngine
{
    public static class DrawableTweener
    {
        public static void TweenColorAlpha(IHasColor hasColor, float from, float to, int duration, ITweener tweener)
        {
            TweenColorAlpha(hasColor, from, to, duration, Easing.Equation.Linear, 0, tweener);
        }

        public static void TweenColorAlpha(IHasColor hasColor, float from, float to, int duration,
            Easing.Equation easing,
            int delay = 0, ITweener tweener = null)
        {
            CoroutineManager.StartCoroutine(
                TweenColorAlphaRoutine(hasColor, from, to, duration, easing, delay, tweener), null);
        }

        static IEnumerator TweenColorAlphaRoutine(IHasColor hasColor, float from, float to, int duration,
            Easing.Equation easing,
            int delay = 0, ITweener tweener = null)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            float durationF = duration * 0.001f;
            float time = 0;
            hasColor.Alpha = from;
            var childs = hasColor.children;
            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is IHasColor)
                {
                    ((IHasColor) childs[i]).Alpha = from;
                }
            }

            while (time < durationF)
            {
                hasColor.Alpha = Easing.Ease(easing, time, from, to, durationF);

                for (int i = 0; i < childs.Count; i++)
                {
                    if (childs[i] is IHasColor)
                    {
                        ((IHasColor) childs[i]).Alpha = Easing.Ease(easing, time, from, to, durationF);
                    }
                }

                time += Time.deltaTime * 0.001f;

                yield return null;
            }

            hasColor.Alpha = to;
            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is IHasColor)
                {
                    ((IHasColor) childs[i]).Alpha = to;
                }
            }

            if (tweener != null)
            {
                tweener.OnTweenEnd(hasColor);
            }
        }

        public static void TweenSpriteAlpha(Sprite s, float from, float to, int duration, ITweener tweener)
        {
            TweenSpriteAlpha(s, from, to, duration, Easing.Equation.Linear, 0, tweener);
        }

        public static void TweenSpriteAlpha(Sprite s, float from, float to, int duration, Easing.Equation easing,
            int delay = 0, ITweener tweener = null)
        {
            CoroutineManager.StartCoroutine(TweenSpriteAlphaRoutine(s, from, to, duration, easing, delay, tweener), null);
        }

        static IEnumerator TweenSpriteAlphaRoutine(Sprite s, float from, float to, int duration, Easing.Equation easing,
            int delay = 0, ITweener tweener = null)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            float durationF = duration * 0.001f;
            float time = 0;
            s.alpha = from;
            var childs = s.GetChildren();
            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is Sprite)
                {
                    ((Sprite) childs[i]).alpha = from;
                }
            }

            while (time < durationF)
            {
                s.alpha = Easing.Ease(easing, time, from, to,  durationF);
                //Console.WriteLine($"{s.name} - alpha: {s.alpha}");

                for (int i = 0; i < childs.Count; i++)
                {
                    if (childs[i] is Sprite)
                    {
                        ((Sprite) childs[i]).alpha = Easing.Ease(easing, time, from, to, durationF);
                    }
                }

                time += Time.deltaTime * 0.001f;

                yield return null;
            }

            if (tweener != null)
            {
                tweener.OnTweenEnd(s);
            }
        }
    }

    public interface IHasColor
    {
        Color MainColor { get; set; }
        float Alpha { get; set; }
        List<GameObject> children { get; }
    }

    public interface ITweener
    {
        void OnTweenEnd(Object obj);
    }
}