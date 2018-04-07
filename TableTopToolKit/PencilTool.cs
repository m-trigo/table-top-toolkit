using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class PencilTool : DrawingTool
    {
        private CanvasDrawings canvasDrawings;
        private Polyline line;
        private bool added;

        public PencilTool(CanvasDrawings cd)
        {
            canvasDrawings = cd;
            line = null;
            added = false;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                line = new Polyline()
                {
                    Stroke = canvasDrawings.ForegroundColor,
                    StrokeThickness = canvasDrawings.ForegroundThickness
                };
            }
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Released)
            {
                line = null;
                added = false;
            }
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
            line = null;
            added = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (line != null)
            {
                line.Points.Add(mousePosition);

                if (!added)
                {
                    canvasDrawings.AddDrawing(new Drawing(line));
                    added = true;
                }
            }
        }
    }
}