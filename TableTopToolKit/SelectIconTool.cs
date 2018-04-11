using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class SelectIconTool : DrawingTool
    {
        private static Rectangle currentIcon;
        private Grid grid;
        private CanvasDrawings source;
        private static Brush currentBrush;

        public SelectIconTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            if(currentIcon == null)
                currentIcon = new Rectangle();
        }
        public void Close()
        {
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            Point snapped = grid.SnapToGridCorners(mousePosition.X - grid.SquareSize / 2, mousePosition.Y - grid.SquareSize / 2);
            foreach (var item in source.Drawings)
            {
                if(item.Elements.Count == 1)
                {
                    if(item.Elements[0].GetType() == typeof(Rectangle) && item.Elements[0] as Rectangle != currentIcon)
                    {
                        Point p = new Point();
                        p.X = Canvas.GetLeft(item.Elements[0]);
                        p.Y = Canvas.GetTop(item.Elements[0]);
                        if(p == snapped)
                        {
                            currentIcon.Fill = currentBrush;
                            currentIcon = item.Elements[0] as Rectangle;
                            currentBrush = currentIcon.Fill;

                            Brush brush = new SolidColorBrush(Color.FromArgb(200,0, 0, 255));
                            currentIcon.Fill = brush;
                            break;
                        }
                    }
                }
            }
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
