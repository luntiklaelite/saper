using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saper
{
    public class kletka
    {
        public bool isBomb, flag, isOpen;
        public int near = 0;
        public kletka()
        {
            isBomb = false;
            flag = false;
            isOpen = false;
        }
    }
}
