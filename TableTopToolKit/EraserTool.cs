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
        private Line placeholderRedLine;
        private bool drawing;



        //private bool mouseDown;


        //private Line lineBeingErased;
        //private Drawing containingDrawing;
        //private double slopeX;
        //private double slopeY;

        //private bool keepDrawing;

        private Grid grid;

        //string log = "";

        public EraserTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            //mouseDown = false;
            //keepDrawing = false;
            drawing = false;
        }
        
        public Point ClosetPointToCoordiante(double x, double y, Point a, Point b)
        {
            Point o = new Point() { X = x, Y = y };
            return (o - a).LengthSquared < (o - b).LengthSquared ? a : b;
        }

        public Point ClosestInterestPoint(double x, double y)
        {
            Point closest = grid.SnapToGridCorners(x, y); // default to a grid corner because it'll always exist
            foreach (Drawing drawing in source.Drawings())
            {
                foreach(Shape shape in drawing.Shapes)
                {
                    if (!(shape is Line))
                    {
                        continue;
                    }
                    Line line = shape as Line;

                    if (Math.Abs(line.X1 - line.X2) < Double.Epsilon) // vertical line
                    {
                        double startY = line.Y1;
                        double endY = line.Y2;
                        if (endY < startY)
                        {
                            double temp = startY;
                            startY = endY;
                            endY = temp;
                        }

                        for (double lineY = startY; lineY <= endY; lineY += grid.SquareSize)
                        {
                            Point point = new Point() { X = line.X1, Y = lineY };
                            if (ClosetPointToCoordiante(x, y, point, closest).Equals(point))
                            {
                                closest = point;
                            }
                        }
                    }
                    else // non-vertical
                    {
                        Point start = new Point() { X = line.X1, Y = line.Y1 };
                        Point end = new Point() { X = line.X2, Y = line.Y2 };

                        // make start the left-most
                        if (end.X < start.X) 
                        {
                            Point temp = start;
                            start = end;
                            end = temp;
                        }

                        double slope = (end.Y - start.Y) / (end.X - start.X);
                        
                        for(double lineX = start.X; lineX < end.X; lineX += grid.SquareSize)
                        {
                            double lineY = start.Y + slope * (lineX - start.X);
                            Point point = new Point() { X = lineX, Y = lineY };
                            if (ClosetPointToCoordiante(x, y, point, closest).Equals(point))
                            {
                                closest = point;
                            }
                        }

                        if (Math.Abs(line.Y1 - line.Y2) < Double.Epsilon) // [1] slope can't be 0
                        {
                            continue; // horizontal line
                        }

                        // make start the top-most
                        if (end.Y < start.Y)
                        {
                            Point temp = start;
                            start = end;
                            end = temp;
                        }

                        slope = 1 / slope; // safe because of [1]

                        for (double lineY = start.Y; lineY < end.Y; lineY += grid.SquareSize)
                        {
                            double lineX = start.X + slope * (lineY - start.Y);
                            Point point = new Point() { X = lineX, Y = lineY };
                            if (ClosetPointToCoordiante(x, y, point, closest).Equals(point))
                            {
                                closest = point;
                            }
                        }
                    }
                }
            }

            return closest;
        }
        
        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            Vector mouseDragDistance = mousePosition - lastKnownMouseDown;
            double adx = Math.Abs(mouseDragDistance.X);
            double ady = Math.Abs(mouseDragDistance.Y);
            if (mouseEvent.LeftButton == MouseButtonState.Pressed
            && (adx > SystemParameters.MinimumHorizontalDragDistance || ady > SystemParameters.MinimumVerticalDragDistance))
            {
                Point start = ClosestInterestPoint(lastKnownMouseDown.X, lastKnownMouseDown.Y);
                Point end = ClosestInterestPoint(mousePosition.X, mousePosition.Y);

                if (start.Equals(end))
                {
                    return;
                }

                if (!drawing)
                {
                    placeholderRedLine = new Line()
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = Brushes.LightPink,
                        StrokeThickness = 4
                    };
                    source.RenderInCanvas(placeholderRedLine);
                    drawing = true;
                }
                else
                {
                    placeholderRedLine.X1 = start.X;
                    placeholderRedLine.Y1 = start.Y;
                    placeholderRedLine.X2 = end.X;
                    placeholderRedLine.Y2 = end.Y;
                }

            }





            //if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            //{
            //    if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
            //    {
            //        if (!keepDrawing)
            //        {
            //            Point snappedStartingPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);

            //            placeholderRedLine = new Line();
            //            placeholderRedLine.Stroke = Brushes.Red;

            //            if (!CanSnapToLine(snappedStartingPoint.X, snappedStartingPoint.Y))
            //            {
            //                return;
            //            }

            //            snappedStartingPoint = SnapToLine(lineBeingErased, snappedStartingPoint.X, snappedStartingPoint.Y);

            //            placeholderRedLine.X1 = placeholderRedLine.X2 = snappedStartingPoint.X;
            //            placeholderRedLine.Y1 = placeholderRedLine.Y2 = snappedStartingPoint.Y;

            //            source.StartDrawing(placeholderRedLine);
            //            keepDrawing = true;
            //        }

            //        Point snappedContinuingPoint = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
            //        if (!CanKeepSnapping(snappedContinuingPoint.X, snappedContinuingPoint.Y))
            //        {
            //            return;
            //        }

            //        snappedContinuingPoint = SnapToLine(lineBeingErased, snappedContinuingPoint.X, snappedContinuingPoint.Y);
            //        placeholderRedLine.X2 = snappedContinuingPoint.X;
            //        placeholderRedLine.Y2 = snappedContinuingPoint.Y;
            //    }

            //    if (!mouseDown)
            //    {
            //        mouseDown = true;
            //    }
            //}
            //else
            //{
            //    mouseDown = false;
            //}

            //lastKnownMouseDown = mousePosition;
        }

        //public bool CanKeepSnapping(double x, double y)
        //{
        //    if (lineBeingErased.X1 <= x && x <= lineBeingErased.X2 && lineBeingErased.Y1 <= y && y <= lineBeingErased.Y2) // line going like this: \
        //    {
        //        return true;
        //    }
        //    else if (lineBeingErased.X2 <= x && x <= lineBeingErased.X1 && lineBeingErased.Y1 <= y && y <= lineBeingErased.Y2) // line going like this: /
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //public bool CanSnapToLine(double x, double y)
        //{
        //    foreach (Drawing drawing in source.Drawings())
        //    {
        //        foreach (Shape shape in drawing.Shapes)
        //        {
        //            Line line = shape as Line;  
        //            if (line != null)   // if single straight line or part of rectangle
        //            {
        //                if (line.X1 <= x && x <= line.X2 && line.Y1 <= y && y <= line.Y2) // line going like this: \
        //                {
        //                    double deltaY = line.Y2 - line.Y1;
        //                    double deltaX = line.X2 - line.X1;
        //                    double angleX = Math.Atan(deltaY / deltaX);
        //                    double angleY = Math.Atan(deltaX / deltaY);

        //                    slopeX = deltaX == 0 ? 1 : Math.Tan(angleX);
        //                    slopeY = deltaY == 0 ? 1 : Math.Tan(angleY);
        //                    lineBeingErased = line;
        //                    containingDrawing = drawing;

        //                    return true;
        //                }
        //                else if (line.X2 <= x && x <= line.X1 && line.Y1 <= y && y <= line.Y2) // line going like this: /
        //                {
        //                    double deltaY = line.Y2 - line.Y1;
        //                    double deltaX = line.X1 - line.X2;
        //                    double angleX = Math.Atan(deltaY / deltaX);
        //                    double angleY = Math.Atan(deltaX / deltaY);

        //                    slopeX = deltaX == 0 ? -1 : Math.Tan(angleX) * -1;
        //                    slopeY = deltaY == 0 ? -1 : Math.Tan(angleY) * -1;
        //                    lineBeingErased = line;
        //                    containingDrawing = drawing;

        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}
        
        //public Point SnapToLine(Line line, double x, double y)
        //{
        //    Point snappedToGrid = new Point(x, y);
        //    double dy = slopeX * (snappedToGrid.X - line.X1);
        //    double dx = slopeY * (snappedToGrid.Y - line.Y1);

        //    if (dx > dy)
        //    {
        //        snappedToGrid.Y = line.Y1 + dy;
        //    }
        //    else if (dy > dx)
        //    {
        //        snappedToGrid.X = line.X1 + dx;
        //    }

        //    return new Point(snappedToGrid.X, snappedToGrid.Y);
        //}

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            drawing = false;
            source.UnRenderFromCanvas(placeholderRedLine);
            //if (placeholderRedLine != null && containingDrawing != null && lineBeingErased != null)
            //{
            //    log += "line: X1:" + lineBeingErased.X1 + " Y1: " + lineBeingErased.Y1 + ", X2: " + lineBeingErased.X2 + ", Y2: " + lineBeingErased.Y2 + "\n";
            //    log += "eraser before: " + placeholderRedLine.X1 + " Y1: " + placeholderRedLine.Y1 + ", X2: " + placeholderRedLine.X2 + ", Y2: " + placeholderRedLine.Y2 + "\n";
            //    SnapLineTool.StandardizeLineDirection(placeholderRedLine);
            //    log += "eraser after: " + placeholderRedLine.X1 + " Y1: " + placeholderRedLine.Y1 + ", X2: " + placeholderRedLine.X2 + ", Y2: " + placeholderRedLine.Y2 + "\n";
            //    //MessageBox.Show(log);
            //    source.EraseLineFromDrawing(containingDrawing, lineBeingErased, placeholderRedLine);
            //}
            //keepDrawing = false;
            //placeholderRedLine = null;
            //lineBeingErased = null;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            lastKnownMouseDown = mousePosition;
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }
    }
}
