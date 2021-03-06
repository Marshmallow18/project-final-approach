﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

public class Player : Sprite
{
    private Sprite _fog1;
    private AnimationSprite _animation, _fog2;
    private int _timer, _timer2, _frame;
    private float _vSpeed, _hSpeed, _deceleration, _state, _scale = 0.2f;
    private Random _rand;
    private float _currentSpeed;
    private float _walkSpeed = 2.4f;
    private float _runSpeed = 10f;
    private bool _lampOpen = true;
    private bool _inputEnabled;
    public Vector2 lastPos;
    public float oil = 100;

    public GameObject[] objectsToCheck;

    private Rectangle _customColliderBounds;

    private SoundChannel _step0Channel;
    private SoundChannel _step1Channel;

    public Player(bool pInputEnabled = true) : base("player_base_sprite.png")
    {
        SetOriginToCenter();

        objectsToCheck = new GameObject[0];
        _inputEnabled = pInputEnabled;

        _inputEnabled = pInputEnabled;
        alpha = 0.0f;

        //Adjust a collider, based on a original image of 256x256
        _customColliderBounds = new Rectangle(-95 * 0.5f * _scale, (-95 * 0.5f * _scale), 95 * _scale,
            95 * _scale);

        _animation = new AnimationSprite("walking_animation.png", 8, 3, -1, false, false);
        AddChild(_animation);
        _animation.SetScaleXY(_scale, _scale);
        _animation.SetOriginToCenter();

        _fog1 = new Sprite("fog.png", true, false);
        AddChild(_fog1);
        _fog1.x = -_fog1.width / 2;
        _fog1.y = -_fog1.height / 2;

        _fog2 = new AnimationSprite("anim_fog.png", 2, 1, -1, true, false);
        
        _fog1.scale = 10;

        AddChild(_fog2);
        _fog2.x = x + 1000;

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

        oil = ((MyGame) game).GetOil();

        PlayStepSound();
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

            if (Input.GetKey(Key.W) && Input.GetKey(Key.A))
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
            if (_timer > 4)
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
        if (oil > 50)
        {
            _fog2.SetScaleXY((oil / 100) * 3, (oil / 100) * 3);
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

    public override Vector2[] GetExtents()
    {
        Vector2[] ret = new Vector2[4];
        ret[0] = TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
        ret[1] = TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
        ret[2] = TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
        ret[3] = TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);
        return ret;
    }

    /// <summary>
    /// frames 4 and 13 are the frames with the feet in the front of the animation
    /// </summary>
    void PlayStepSound()
    {
        if (_frame == 4)
        {
            if (_step0Channel == null || !_step0Channel.IsPlaying)
                _step0Channel = GameSoundManager.Instance.PlayFx(Settings.Footstep1_SFX, Settings.Footsteps_Volume);
        }
        else if (_frame == 13)
        {
            if (_step1Channel == null || !_step1Channel.IsPlaying)
                _step1Channel = GameSoundManager.Instance.PlayFx(Settings.Footstep2_SFX, Settings.Footsteps_Volume);
        }
    }

    public bool InputEnabled
    {
        get => _inputEnabled;
        set => _inputEnabled = value;
    }

    public AnimationSprite Fog2 => _fog2;
    public Sprite Fog1 => _fog1;

    public int Frame => _frame;
}