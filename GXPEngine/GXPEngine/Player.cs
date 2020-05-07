using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Player : Sprite
{
    private AnimationSprite _animation;
    private int _timer, _frame, _state;
    private float _vSpeed, _hSpeed, _deceleration;
    public Player() : base("colors.png")
    {
        alpha = 0.0f;

        _animation = new AnimationSprite("player_animation_sprite.png", 16, 1);
        AddChild(_animation);

        SetOrigin(width / 2, height / 2);
        _animation.SetOrigin(_animation.width / 2, _animation.height / 2);

        _deceleration = 0.9f;
        _state = 0; //state is used for knowing the direction: WASD = 1234
    }

    void Update()
    {
        Movement();
        Animation();
    }

    void Movement()
    {
        if (Input.GetKey(Key.W))
        {
            _state = 1;
            _vSpeed = -2.0f;
        }
        if(Input.GetKeyUp(Key.W))
        {
            _state = 0;
        }
        if(Input.GetKeyDown(Key.W))
        {
            _frame = 0;
        }

        if (Input.GetKey(Key.A))
        {
            _state = 2;
            _hSpeed = -2.0f;
        }
        if (Input.GetKeyUp(Key.A))
        {
            _state = 0;
        }
        if (Input.GetKeyDown(Key.A))
        {
            _frame = 4;
        }

        if (Input.GetKey(Key.S))
        {
            _state = 3;
            _vSpeed = 2.0f;
        }
        if (Input.GetKeyUp(Key.S))
        {
            _state = 0;
        }
        if (Input.GetKeyDown(Key.S))
        {
            _frame = 8;
        }

        if (Input.GetKey(Key.D))
        {
            _state = 4;
            _hSpeed = 2.0f;
        }
        if (Input.GetKeyUp(Key.D))
        {
            _state = 0;
        }
        if (Input.GetKeyDown(Key.D))
        {
            _frame = 12;
        }

        Move(0, _vSpeed);
        _vSpeed *= _deceleration;

        Move(_hSpeed, 0);
        _hSpeed *= _deceleration;
    }

    void Animation()
    {
        if (_state == 1)
        {
                 _animation.SetFrame(_frame);

            _timer++;
            if (_timer > 10)
            {
                
                _frame++;
                _timer = 0;
            }
            if (_frame > 3)
            {
                _frame = 0;
            }
        }

        else if (_state == 2)
        {
                _animation.SetFrame(_frame);

            _timer++;

            if (_timer > 10)
            {
                _frame++;
                _timer = 0;
            }
            if (_frame > 7)
            {
                _frame = 4;
            }
        }

        else if (_state == 3)
        {
                _animation.SetFrame(_frame);

            _timer++;

            if (_timer > 10)
            {
                _frame++;
                _timer = 0;
            }
            if (_frame > 11)
            {
                _frame = 8;
            }
        }

        else if (_state == 4)
        {
                _animation.SetFrame(_frame);

            _timer++;

            if (_timer > 10)
            {
                _frame++;
                _timer = 0;
            }
            if (_frame > 15)
            {
                _frame = 12;
            }
        }

        else
        {
            _animation.SetFrame(_frame - _frame % 4);
        }
    }
}
