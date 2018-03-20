using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    class EraserTool : DrawingTool {
        private CanvasDrawings source;

        private Point lastKnownMouseDown;
        private bool mouseDown;

        private Line placeholderRedLine;

        private bool drawingCornerSnappinLine;
        private bool drawingGridSnappingLine;

        private Grid grid;

        public EraserTool(CanvasDrawings canvasDrawings, Grid grid)
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
                    if (!drawingGridSnappingLine && !drawingCornerSnappinLine)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            drawingGridSnappingLine = true;
                        }
                        else
                        {
                            drawingCornerSnappinLine = true;
                        }

                        placeholderRedLine = new Line();
                        placeholderRedLine.Stroke = Brushes.Red;
                        Point beginning = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                        placeholderRedLine.X1 = beginning.X;
                        placeholderRedLine.Y1 = beginning.Y;

                        source.StartDrawing(placeholderRedLine);
                    }

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        Point snapped = grid.SnapToGridLines(new Point(placeholderRedLine.X1, placeholderRedLine.Y1), new Point(mousePosition.X, mousePosition.Y));
                        placeholderRedLine.X2 = snapped.X;
                        placeholderRedLine.Y2 = snapped.Y;
                    }
                    else
                    {
                        Point snapped = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                        placeholderRedLine.X2 = snapped.X;
                        placeholderRedLine.Y2 = snapped.Y;
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

            if (placeholderRedLine != null)
            {
                source.Erase(placeholderRedLine);
            }
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }
    }
}
