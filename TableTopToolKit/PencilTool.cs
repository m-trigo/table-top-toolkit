using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class PencilTool : DrawingTool
    {
        private CanvasDrawings source;
        private Point lastKnownMouseDown;
        private bool mouseDown;
        private bool startDrawing;

        public PencilTool(CanvasDrawings canvasDrawings)
        {
            source = canvasDrawings;
            mouseDown = false;
            startDrawing = false;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                mouseDown = true;
            }
            lastKnownMouseDown = mousePosition;
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Released)
            {
                mouseDown = false;
                startDrawing = false;
            }
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
            mouseDown = false;
            startDrawing = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
            {
                Line lineSegment = new Line();
                lineSegment.X1 = lastKnownMouseDown.X;
                lineSegment.Y1 = lastKnownMouseDown.Y;
                lineSegment.X2 = mousePosition.X;
                lineSegment.Y2 = mousePosition.Y;

                if (!startDrawing)
                {
                    source.StartDrawing(lineSegment);
                    startDrawing = true;
                }
                else
                {
                    source.ContinueDrawing(lineSegment);
                }
            }

            lastKnownMouseDown = mousePosition;
        }
    }
}