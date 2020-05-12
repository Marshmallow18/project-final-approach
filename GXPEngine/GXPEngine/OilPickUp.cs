using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.HUD;

namespace GXPEngine
{
    class OilPickUp : AnimationSprite
    {
        public int max;

        private string _oilImageFileName = "";

        private int _oilType;

        private int _numType;

        private bool _working = false;

        private Random _rand = new Random();

        public OilPickUp(int newNumType, int newOilType, string poilImageFileName, string filename, int cols, int rows, int frames = -1, bool keepInCache = false,
            bool addCollider = true) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
            _oilImageFileName = poilImageFileName;

            DrawableTweener.Blink(this, 1, 0.5f, 600);

            max = 100;

            _oilType = newOilType;

            _numType = newNumType;

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

        public void RandomizeSpawn(int value)
        {
            if(value == 1)
            {
                if(_rand.Next(1, 2) == _numType)
                {
                    _working = true;
                }
            }

            if(value == 2)
            {
                if (_rand.Next(1, 2) == _numType)
                {
                    _working = true;
                }
            }

            if (value == 3)
            {
                if (_rand.Next(1, 4) == _numType)
                {
                    _working = true;
                }
            }
        }

        public void OnCollision(GameObject any)
        {
            if(any is Player)
            {
                ((MyGame)game).SetOil(max);
                //_working = false;
            }
        }
    }

}

