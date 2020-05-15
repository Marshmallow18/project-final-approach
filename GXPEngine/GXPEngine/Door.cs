using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class Door : AnimationSprite
    {
        private Rectangle _customColliderBounds;
        
        private bool _isOpenForever;
        private bool _isOneSided;

        public Door(bool pIsOneSided, string filename, int cols, int rows, int frames = -1, bool keepInCache = false,
            bool addCollider = true) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
            _customColliderBounds = new Rectangle(0, -54 - 20, 128, 20);

            _isOneSided = pIsOneSided;
            SetFrame(_isOneSided ? 2 : 0);
        }

        void Update()
        {
            if (!Enabled)
                return;
        }

        public override void SetOrigin(float pX, float pY)
        {
            _bounds.x = -pX;
            _bounds.y = -pY;
        }

        public override Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            ret[1] = TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            ret[2] = TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            ret[3] = TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);
            return ret;
        }

        public void Open()
        {
            //alpha = 0;
            _collider.Enabled = false;
            
            //DrawableTweener.TweenSpriteAlpha(this, 1, 0, 500, Easing.Equation.QuadEaseOut);
            SetFrame(1);

            GameSoundManager.Instance.PlayFx(Settings.Door0_Open_Sound, Settings.SFX_Default_Volume);
            
            Console.WriteLine($"{this} open");
        }

        public void Close()
        {
            if (!_isOpenForever)
            {
                //alpha = 1;
                _collider.Enabled = true;
                
                //DrawableTweener.TweenSpriteAlpha(this, 0, 1, 500,Easing.Equation.QuadEaseOut);
                SetFrame(_isOneSided ? 2 : 0);
                
                Console.WriteLine($"{this} close");
            }
        }
        
        public bool IsOpenForever
        {
            get => _isOpenForever;
            set => _isOpenForever = value;
        }

        public bool IsOneSided
        {
            get => _isOneSided;
            set => _isOneSided = value;
        }
    }
}