using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class RectangleTool : DrawingTool
    {
        private CanvasDrawings source;
        private Grid grid;

        private Point lastKnownMouseDown;
        private bool mouseDown;

        private bool keepDrawing;

        private Line left;
        private Line top;
        private Line right;
        private Line bottom;

        public RectangleTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;

            keepDrawing = false;

            top = null;
            left = null;
            bottom = null;
            right = null;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                mouseDown = true;
            }
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            keepDrawing = false;
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
                {
                    if (!keepDrawing)
                    {
                        keepDrawing = true;

                        Point startingPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                        Point endPoint = startingPoint;

                        left = new Line()
                        {
                            X1 = startingPoint.X,
                            Y1 = startingPoint.Y,
                            X2 = startingPoint.X,
                            Y2 = endPoint.Y,
                            Stroke = source.ForegroundColor,
                            StrokeThickness = source.ForegroundThickness
                        };
                        top = new Line()
                        {
                            X1 = startingPoint.X,
                            Y1 = startingPoint.Y,
                            X2 = endPoint.X,
                            Y2 = startingPoint.Y,
                            Stroke = source.ForegroundColor,
                            StrokeThickness = source.ForegroundThickness
                        };
                        right = new Line()
                        {
                            X1 = endPoint.X,
                            Y1 = endPoint.Y,
                            X2 = endPoint.X,
                            Y2 = startingPoint.Y,
                            Stroke = source.ForegroundColor,
                            StrokeThickness = source.ForegroundThickness
                        };
                        bottom = new Line()
                        {
                            X1 = endPoint.X,
                            Y1 = endPoint.Y,
                            X2 = startingPoint.X,
                            Y2 = endPoint.Y,
                            Stroke = source.ForegroundColor,
                            StrokeThickness = source.ForegroundThickness
                        };

                        Drawing rectangleDrawing = new Drawing(left);
                        rectangleDrawing.AddToDrawing(new UIElement[] { top, right, bottom });
                        source.AddDrawing(rectangleDrawing);
                    }

                    Point currentPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);

                    double width = currentPoint.X - left.X2;
                    double height = currentPoint.Y - top.Y2;

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        height = Math.Abs(width);
                    }

                    top.X2 = right.X1 = right.X2 = bottom.X2 = top.X1 + width;
                    left.Y2 = bottom.Y1 = right.Y2 = bottom.Y2 = top.Y1 + height;
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

        public void Close()
        {
        }
    }
}