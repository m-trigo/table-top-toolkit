﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class Grid
    {
        private int canvasWidth;
        private int canvasHeight;
        private int step;
        private bool isVisible;
        private bool isInDotsMode;
        private CanvasDrawings canvasDrawings;

        public int SquareSize { get => step; }
        public List<Line> GridLines { private set; get; }
        private List<Line> GridDots { set; get; }
        private double MaxX { get => GridLines[GridLines.Count - 1].X2; }
        private double MaxY { get => GridLines[GridLines.Count - 1].Y2; }

        public Grid(int width, int height, int step, CanvasDrawings source)
        {
            canvasWidth = width;
            canvasHeight = height;
            this.step = step;
            isVisible = true;
            isInDotsMode = false;
            canvasDrawings = source;

            InitializeGridLines();
            canvasDrawings.AddNonDrawing(GridLines);
            canvasDrawings.AddNonDrawing(GridDots);
        }

        public void InitializeGridLines()
        {
            GridLines = new List<Line>();
            GridDots = new List<Line>();
            int numberOfHorizonalLines = canvasHeight / step;
            int numberOfVerticalLines = canvasWidth / step;

            int gridBottom = numberOfHorizonalLines * step;
            int gridRight = numberOfVerticalLines * step;

            for (int xPos = 0; xPos < canvasWidth; xPos += step)
            {
                Line line = new Line();
                line.Stroke = canvasDrawings.BackgroundColor;
                line.StrokeThickness = canvasDrawings.BackgroundThickness;

                line.X1 = xPos;
                line.Y1 = 0;

                line.X2 = xPos;
                line.Y2 = gridBottom;

                GridLines.Add(line);
            }

            for (int yPos = 0; yPos < canvasHeight; yPos += step)
            {
                Line line = new Line();
                line.Stroke = canvasDrawings.BackgroundColor;
                line.StrokeThickness = canvasDrawings.BackgroundThickness; line.Y1 = yPos;

                line.X1 = 0;

                line.Y2 = yPos;
                line.X2 = gridRight;

                GridLines.Add(line);
            }
        }


        public void InitializeGridDots()
        {
            for (int xPos = 0; xPos < canvasWidth; xPos += step) {
                for (int yPos = 0; yPos < canvasHeight; yPos += step) {
                    Line leftToRight = new Line();
                    leftToRight.Stroke = Brushes.Transparent;
                    leftToRight.StrokeThickness = canvasDrawings.ForegroundThickness;
                    leftToRight.X1 = xPos - 0.5;
                    leftToRight.X2 = xPos + 0.5;
                    leftToRight.Y1 = yPos - 0.5;
                    leftToRight.Y2 = yPos + 0.5;

                    Line rightToLeft = new Line();
                    rightToLeft.Stroke = Brushes.Transparent;
                    rightToLeft.StrokeThickness = canvasDrawings.ForegroundThickness;
                    rightToLeft.X1 = xPos + 0.5;
                    rightToLeft.X2 = xPos - 0.5;
                    rightToLeft.Y1 = yPos - 0.5;
                    rightToLeft.Y2 = yPos + 0.5;

                    GridDots.Add(leftToRight);
                    GridDots.Add(rightToLeft);
                }
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

        public int RoundToClosestVerticalLineX(double value)
        {
            for (int x = 0; x <= MaxX; x++)
            {
                if (Math.Abs(value - x) < step / 2)
                {
                    return x;
                }
            }

            return (int)(Math.Round(value));
        }

        public int RoundToClosestHorizontalLineY(double value)
        {
            for (int y = 0; y <= MaxY; y++)
            {
                if (Math.Abs(value - y) < step / 2)
                {
                    return y;
                }
            }

            return (int)(Math.Round(value));
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
            if (isInDotsMode)
            {
                foreach (Line dot in GridDots)
                {
                    dot.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
                }
            }
            else
            {
                foreach (Line line in GridLines)
                {
                    line.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
                }
            }

            isVisible = !isVisible;
        }

        public void ToggleMode()
        {
            if (isInDotsMode)
            {
                foreach (Line line in GridLines)
                {
                    line.Visibility = Visibility.Visible;
                    line.Stroke = canvasDrawings.BackgroundColor;
                }
                foreach (Line dot in GridDots)
                {
                    dot.Stroke = Brushes.Transparent;
                }
            }
            else
            {
                foreach (Line line in GridLines)
                {
                    line.Stroke = Brushes.Transparent;
                }
                foreach (Line dot in GridDots)
                {
                    dot.Visibility = Visibility.Visible;
                    dot.Stroke = Brushes.DarkGray;
                }
            }

            isVisible = true;
            isInDotsMode = !isInDotsMode;
        }

        public Line ClosestGridLine(double x, double y)
        {
            if (x > MaxX)
            {
                x = MaxX;
            }

            if (y > MaxY)
            {
                y = MaxY;
            }

            Point closePoint = SnapToGridCorners(x, y);
            double dx = x - closePoint.X;
            double dy = y - closePoint.Y;

            double dxStep = dx == 0 ? step : (dx / Math.Abs(dx)) * step;
            double dyStep = dy == 0 ? step : (dy / Math.Abs(dy)) * step;

            if (dx == 0 && dy == 0)
            {
                return new Line();
            }

            Point farPoint = (Math.Abs(dx) < Math.Abs(dy))
                ? new Point() { X = closePoint.X, Y = closePoint.Y + dyStep }
                : new Point() { X = closePoint.X + dxStep, Y = closePoint.Y };

            return new Line()
            {
                X1 = closePoint.X,
                Y1 = closePoint.Y,
                X2 = farPoint.X,
                Y2 = farPoint.Y
            };
        }
    }
}