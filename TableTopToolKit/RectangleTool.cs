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
    internal class RectangleTool : DrawingTool
    {
        private CanvasDrawings source;
        private Grid grid;

        private Point lastKnownMouseDown;
        private bool mouseDown;
        private bool continueDrawing;

        private Line startVertical;
        private Line startHorizontal;
        private Line endVertical;
        private Line endHorizontal;

        public RectangleTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;
            continueDrawing = false;
            startHorizontal = null;
            startVertical = null;
            endHorizontal = null;
            endVertical = null;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            lastKnownMouseDown = mousePosition;
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                mouseDown = true;
            }
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Released)
            {
                mouseDown = false;
                continueDrawing = false;
            }
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
            mouseDown = false;
            continueDrawing = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
            {
                Point startingPoint = grid.SnapToGridCorners(lastKnownMouseDown.X, lastKnownMouseDown.Y);
                Point endPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);

                double dx = endPoint.X - startingPoint.X;
                double dy = endPoint.Y - startingPoint.Y;

                if (continueDrawing)
                {
                    startVertical.Y2 = endPoint.Y;
                    startHorizontal.X2 = endPoint.X;
                    endVertical.X1 = endPoint.X;
                    endVertical.Y1 = endPoint.Y;
                    endVertical.X2 = endPoint.X;
                    endHorizontal.X1 = endPoint.X;
                    endHorizontal.Y1 = endPoint.Y;
                    endHorizontal.Y2 = endPoint.Y;
                }
                else
                {
                    startVertical = new Line() { X1 = startingPoint.X, Y1 = startingPoint.Y, X2 = startingPoint.X, Y2 = endPoint.Y }; // left
                    startHorizontal = new Line() { X1 = startingPoint.X, Y1 = startingPoint.Y, X2 = endPoint.X, Y2 = startingPoint.Y }; // top
                    endVertical = new Line() { X1 = endPoint.X, Y1 = endPoint.Y, X2 = endPoint.X, Y2 = startingPoint.Y }; // right
                    endHorizontal = new Line() { X1 = endPoint.X, Y1 = endPoint.Y, X2 = startingPoint.X, Y2 = endPoint.Y }; // bottom

                    source.StartDrawing(startVertical);
                    source.ContinueDrawing(startHorizontal);
                    source.ContinueDrawing(endVertical);
                    source.ContinueDrawing(endHorizontal);
                    continueDrawing = true;
                }
            }
            lastKnownMouseDown = mousePosition;
        }
    }
}