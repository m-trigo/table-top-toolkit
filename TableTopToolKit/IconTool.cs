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
    class IconTool : DrawingTool
    {

        private CanvasDrawings source;
        private Grid grid;

        private Image image;

        public IconTool(CanvasDrawings canvasDrawings, Grid grid, Button icon)
        {
            source = canvasDrawings;
            this.grid = grid;
            image = new Image();
            image.Source = (icon.Content as Image).Source;
            image.Height =  30;
            image.Width =   30;

        }
        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            // the -15 is to make sure the image is in the correct place ( since its based off of top left )
            Point snapped = grid.SnapToGridCorners(mousePosition.X-15, mousePosition.Y-15);
            
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
