using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class Grid
    {
        private int canvasWidth;
        private int canvasHeight;
        private int step;
        private bool isVisible;

        public List<Line> GridLines { private set; get; }

        public Grid(int width, int height, int step)
        {
            canvasWidth = width;
            canvasHeight = height;
            this.step = step;
            isVisible = true;
            InitializeGridLines();
        }

        public void InitializeGridLines()
        {
            GridLines = new List<Line>();
            int numberOfHorizonalLines = canvasHeight / step;
            int numberOfVerticalLines = canvasWidth / step;

            int gridBottom = numberOfHorizonalLines * step;
            int gridRight = numberOfVerticalLines * step;

            for (int xPos = 0; xPos < canvasWidth; xPos += step)
            {
                Line line = new Line();
                line.X1 = xPos;
                line.Y1 = 0;

                line.X2 = xPos;
                line.Y2 = gridBottom;

                GridLines.Add(line);
            }

            for (int yPos = 0; yPos < canvasHeight; yPos += step)
            {
                Line line = new Line();
                line.Y1 = yPos;
                line.X1 = 0;

                line.Y2 = yPos;
                line.X2 = gridRight;

                GridLines.Add(line);
            }
        }

        public Point SnapToGridCorners(double x, double y)
        {
            return new Point(SnapCoordToClosest(x), SnapCoordToClosest(y));
        }

        public Point SnapToGridLines(Point start, Point end)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;

            if (Math.Abs(dx) < Math.Abs(dy))
            {
                return new Point(start.X, SnapCoordToClosest(end.Y));
            }
            else
            {
                return new Point(SnapCoordToClosest(end.X), start.Y);
            }
        }

        private int SnapCoordToClosest(double coordToSnap)
        {
            int cellIndex = (int)(coordToSnap / step);
            if (coordToSnap - step * cellIndex < step * (cellIndex + 1) - coordToSnap)
            {
                return step * cellIndex;
            }
            else
            {
                return step * (cellIndex + 1);
            }
        }

        public void ToggleVisibility()
        {
            foreach (Line line in GridLines)
            {
                line.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
            }

            isVisible = !isVisible;
        }
    }
}