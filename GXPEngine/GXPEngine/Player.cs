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
    private int _timer, _timer2, _frameUp , _frameLeft, _frameDown, _frameRight, _state;
    private float _vSpeed, _hSpeed, _deceleration;
    private Random _rand;
    private float _currentSpeed;
    private float _walkSpeed = 2f;
    private float _runSpeed = 10f;
    private bool _isRunning = false, _lampOpen = true;
    private bool _inputEnabled;
    public Vector2 lastPos;
    public float oil = 100;
    
    public GameObject[] objectsToCheck;

    
    public Player(bool pInputEnabled = true) : base("player_base_sprite.png")
    {
        objectsToCheck = new GameObject[0];
        _inputEnabled = pInputEnabled;
        
        alpha = 0.0f;

        _animation = new AnimationSprite("player_animation_sprite.png", 16, 1, -1, false, false);
        AddChild(_animation);

        _fog1 = new Sprite("fog.png");
        AddChild(_fog1);
        _fog1.x = -_fog1.width / 2;
        _fog1.y = -_fog1.height / 2;
        _fog2 = new AnimationSprite("anim_fog.png", 2, 1);
        AddChild(_fog2);
        _animation.SetOrigin(_animation.width / 2, 78);
        _deceleration = 0.9f;
        _state = 0; //state is used for knowing the direction: WASD = 1234
        _currentSpeed = _walkSpeed;
        _rand = new Random();
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
                _state = 1;
                _vSpeed = -_currentSpeed;
            }
        

            if (Input.GetKey(Key.S))
            {
                _state = 3;
                _vSpeed = _currentSpeed;
            }
        

            if (Input.GetKey(Key.A))
            {
                _state = 2;
                _hSpeed = -_currentSpeed;
            }
        

            if (Input.GetKey(Key.D))
            {
                _state = 4;           
                _hSpeed = _currentSpeed;
            }
        

            if (Input.GetKeyUp(Key.W) || Input.GetKeyUp(Key.A) || Input.GetKeyUp(Key.S) || Input.GetKeyUp(Key.D))
            {
                _state = 0;
            }
        }
        else
        {
            _state = 0;
        }

        lastPos = Position;
        
        _vSpeed *= _deceleration;
        _hSpeed *= _deceleration;
        
        MoveUntilCollision(_hSpeed, _vSpeed, objectsToCheck);
    }

    public void Animation()
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