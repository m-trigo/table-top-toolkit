using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopToolKit
{
    internal class Drawing
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public Drawing(int startIndex)
        {
            StartIndex = startIndex;
            EndIndex = -1;
        }

        public bool IsComplete()
        {
            return EndIndex != -1;
        }
    }
}