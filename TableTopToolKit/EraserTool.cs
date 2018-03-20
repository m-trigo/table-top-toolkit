using System;
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

        private Line lineBeingErased;
        private Drawing containingDrawing;
        private double slope;

        private bool keepDrawing;

        private Grid grid;

        public EraserTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;
            keepDrawing = false;
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

                        placeholderRedLine = new Line();
                        placeholderRedLine.Stroke = Brushes.Red;

                        object beginningObject = SnapToLine(mousePosition.X, mousePosition.Y);
                        if (beginningObject == null)
                        {
                            return;
                        }
                        Point beginning = (Point)beginningObject;
                        placeholderRedLine.X1 = beginning.X;
                        placeholderRedLine.Y1 = beginning.Y;

                        source.StartDrawing(placeholderRedLine);
                    }

                    Point snapped = SnapToCurrentLine(mousePosition.X, mousePosition.Y);
                    placeholderRedLine.X2 = snapped.X;
                    placeholderRedLine.Y2 = snapped.Y;
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

        public object PointOfLine(double x, double y)
        {
            foreach (Drawing drawing in source.Drawings())
            {
                foreach (Shape shape in drawing.Shapes)
                {
                    Line line = shape as Line;  
                    if (line != null)   // if single straight line or part of rectangle
                    {
                        if (line.X1 <= x && x <= line.X2 && line.Y1 <= y && placeholderRedLine.Y2 <= y)
                        {
                            double deltaY = line.Y2 - line.Y1;
                            double deltaX = line.X2 - line.X1;
                            double angle = Math.Atan(deltaY / deltaX);

                            slope = Math.Tan(angle);
                            lineBeingErased = line;
                            containingDrawing = drawing;

                            return SnapToCurrentLine(x, y);
                        }
                    }
                }
            }

            return null;
        }
        
        public Point SnapToCurrentLine(double x, double y)
        {
            Point snappedToGrid = grid.SnapToGridCorners(x, y);
            // dy = slope*(x - line.X1) 
            // y = line.Y1 + dy;
            double dy = slope * (snappedToGrid.X - lineBeingErased.X1);
            double goodY = lineBeingErased.Y1 + dy;

            return new Point(snappedToGrid.X, goodY);
        }

        public object SnapToLine(double x, double y)
        {
            Point snappedToGrid = grid.SnapToGridCorners(x, y);
            return PointOfLine(snappedToGrid.X, snappedToGrid.Y);
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            keepDrawing = false;

            if (placeholderRedLine != null)
            {
                source.EraseLineFromDrawing(containingDrawing, lineBeingErased, placeholderRedLine);
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
