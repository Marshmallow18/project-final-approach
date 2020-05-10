using System;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
    public class DoorTrigger : Sprite
    {
        private Door _door;
        private bool isOnCollisionWith;
        private GameObject otherInCollision;
        
        public DoorTrigger(Door pDoor, string fileName = "data/Door Trigger Helper.png", bool addCollider = true) : base(fileName, addCollider)
        {
            _door = pDoor;
        }

        void OnCollision(GameObject other)
        {
            if (isOnCollisionWith == false && other is Player)
            {
                isOnCollisionWith = true;
                otherInCollision = other;

                TriggerEnter();
            }
        }

        private void TriggerEnter()
        {
            OpenDoor();
        }

        private void TriggerExit()
        {
            _door.Close();
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