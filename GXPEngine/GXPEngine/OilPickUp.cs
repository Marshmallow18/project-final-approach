using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.HUD;

namespace GXPEngine
{
    public class OilPickUp : AnimationSprite
    {
        public int max;

        private string _oilImageFileName = "";

        public int _oilType;

        private bool _working = false;

        private Random _rand = new Random();

        
        public OilPickUp( int newOilType, string poilImageFileName, string filename, int cols, int rows, int frames = -1, bool keepInCache = false,
            bool addCollider = true) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
            _oilImageFileName = poilImageFileName;

            DrawableTweener.Blink(this, 1, 0.5f, 600);

            max = 100;

            _oilType = newOilType;


            //RandomizeSpawn(_oilType);
        }

        void Update()
        {
           // if(_working)
           // {
           //     alpha = 1.0f;
           // }
           // else
           // {
           //     alpha = 0.0f;
          //  }
        }

        public void OnCollision(GameObject any)
        {
            if(Enabled && any is Player)
            {
                ((MyGame)game).SetOil(max);

                if (_oilType >= 1 && _oilType<=3)
                {
                    ((MyGame)game).WaitForNextRandomize(this);
                }
                else
                {
                    ((MyGame)game).WaitForNext(this);
                }

                Console.WriteLine($"{this} onCollision with {any}");

                //LateDestroy();
                //_working = false;
            }
        }
    }

}

