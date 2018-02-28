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
    internal class SnapLineTool : DrawingTool
    {
        private CanvasDrawings source;

        private Point lastKnownMouseDown;
        private bool mouseDown;

        private Line currentLine;
        private bool drawingStraightLine;

        private Grid grid;

        public SnapLineTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;
            drawingStraightLine = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (!drawingStraightLine)
                        {
                            drawingStraightLine = true;

                            currentLine = new Line();
                            Point beginning = grid.SnapToGrid(mousePosition.X, mousePosition.Y);
                            currentLine.X1 = beginning.X;
                            currentLine.Y1 = beginning.Y;

                            source.AddSimpleDrawing(currentLine);
                        }

                        Point snapped = grid.SnapToGrid(mousePosition.X, mousePosition.Y);
                        currentLine.X2 = snapped.X;
                        currentLine.Y2 = snapped.Y;
                    }
                }

                if (!mouseDown)
                {
                    mouseDown = true;
                }
            }
            else
            {
                mouseDown = false;
            }

            lastKnownMouseDown = mousePosition;
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (drawingStraightLine)
            {
                drawingStraightLine = false;
            }
        }
    }
}