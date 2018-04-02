using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    class EraserTool : DrawingTool {
        
        const double SLOPE_EPSILON = 0.001;

        private CanvasDrawings source;
        private Grid grid;

        private Point lastKnownMouseDown;
        private Line eraserLine;
        private bool drawing;
        
        private Line redLine;
        private Drawing erasedDrawing;
        private Line erasedSegment;

        public EraserTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;

            drawing = false;
            erasedDrawing = null;
            erasedSegment = null;
            redLine = null;
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
                        
                        for(double lineX = start.X; lineX <= end.X; lineX += grid.SquareSize)
                        {
                            double lineY = start.Y + slope * (lineX - start.X);
                            
                            Point point = new Point() { X = lineX, Y = lineY };
                            if (ClosetPointToCoordiante(x, y, point, closest).Equals(point))
                            {
                                //point.Y = Math.Round(point.Y);
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

                        for (double lineY = start.Y; lineY <= end.Y; lineY += grid.SquareSize)
                        {
                            double lineX = start.X + slope * (lineY - start.Y);
                            Point point = new Point() { X = lineX, Y = lineY };
                            if (ClosetPointToCoordiante(x, y, point, closest).Equals(point))
                            {
                                //point.X = Math.Round(point.X);
                                closest = point;
                            }
                        }
                    }
                }
            }

            return closest;
        }
        
        private double LineLengthSquared(Line line)
        {
            Point a = new Point() { X = line.X1, Y = line.Y1 };
            Point b = new Point() { X = line.X2, Y = line.Y2 };
            return (a - b).LengthSquared;
        }
        
        private bool Inlined(Point lhs, Point rhs, double targetSlope)
        {
            double slope = (rhs.Y - lhs.Y) / (rhs.X - lhs.X);
            return Math.Abs(targetSlope - slope) < SLOPE_EPSILON;
        }
        
        public Line SuperSection(Line a, Line b)
        {
            bool isAVertical = Math.Abs(a.X1 - a.X2) < Double.Epsilon;
            bool isBVertical = Math.Abs(b.X1 - b.X2) < Double.Epsilon;

            if (isAVertical || isBVertical) // if either is vertical
            {
                if (isAVertical && isBVertical && a.X1 == b.X1) // if then both are vertical, there is a super-section
                {
                    List<Point> points = new List<Point>()
                    {
                        new Point(){X = a.X1, Y = a.Y1},
                        new Point(){X = a.X2, Y = a.Y2},
                        new Point(){X = b.X1, Y = b.Y1},
                        new Point(){X = b.X2, Y = b.Y2}
                    };

                    points.Sort((lhs, rhs) => { return (int)(lhs.Y - rhs.Y); });
                    
                    if (a.Y2 < a.Y1)
                    {
                        double temp = a.X1;
                        a.X1 = a.X2;
                        a.X2 = temp;

                        temp = a.Y1;
                        a.Y1 = a.Y2;
                        a.Y2 = temp;
                    }
                    
                    Point aStart = new Point() { X = a.X1, Y = a.Y1 };
                    Point aEnd = new Point() { X = a.X2, Y = a.Y2 };

                    if ((points[0].Equals(aStart) && points[1].Equals(aEnd))
                    || (points[2].Equals(aStart) && points[3].Equals(aEnd)))
                    {
                        return null;
                    }

                    return new Line()
                    {
                        X1 = points[1].X,
                        Y1 = points[1].Y,

                        X2 = points[2].X,
                        Y2 = points[2].Y
                    };
                }

                return null; // one is vertical while the other isn't
            }

            // Both have slopes from this point on, even if they might both be 0
            double aSlope = (a.Y2 - a.Y1) / (a.X2 - a.X1);
            double bSlope = (b.Y2 - b.Y1) / (b.X2 - b.X1);

            if (Math.Abs(aSlope - bSlope) < SLOPE_EPSILON) // same slope
            {
                List<Point> points = new List<Point>()
                {
                    new Point(){X = a.X1, Y = a.Y1},
                    new Point(){X = a.X2, Y = a.Y2},
                    new Point(){X = b.X1, Y = b.Y1},
                    new Point(){X = b.X2, Y = b.Y2}
                };

                points.Sort((lhs, rhs) => { return (int)(lhs.X - rhs.X); });

                List<Point> distinctPoints = new List<Point>();
                foreach(Point p in points)
                {
                    if (!distinctPoints.Contains(p))
                    {
                        distinctPoints.Add(p);
                    }
                }

                bool inlined = true;
                for(int i = 1; i < distinctPoints.Count; i++)
                {
                    Point lhs = distinctPoints[i - 1];
                    Point rhs = distinctPoints[i];
                    if (lhs.Equals(rhs))
                    {
                        continue;
                    }

                    inlined &= Inlined(lhs, rhs, aSlope);
                }

                if (inlined)
                {
                    if (a.X2 < a.X1)
                    {
                        double temp = a.X1;
                        a.X1 = a.X2;
                        a.X2 = temp;

                        temp = a.Y1;
                        a.Y1 = a.Y2;
                        a.Y2 = temp;
                    }

                    Point aStart = new Point() { X = a.X1, Y = a.Y1 };
                    Point aEnd = new Point() { X = a.X2, Y = a.Y2 };

                    if ((points[0].Equals(aStart) && points[1].Equals(aEnd))
                    || (points[2].Equals(aStart) && points[3].Equals(aEnd)))
                    {
                        return null;
                    }
                    
                    return new Line()
                    {
                        X1 = points[1].X,
                        Y1 = points[1].Y,

                        X2 = points[2].X,
                        Y2 = points[2].Y
                    };
                }
            }
            
            return null;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (eraserLine != null)
            {
                source.UnRenderFromCanvas(eraserLine);
            }

            if (redLine != null)
            {
                source.UnRenderFromCanvas(redLine);
            }

            Vector mouseDragDistance = mousePosition - lastKnownMouseDown;
            double adx = Math.Abs(mouseDragDistance.X);
            double ady = Math.Abs(mouseDragDistance.Y);
            if (mouseEvent.LeftButton == MouseButtonState.Pressed
            && (adx > SystemParameters.MinimumHorizontalDragDistance || ady > SystemParameters.MinimumVerticalDragDistance))
            {
                //Point start = ClosestInterestPoint(lastKnownMouseDown.X, lastKnownMouseDown.Y);
                //Point end = ClosestInterestPoint(mousePosition.X, mousePosition.Y);

                Point start = grid.SnapToGridCorners(lastKnownMouseDown.X, lastKnownMouseDown.Y);
                Point end = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);

                if (start.Equals(end))
                {
                    return;
                }

                if (!drawing)
                {
                    eraserLine = new Line()
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = Brushes.LightPink,
                        StrokeThickness = 5
                    };
                    drawing = true;
                }
                else
                {
                    eraserLine.X1 = start.X;
                    eraserLine.Y1 = start.Y;
                    eraserLine.X2 = end.X;
                    eraserLine.Y2 = end.Y;
                }

                source.RenderInCanvas(eraserLine);

                Line intersection = null;

                foreach (Drawing drawing in source.Drawings())
                {
                    foreach (Shape shape in drawing.Shapes)
                    {
                        if (!(shape is Line))
                        {
                            continue;
                        }

                        Line line = shape as Line;
                        Line inter = SuperSection(eraserLine, line);

                        bool replaced = false;
                        if (intersection == null)
                        {
                            intersection = inter;
                            replaced = true;
                        }
                        else if (inter != null && LineLengthSquared(inter) > LineLengthSquared(intersection))
                        {
                            intersection = inter;
                            replaced = true;
                        }

                        if (replaced)
                        {
                            erasedDrawing = drawing;
                            erasedSegment = line;
                        }
                    }
                }

                if (intersection != null)
                {
                    redLine = intersection;
                    redLine.StrokeThickness = 4;
                    redLine.Stroke = Brushes.Red;
                    source.RenderInCanvas(redLine);
                }
                else
                {
                    erasedDrawing = null;
                    erasedSegment = null;
                    redLine = null;
                }
            }
        }
        
        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (redLine != null && erasedDrawing != null && erasedSegment != null)
            {
                source.EraseLineFromDrawing(erasedDrawing, erasedSegment, redLine);
            }

            drawing = false;
            source.UnRenderFromCanvas(eraserLine);
            source.UnRenderFromCanvas(redLine);
            erasedDrawing = null;
            erasedSegment = null;
            redLine = null;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            lastKnownMouseDown = mousePosition;
            drawing = false;
            erasedDrawing = null;
            erasedSegment = null;

            if (eraserLine != null)
            {
                source.UnRenderFromCanvas(eraserLine);
            }

            if (redLine != null)
            {
                source.UnRenderFromCanvas(redLine);
            }
            redLine = null;
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
            drawing = false;
            source.UnRenderFromCanvas(eraserLine);
            source.UnRenderFromCanvas(redLine);
        }
    }
}
