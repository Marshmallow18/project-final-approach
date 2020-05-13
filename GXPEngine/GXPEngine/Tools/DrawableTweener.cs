using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using GXPEngine.Core;

namespace GXPEngine
{
    public static class DrawableTweener
    {
        public delegate void OnFinished();

        public static void TweenColorAlpha(IHasColor hasColor, float from, float to, int duration,
            OnFinished onFinished)
        {
            TweenColorAlpha(hasColor, from, to, duration, Easing.Equation.Linear, 0, onFinished);
        }

        public static void TweenColorAlpha(IHasColor hasColor, float from, float to, int duration,
            Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
        {
            CoroutineManager.StartCoroutine(
                TweenColorAlphaRoutine(hasColor, from, to, duration, easing, delay, onFinished), null);
        }

        static IEnumerator TweenColorAlphaRoutine(IHasColor hasColor, float from, float to, int duration,
            Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
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

            onFinished?.Invoke();
        }

        public static void TweenSpriteAlpha(Sprite s, float from, float to, int duration, Easing.Equation easing = Easing.Equation.QuadEaseOut)
        {
            TweenSpriteAlpha(s, from, to, duration, easing, 0, null);
        }
        
        public static void TweenSpriteAlpha(Sprite s, float from, float to, int duration, OnFinished onFinished)
        {
            TweenSpriteAlpha(s, from, to, duration, Easing.Equation.Linear, 0, onFinished);
        }

        public static void TweenSpriteAlpha(Sprite s, float from, float to, int duration, Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
        {
            CoroutineManager.StartCoroutine(TweenSpriteAlphaRoutine(s, from, to, duration, easing, delay, onFinished),
                s);
        }

        static IEnumerator TweenSpriteAlphaRoutine(Sprite s, float from, float to, int duration, Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            float durationF = duration * 0.001f;
            float time = 0;
            s.alpha = from;
            var childs = s.GetChildrenRecursive();

            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is Sprite)
                {
                    ((Sprite) childs[i]).alpha = from;
                }
            }

            while (time < durationF)
            {
                float easeVal = Easing.Ease(easing, time, 0, 1, durationF);
                float easeValMap = Mathf.Map(easeVal, 0, 1, from, to);
                s.alpha = easeValMap;

                for (int i = 0; i < childs.Count; i++)
                {
                    if (childs[i] is Sprite)
                    {
                        ((Sprite) childs[i]).alpha = easeValMap;
                    }
                }

                time += Time.delta;

                yield return null;
            }

            onFinished?.Invoke();
        }

        public static void Blink(Sprite s, float from, float to, int duration)
        {
            BlinkOut(s, from, to, duration);
        }
        
        static void BlinkIn(Sprite s, float from, float to, int duration)
        {
            DrawableTweener.TweenSpriteAlpha(s, from, to, duration, Easing.Equation.QuadEaseIn, 0, () => { BlinkOut(s, to, from, duration); });
        }

        static void BlinkOut(Sprite s, float from, float to, int duration)
        {
            DrawableTweener.TweenSpriteAlpha(s, from, to, duration, Easing.Equation.QuadEaseIn, 0, () => { BlinkIn(s, to, from, duration); });
        }
        
        public static void TweenScale(GameObject g, Vector2 from, Vector2 to, int duration, OnFinished onFinished)
        {
            TweenScale(g, from, to, duration, Easing.Equation.QuadEaseOut, 0, onFinished);
        }

        public static void TweenScale(GameObject g, Vector2 from, Vector2 to, int duration, Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
        {
            CoroutineManager.StartCoroutine(TweenSpriteScaleRoutine(g, from, to, duration, easing, delay, onFinished),
                g);
        }

        static IEnumerator TweenSpriteScaleRoutine(GameObject g, Vector2 from, Vector2 to, int duration,
            Easing.Equation easing,
            int delay = 0, OnFinished onFinished = null)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            float durationF = duration * 0.001f;
            float time = 0;
            g.SetScaleXY(from.x, from.y);

            while (time < durationF)
            {
                float easeVal = Easing.Ease(easing, time, 0, 1, durationF);

                float easeValMapX = Mathf.Map(easeVal, 0, 1, from.x, to.x);
                float easeValMapY = Mathf.Map(easeVal, 0, 1, from.y, to.y);
                
                float scaleX = easeValMapX;
                float scaleY = easeValMapY;

                g.SetScaleXY(scaleX, scaleY);
                
                time += Time.delta;

                yield return null;
            }

            onFinished?.Invoke();
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