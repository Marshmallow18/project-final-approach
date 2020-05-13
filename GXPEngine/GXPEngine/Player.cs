using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Player : Sprite
{
    private Sprite _fog1;
    private AnimationSprite _animation, _fog2;
    private int _timer, _timer2, _frame;
    private float _vSpeed, _hSpeed, _deceleration, _state, _scale = 0.4f;
    private Random _rand;
    private float _currentSpeed;
    private float _walkSpeed = 2f;
    private float _runSpeed = 10f;
    private bool _lampOpen = true;
    private bool _inputEnabled;
    public Vector2 lastPos;
    public float oil = 100;
    
    public GameObject[] objectsToCheck;


    public Player(bool pInputEnabled = true) : base("player base sprite.png")
    
    public Player(bool pInputEnabled = true) : base("player_base_sprite.png")
    {
        objectsToCheck = new GameObject[0];
        _inputEnabled = pInputEnabled;

        _inputEnabled = pInputEnabled;       
        alpha = 0.0f;

        _animation = new AnimationSprite("walking_animation.png", 8, 3, -1, false, false);
        AddChild(_animation);
        _animation.SetScaleXY(_scale, _scale);

        _fog1 = new Sprite("fog.png");
        AddChild(_fog1);
        _fog1.x = -_fog1.width / 2;
        _fog1.y = -_fog1.height / 2;
        _fog2 = new AnimationSprite("anim_fog.png", 2, 1);
        AddChild(_fog2);
        _animation.SetOrigin(_animation.width / 2, 78);

        _deceleration = 0.9f;
        _currentSpeed = _walkSpeed;
        _rand = new Random();
        _frame = 1;
    }

    void Update()
    {
        if (!Enabled)
            return;

        lastPos = Position;

        Movement();
        Animation();
        LampFlicker();
        LampReduceLight();

        _fog2.x = -_fog2.width / 2 - 10;
        _fog2.y = -_fog2.height / 2 + 10;

        oil = ((MyGame)game).GetOil();
    }

    public void Movement()
    {
        if (_inputEnabled)
        {
            if (Input.GetKey(Key.W))
            {
                rotation = 0;
                _vSpeed = -_currentSpeed;
                _state = 1;

            }


            if (Input.GetKey(Key.A))
            {
                rotation = 270;
                _hSpeed = -_currentSpeed;
                _state = 1;
            }

            if (Input.GetKey(Key.S))
            {
                rotation = 180;
                _vSpeed = _currentSpeed;
                _state = 1;
            }        

            if (Input.GetKey(Key.D))
            {
                rotation = 90;
                _hSpeed = _currentSpeed;
                _state = 1;
            }

            if(Input.GetKey(Key.W) && Input.GetKey(Key.A))
            {
                rotation = 315;
            }

            if (Input.GetKey(Key.A) && Input.GetKey(Key.S))
            {
                rotation = 225;
            }

            if (Input.GetKey(Key.S) && Input.GetKey(Key.D))
            {
                rotation = 135;
            }

            if (Input.GetKey(Key.D) && Input.GetKey(Key.W))
            {
                rotation = 45;
            }

            if (Input.GetKeyUp(Key.W) || Input.GetKeyUp(Key.A) || Input.GetKeyUp(Key.S) || Input.GetKeyUp(Key.D))
            {
                _state = 0;
                _frame = 0;
            }
        }
        else
        {
            _state = 0;
            _frame = 0;
        }

        lastPos = Position;

        _vSpeed *= _deceleration;
        _hSpeed *= _deceleration;

        MoveUntilCollision(_hSpeed, _vSpeed, objectsToCheck);
    }

    public void Animation()
    {
        if (_state == 1)
        {
            _animation.SetFrame(_frame);

            _timer++;
            if (_timer > 2)
            {

                _frame++;
                _timer = 0;
            }
            if (_frame > 18)
            {
                _frame = 1;
            }
        }

        if (_state == 0) _animation.SetFrame(0);
    }
    public void LampFlicker()
    {
        if (_lampOpen)
        {
            _timer2++;
            if (_timer2 > 60)
            {
                if (_rand.Next(10) == 1)
                {
                    _fog2.SetFrame(1);
                }
                else
                {
                    _fog2.SetFrame(0);
                }
                _timer2 = 0;
            }
        }
    }

    public void LampReduceLight()
    {

            if (oil > 30)
            {
                _fog2.SetScaleXY(oil/100, oil/100);
                _lampOpen = true;
            }
            else
            {
                _fog2.SetFrame(1);
                _lampOpen = false;
            }
            
    }

    public void EnableRun(bool active)
    {
        _currentSpeed = active ? _runSpeed : _walkSpeed;
    }

    public bool InputEnabled
    {
        get => _inputEnabled;
        set => _inputEnabled = value;
    }
}