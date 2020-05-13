using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Components;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class FollowCamera : MCamera
    {
        public float speed = 15f;
        public float interpVelocity;
        public float MinDistance;
        public float TargetFrontDistance = 300f;
        private GameObject _target;
        public Vector2 Offset;
        private Vector2 _targetPos;

        private bool _hasSpeedListener;
        private IHasSpeed _iHasSpeed;

        private bool _followEnabled = true;

        // How long the object should shake for.
        public float shakeDuration = 0f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float shakeAmount = 1.7f;
        public float decreaseFactor = 1.0f;
        private MapGameObject _map;

        public FollowCamera(int windowX, int windowY, int windowWidth, int windowHeight, MapGameObject pMap = null) :
            base(windowX, windowY,
                windowWidth, windowHeight)
        {
            _map = pMap;

            CoroutineManager.StartCoroutine(WaitForMapAssign(), game);

            Enabled = false;
        }

        private IEnumerator WaitForMapAssign()
        {
            while (_map == null)
            {
                yield return null;
            }

            Start();
        }

        public void Start()
        {
            _targetPos = Position;
            Enabled = true;
        }


        void Update()
        {
            if (!Enabled) return;

            Vector2 pos = new Vector2(this.x, this.y);

            if (_followEnabled && _target != null)
            {
                Vector2 targetP = new Vector2(_target.x, _target.y);

                if (_hasSpeedListener)
                {
                    //Target point is in front of target, set offset
                    float dist = Mathf.Map(_iHasSpeed.Speed, 0, _iHasSpeed.MaxSpeed, 0, TargetFrontDistance);

                    //TODO: make IHasSpeed has a Vector2 _forward
                    float xDir = Mathf.Cos(_target.rotation.DegToRad());
                    float yDir = Mathf.Sin(_target.rotation.DegToRad());

                    targetP = targetP + (new Vector2(xDir, yDir)) * dist;
                }

                Vector2 targetDirection = (targetP - pos);
                float directionMag = targetDirection.Magnitude;

                if (directionMag > 0)
                {
                    interpVelocity = directionMag * speed;

                    float lastX = _targetPos.x;
                    float lastY = _targetPos.y;

                    _targetPos = pos + (targetDirection.Normalized * interpVelocity * Time.delta);

                    // if (_targetPos.x - MyGame.HALF_SCREEN_WIDTH < 0 || _targetPos.x + MyGame.HALF_SCREEN_WIDTH > _map.TotalWidth)
                    // {
                    //     _targetPos.x = lastX;
                    // }
                    //
                    // if (_targetPos.y - MyGame.HALF_SCREEN_HEIGHT < 0 || _targetPos.y + MyGame.HALF_SCREEN_HEIGHT > _map.TotalHeight)
                    // {
                    //     _targetPos.y = lastY;
                    // }

                    if (_targetPos.x < MyGame.HALF_SCREEN_WIDTH || _targetPos.x > _map.TotalWidth)
                    {
                        _targetPos.x = lastX;
                    }

                    if (_targetPos.y < MyGame.HALF_SCREEN_HEIGHT || _targetPos.y > _map.TotalHeight)
                    {
                        _targetPos.y = lastY;
                    }

                    //var nextPos = Vector2.Lerp(pos, targetPos + offset, 0.25f);
                    // float nextX = Easing.Ease(Easing.Equation.CubicEaseOut, 1, pos.x, _targetPos.x, 4);
                    // float nextY = Easing.Ease(Easing.Equation.CubicEaseOut, 1, pos.y, _targetPos.y, 4);

                    float nextX = pos.x + Easing.Ease(Easing.Equation.CubicEaseOut, 1, 0, _targetPos.x - pos.x, 4);
                    float nextY = pos.y + Easing.Ease(Easing.Equation.CubicEaseOut, 1, 0, _targetPos.y - pos.y, 4);

                    this.SetXY(nextX, nextY);
                }
            }

            pos.x = x;
            pos.y = y;

            if (shakeDuration > 0)
            {
                var shakePos = pos + MRandom.InsideUnitCircle() * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;

                SetXY(shakePos.x, shakePos.y);
            }
            else
            {
                shakeDuration = 0f;
                SetXY(pos.x, pos.y);
            }
        }

        public GameObject Target
        {
            get => _target;
            set
            {
                this.HasSpeed = value as IHasSpeed;
                _target = value;
            }
        }

        public IHasSpeed HasSpeed
        {
            get => _iHasSpeed;
            set
            {
                _hasSpeedListener = value != null;
                _iHasSpeed = value;
            }
        }

        public bool FollowEnabled
        {
            get => _followEnabled;
            set => _followEnabled = value;
        }

        public MapGameObject Map
        {
            get => _map;
            set => _map = value;
        }
    }
}