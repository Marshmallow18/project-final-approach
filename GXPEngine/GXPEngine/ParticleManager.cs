using System;
using System.Collections;
using GXPEngine.Core;

namespace GXPEngine
{
    /// <summary>
    /// Imported from previous project Team 23 - Storks - 2020 (Leao Victor - CMGT ECM1V.ec)
    /// </summary>
    public class ParticleManager : GameObject
    {
        public static ParticleManager Instance;

        private AnimationSprite _smoke00;
        private AnimationSprite _smallBlackSmoke00;

        private AnimationSprite _cartoonCoinsExplosion;
        private IEnumerator _cartoonCoinsExplosionRoutine;

        private SmallSnowFlakeParticle[] _smallSnowFlakes;
        private IEnumerator _smallSnowFlakesRoutine;

        private SmallSnowFlakeParticle[] _smallSnowFlakes2;
        private IEnumerator _smallSnowFlakesRoutine2;

        public ParticleManager()
        {
            Instance = this;

            _smoke00 = new AnimationSprite("data/Smoke Particle 00.png", 4, 2, 7, true, false);
            _smoke00.SetOriginToCenter();

            _smallBlackSmoke00 = new AnimationSprite("data/Small Black Smokes00.png", 5, 5, 10, true, false);
            _smallBlackSmoke00.SetOriginToCenter();

            _cartoonCoinsExplosion =
                new AnimationSprite("data/cartoon coin explosion_image.png", 8, 4, -1, false, false);
            _cartoonCoinsExplosion.SetOriginToCenter();
            _cartoonCoinsExplosion.SetActive(false);

            _smallSnowFlakes = new SmallSnowFlakeParticle[20];
            for (int i = 0; i < _smallSnowFlakes.Length; i++)
            {
                var snow = new SmallSnowFlakeParticle();
                _smallSnowFlakes[i] = snow;
            }

            _smallSnowFlakes2 = new SmallSnowFlakeParticle[20];
            for (int i = 0; i < _smallSnowFlakes2.Length; i++)
            {
                var snow = new SmallSnowFlakeParticle();
                _smallSnowFlakes2[i] = snow;
            }
        }

        public void PlaySmallSmoke(GameObject parentObj, float px = 0, float py = 0, int duration = 500,
            int depthIndex = -1)
        {
            CoroutineManager.StartCoroutine(PlaySmallSmokeRoutine(parentObj, px, py, duration, depthIndex), this);
        }

        private IEnumerator PlaySmallSmokeRoutine(GameObject parentObj, float px, float py, int duration,
            int depthIndex)
        {
            if (depthIndex < 0)
            {
                parentObj.AddChild(_smallBlackSmoke00);
            }
            else
            {
                parentObj.AddChildAt(_smallBlackSmoke00, depthIndex);
            }

            _smallBlackSmoke00.visible = true;
            _smallBlackSmoke00.SetXY(px, py);

            _smallBlackSmoke00.alpha = 1;

            float time = 0;
            while (time < duration)
            {
                float fFrame = Mathf.Map(time, 0, duration, 0, _smallBlackSmoke00.frameCount - 1);
                int frame = Mathf.Round(fFrame) % _smallBlackSmoke00.frameCount;

                _smallBlackSmoke00.alpha = 1 - Easing.Ease(Easing.Equation.CubicEaseIn, time, 0, 1, duration);

                _smallBlackSmoke00.SetFrame(frame);

                time += Time.deltaTime;
                yield return null;
            }

            _smallBlackSmoke00.visible = false;
            parentObj?.RemoveChild(_smallBlackSmoke00);
        }

        public void PlayCoinsExplosion(GameObject target)
        {
            //CoroutineManager.StopCoroutine(_cartoonCoinsExplosionRoutine);

            //_cartoonCoinsExplosionRoutine =
                CoroutineManager.StartCoroutine(PlayCoinsExplosionRoutine(target, 0, 0), this);
        }

        public void PlayCoinsExplosion(GameObject target, float offSetX, float offSetY)
        {
            CoroutineManager.StopCoroutine(_cartoonCoinsExplosionRoutine);

            _cartoonCoinsExplosionRoutine =
                CoroutineManager.StartCoroutine(PlayCoinsExplosionRoutine(target, offSetX, offSetY), this);
        }

        private IEnumerator PlayCoinsExplosionRoutine(GameObject target, float offSetX, float offSetY)
        {
            int time = 0;
            int duration = 1200;

            yield return null;

            _cartoonCoinsExplosion.SetActive(true);
            target.AddChild(_cartoonCoinsExplosion);
            _cartoonCoinsExplosion.SetXY(0 + offSetX, 0 + offSetY);
            _cartoonCoinsExplosion.alpha = 1f;

            DrawableTweener.TweenSpriteAlpha(_cartoonCoinsExplosion, 1, 0, duration - 500, Easing.Equation.QuadEaseOut,
                500);

            while (time < duration)
            {
                float fFrame = Mathf.Map(time, 0, duration, 0, _cartoonCoinsExplosion.frameCount - 1);
                int frame = Mathf.Round(fFrame) % _cartoonCoinsExplosion.frameCount;

                _cartoonCoinsExplosion.SetFrame(frame);

                time += Time.deltaTime;

                yield return null;
            }

            yield return new WaitForMilliSeconds(200);

            target.RemoveChild(_cartoonCoinsExplosion);
            _cartoonCoinsExplosion.SetActive(false);
        }

        public void SmallSnowFlakesParticles(GameObject target, float range = 30, int duration = 1000)
        {
            _smallSnowFlakesRoutine =
                CoroutineManager.StartCoroutine(SmallSnowFlakesParticlesRoutine(target, range, duration), this);
        }

        IEnumerator SmallSnowFlakesParticlesRoutine(GameObject target, float range, int duration)
        {
            while (target?.parent != null)
            {
                for (int i = 0; i < _smallSnowFlakes.Length; i++)
                {
                    var pos = MRandom.InsideUnitCircle() * range;
                    var snow = _smallSnowFlakes[i];

                    target.parent.AddChild(snow);
                    snow.Show(target, pos, Vector2.zero);

                    yield return new WaitForMilliSeconds(duration / _smallSnowFlakes.Length);
                }
            }
        }

        public void StopSmallSnowFlakesParticles()
        {
            CoroutineManager.StopCoroutine(_smallSnowFlakesRoutine);
        }

        public void SmallSnowFlakesParticles2(GameObject target, Vector2 offset, float range = 30, int duration = 1000)
        {
            _smallSnowFlakesRoutine2 =
                CoroutineManager.StartCoroutine(SmallSnowFlakesParticlesRoutine2(target, offset, range, duration),
                    this);
        }

        IEnumerator SmallSnowFlakesParticlesRoutine2(GameObject target, Vector2 offset, float range, int duration)
        {
            while (target?.parent != null)
            {
                for (int i = 0; i < _smallSnowFlakes2.Length; i++)
                {
                    var pos = MRandom.InsideUnitCircle() * range;
                    var snow = _smallSnowFlakes2[i];

                    target.parent.AddChild(snow);
                    snow.Show(target, pos, offset);

                    yield return new WaitForMilliSeconds(duration / _smallSnowFlakes2.Length);
                }
            }
        }

        public void StopSmallSnowFlakesParticles2()
        {
            CoroutineManager.StopCoroutine(_smallSnowFlakesRoutine2);
        }

        public void Reset()
        {
            _smoke00.parent?.RemoveChild(_smoke00);
            _smallBlackSmoke00.parent?.RemoveChild(_smallBlackSmoke00);
            _cartoonCoinsExplosion.parent?.RemoveChild(_cartoonCoinsExplosion);
        }
    }

    public class SmallSnowFlakeParticle : Sprite
    {
        private GameObject _target;
        private Vector2 _offset;
        private Vector2 _offset2;

        public SmallSnowFlakeParticle() : base("data/small snowflakee.png", true, false)
        {
            SetOriginToCenter();
        }

        public void Show(GameObject target, Vector2 pos, Vector2 offset2)
        {
            _target = target;
            _offset = pos;
            _offset2 = offset2;

            this.SetScaleXY(1f, 1f);
            this.alpha = 1;
            DrawableTweener.TweenScale(this, Vector2.one, Vector2.one * 2, 400, () =>
            {
                parent?.RemoveChild(this);
                _target = null;
            });
        }

        void Update()
        {
            if (!Enabled || _target == null) return;

            SetXY(_target.x + _offset.x + _offset2.x, _target.y + _offset.y + _offset2.y);
        }
    }
}