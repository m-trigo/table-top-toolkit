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

        private bool drawingCornerSnappinLine;
        private bool drawingGridSnappingLine;

        private Grid grid;

        public SnapLineTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;
            drawingCornerSnappinLine = false;
            drawingGridSnappingLine = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        //drawingCornerSnappinLine = false;
                        if (!drawingGridSnappingLine && !drawingCornerSnappinLine)
                        {
                            drawingGridSnappingLine = true;

                            currentLine = new Line();
                            currentLine.Stroke = Brushes.Red;
                            Point beginning = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                            currentLine.X1 = beginning.X;
                            currentLine.Y1 = beginning.Y;

                            source.AddSimpleDrawing(currentLine);
                        }

                        currentLine.Stroke = Brushes.Red;
                        Point snapped = grid.SnapToGridLines(new Point(currentLine.X1, currentLine.Y1), new Point(mousePosition.X, mousePosition.Y));
                        currentLine.X2 = snapped.X;
                        currentLine.Y2 = snapped.Y;
                    }
                    else
                    {
                        if (!drawingCornerSnappinLine && !drawingGridSnappingLine) {
                            drawingCornerSnappinLine = true;

                            currentLine = new Line();

                            currentLine.Stroke = Brushes.Blue;
                            Point beginning = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                            currentLine.X1 = beginning.X;
                            currentLine.Y1 = beginning.Y;

                            source.AddSimpleDrawing(currentLine);
                        }

                        currentLine.Stroke = Brushes.Blue;
                        Point snapped = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
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
            if (drawingCornerSnappinLine)
            {
                drawingCornerSnappinLine = false;
            }
            if (drawingGridSnappingLine)
            {
                drawingGridSnappingLine = false;
            }
        }
    }
}