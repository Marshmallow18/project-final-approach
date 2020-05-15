using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{

    public class DarkHall : Sprite
    {

        public DarkHall(string fileName , bool addCollider = false) : base(fileName, addCollider)
        {
            Console.WriteLine("YES");
        }
    }
}
