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
        private double slopeX;
        private double slopeY;

        private bool keepDrawing;

        private Grid grid;

        string log = "";

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
                        Point snappedStartingPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);

                        placeholderRedLine = new Line();
                        placeholderRedLine.Stroke = Brushes.Red;

                        if (!CanSnapToLine(snappedStartingPoint.X, snappedStartingPoint.Y))
                        {
                            return;
                        }

                        snappedStartingPoint = SnapToLine(lineBeingErased, snappedStartingPoint.X, snappedStartingPoint.Y);

                        placeholderRedLine.X1 = placeholderRedLine.X2 = snappedStartingPoint.X;
                        placeholderRedLine.Y1 = placeholderRedLine.Y2 = snappedStartingPoint.Y;

                        source.StartDrawing(placeholderRedLine);
                        keepDrawing = true;
                    }

                    Point snappedContinuingPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                    snappedContinuingPoint = SnapToLine(lineBeingErased, snappedContinuingPoint.X, snappedContinuingPoint.Y);
                    placeholderRedLine.X2 = snappedContinuingPoint.X;
                    placeholderRedLine.Y2 = snappedContinuingPoint.Y;
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

        public bool CanSnapToLine(double x, double y)
        {
            foreach (Drawing drawing in source.Drawings())
            {
                foreach (Shape shape in drawing.Shapes)
                {
                    Line line = shape as Line;  
                    if (line != null)   // if single straight line or part of rectangle
                    {
                        if (line.X1 <= x && x <= line.X2 && line.Y1 <= y && y <= line.Y2) // line going like this: \
                        {
                            double deltaY = line.Y2 - line.Y1;
                            double deltaX = line.X2 - line.X1;
                            double angleX = Math.Atan(deltaY / deltaX);
                            double angleY = Math.Atan(deltaX / deltaY);

                            slopeX = deltaX == 0 ? 1 : Math.Tan(angleX);
                            slopeY = deltaY == 0 ? 1 : Math.Tan(angleY);
                            lineBeingErased = line;
                            containingDrawing = drawing;

                            return true;
                        }
                        else if (line.X2 <= x && x <= line.X1 && line.Y1 <= y && y <= line.Y2) // line going like this: /
                        {
                            double deltaY = line.Y2 - line.Y1;
                            double deltaX = line.X1 - line.X2;
                            double angleX = Math.Atan(deltaY / deltaX);
                            double angleY = Math.Atan(deltaX / deltaY);

                            slopeX = deltaX == 0 ? -1 : Math.Tan(angleX) * -1;
                            slopeY = deltaY == 0 ? -1 : Math.Tan(angleY) * -1;
                            lineBeingErased = line;
                            containingDrawing = drawing;

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        public Point SnapToLine(Line line, double x, double y)
        {
            Point snappedToGrid = new Point(x, y);
            double dy = slopeX * (snappedToGrid.X - line.X1);
            double dx = slopeY * (snappedToGrid.Y - line.Y1);

            if (dx > dy)
            {
                snappedToGrid.Y = line.Y1 + dy;
            }
            else if (dy > dx)
            {
                snappedToGrid.X = line.X1 + dx;
            }

            return new Point(snappedToGrid.X, snappedToGrid.Y);
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (placeholderRedLine != null && containingDrawing != null && lineBeingErased != null)
            {
                SnapLineTool.StandardizeLineDirection(placeholderRedLine);
                source.EraseLineFromDrawing(containingDrawing, lineBeingErased, placeholderRedLine);
            }
            keepDrawing = false;
            placeholderRedLine = null;
            lineBeingErased = null;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }
    }
}
