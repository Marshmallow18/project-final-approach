using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

class Player : Sprite
{
    private Sprite _fog1;
    private AnimationSprite _animation, _fog2;
    private int _timer, _timer2, _timer3, _frameUp = 0, _frameLeft = 4, _frameDown = 8, _frameRight = 12, _state;
    private float _vSpeed, _hSpeed, _deceleration, _scale = 1.0f;
    private bool _check_walk = false;
    private Random _rand = new Random();
    public Vector2 lastPos;
    public Player() : base("colors.png")
    {
        alpha = 0.0f;

        _animation = new AnimationSprite("player_animation_sprite.png", 16, 1);
        AddChild(_animation);

        _fog1 = new Sprite("fog.png");
        AddChild(_fog1);

        _fog1.x = -_fog1.width / 2;
        _fog1.y = -_fog1.height / 2;

        _fog2 = new AnimationSprite("anim_fog.png", 2, 1);
        AddChild(_fog2);

        

        SetOrigin(width / 2, height / 2);
        _animation.SetOrigin(_animation.width / 2, _animation.height / 2);

        _deceleration = 0.9f;
        _state = 0; //state is used for knowing the direction: WASD = 1234
        _rand = new Random();
    }

    void Update()
    {
        Movement();
        Animation();
        //LampFlicker();
        LampReduceLight();

        _fog2.x = -_fog2.width / 2 - 10;
        _fog2.y = -_fog2.height / 2 + 10;
    }

    void Movement()
    {
        if (Input.GetKey(Key.W))
        {
            _state = 1;
            _vSpeed = -2.0f;
        }
        

        if (Input.GetKey(Key.S))
        {
            _state = 3;
            _vSpeed = 2.0f;
        }
        

        if (Input.GetKey(Key.A))
        {
            _state = 2;
            _hSpeed = -2.0f;
        }
        

        if (Input.GetKey(Key.D))
        {
            _state = 4;           
            _hSpeed = 2.0f;
        }
        

        if (Input.GetKeyUp(Key.W) || Input.GetKeyUp(Key.A) || Input.GetKeyUp(Key.S) || Input.GetKeyUp(Key.D))
        {
            _state = 0;
        }

        lastPos = Position;

        Move(0, _vSpeed);
        _vSpeed *= _deceleration;

        Move(_hSpeed, 0);
        _hSpeed *= _deceleration;
    }

    void Animation()
    {
        if (_state == 0)
        {
            _animation.SetFrame(_frameDown);
        }

        if (_state == 1)
        {
                 _animation.SetFrame(_frameUp);

            _timer++;
            if (_timer > 10)
            {

                _frameUp++;
                _timer = 0;
            }
            if (_frameUp > 3)
            {
                _frameUp = 0;
            }
        }
        else
        {
            _frameUp = 0;
        }

        if (_state == 2)
        {
                _animation.SetFrame(_frameLeft);

            _timer++;

            if (_timer > 10)
            {
                _frameLeft++;
                _timer = 0;
            }
            if (_frameLeft > 7)
            {
                _frameLeft = 4;
            }
        }
        else
        {
            _frameLeft = 4;
        }

        if (_state == 3)
        {
                _animation.SetFrame(_frameDown);

            _timer++;

            if (_timer > 10)
            {
                _frameDown++;
                _timer = 0;
            }
            if (_frameDown > 11)
            {
                _frameDown = 8;
            }
        }
        else
        {
            _frameDown = 8;
        }

        if (_state == 4)
        {
                _animation.SetFrame(_frameRight);

            _timer++;

            if (_timer > 10)
            {
                _frameRight++;
                _timer = 0;
            }
            if (_frameRight > 15)
            {
                _frameRight = 12;
            }
        }
        else
        {
            _frameRight = 12;
        }

        
    }
    void LampFlicker()
    {
        _timer2++;
        if (_timer2 > 60)
        {
            if (_rand.Next(10) == 1)
            {
                _fog2.SetFrame(1);
            }
            if (_rand.Next(2) == 1)
            {
                _fog2.SetFrame(0);
            }
            _timer2 = 0;
        }
    }

    void LampReduceLight()
    {
        _timer3++;
        if(_timer3 > 120)
        {
            _scale -= 0.05f;
            if (_scale > 0.5f)
            {
                _fog2.SetScaleXY(_scale, _scale);
            }
            else
            {
                _fog2.NextFrame();
            }
            _timer3 = 0;
        }
    }
}
