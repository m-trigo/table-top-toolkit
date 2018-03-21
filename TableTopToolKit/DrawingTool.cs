using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TableTopToolKit
{
    internal interface DrawingTool
    {
        void MouseMove(Point position, MouseEventArgs mouseEvent);

        void MouseUp(Point mousePosition, MouseEventArgs mouseEvent);

        void MouseDown(Point mousePosition, MouseEventArgs mouseEvent);

        void MouseExit(Point mousePosition, MouseEventArgs mouseEvent);
    }
}