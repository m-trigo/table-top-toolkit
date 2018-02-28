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

        public List<Line> gridLines;

        public Grid(int width, int height, int step)
        {
            canvasWidth = width;
            canvasHeight = height;
            this.step = step;
        }

        public void InitializeGridLines()
        {
            gridLines = new List<Line>();

            for (int xPos = 0; xPos < canvasWidth; xPos += step)
            {
                Line line = new Line();
                line.X1 = xPos;
                line.Y1 = 0;

                line.X2 = xPos;
                line.Y2 = canvasWidth;

                line.Stroke = Brushes.Gray;

                gridLines.Add(line);
            }

            for (int yPos = 0; yPos < canvasHeight; yPos += step)
            {
                Line line = new Line();
                line.Y1 = yPos;
                line.X1 = 0;

                line.Y2 = yPos;
                line.X2 = canvasWidth;

                line.Stroke = Brushes.Gray;

                gridLines.Add(line);
            }
        }

        public Point SnapToGrid(double x, double y)
        {
            return new Point( SnapCoordToClosest(x), SnapCoordToClosest(y) );
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

    }
}
