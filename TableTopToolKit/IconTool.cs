using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class IconTool : DrawingTool
    {
        private CanvasDrawings source;
        private Grid grid;

        private Image image;

        public IconTool(CanvasDrawings canvasDrawings, Grid grid, Image icon)
        {
            source = canvasDrawings;
            this.grid = grid;
            image = new Image();
            image.Source = icon.Source;
            image.Height = grid.SquareSize;
            image.Width = grid.SquareSize;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            Point snapped = grid.SnapToGridCorners(mousePosition.X - grid.SquareSize / 2, mousePosition.Y - grid.SquareSize / 2);
            source.DrawToCanvas(image, snapped.X, snapped.Y);
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseMove(Point position, MouseEventArgs mouseEvent)
        {
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }
    }
}