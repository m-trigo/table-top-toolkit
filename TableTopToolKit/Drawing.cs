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

        public Drawing(int startIndex, int endIndex = -1)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool IsComplete()
        {
            return EndIndex != -1;
        }
    }
}