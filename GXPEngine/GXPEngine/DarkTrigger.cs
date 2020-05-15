using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{

    public class DarkTrigger : Sprite
    {
        private bool isOnCollisionWith;
        private GameObject otherInCollision;

        public DarkTrigger(string fileName = "data/Door Trigger Helper.png", bool addCollider = true) : base(fileName, addCollider)
        {

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
            ((MyGame)game).StopOil();
        }

        private void TriggerExit()
        {
            ((MyGame)game).StartOil();
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
