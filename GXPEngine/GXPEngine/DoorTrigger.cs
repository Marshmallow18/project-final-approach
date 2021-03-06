﻿using System;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.HUD;



namespace GXPEngine
{
    public class DoorTrigger : Sprite
    {
        private Door _door;
        private bool isOnCollisionWith;
        private GameObject otherInCollision;

        private bool _disableAfterHit;

        private bool _isDarkTrigger;
        
        public DoorTrigger(Door pDoor, bool pIsDarkTrigger, bool pDisableAfterHit = true, string fileName = "data/Door Trigger Helper.png", bool addCollider = true) : base(fileName, addCollider)
        {
            _door = pDoor;
            _disableAfterHit = pDisableAfterHit;
            _isDarkTrigger = pIsDarkTrigger;

            if (_isDarkTrigger)

                _door.visible = false;
        }

        void OnCollision(GameObject other)
        {
            if (isOnCollisionWith == false && other is Player)
            {
                isOnCollisionWith = true;
                otherInCollision = other;

                TriggerEnter();

                _collider.Enabled = _isDarkTrigger || !_disableAfterHit;
            }
        }

        private void TriggerEnter()
        {
            if (_isDarkTrigger)

            {

                if (((MyGame)game).GetOil() > 65)

                {

                    OpenDoor();

                    ((MyGame)game).StopOil();

                }

                else

                {

                    GameHud.Instance.ShowTextBox("This tunnel is too dark to enter, I should refill my oil lamp.", 500, 60, 0, 0, true);

                }
            }
            else

            {

                OpenDoor();
                GameSoundManager.Instance.PlayFx(Settings.Door0_Open_Sound, Settings.SFX_Default_Volume);

            }
        }

        private void TriggerExit()
        {
            _door.Close();
            ((MyGame)game).StartOil();
        }



        private void OpenDoor()
        {
            _door.Open();
        }

        void Update()
        {
            if (!Enabled) return;

            if (isOnCollisionWith && otherInCollision != null && HitTest(otherInCollision) == false)
            {
                isOnCollisionWith = false;
                TriggerExit();
            }



            alpha = MyGame.Debug ? 0.5f : 0;
        }
    }
}