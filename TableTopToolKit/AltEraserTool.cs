using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class AltEraserTool : DrawingTool
    {
        private CanvasDrawings source;
        private Grid grid;
        private Line erasingLine;

        public AltEraserTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            erasingLine = new Line();
            erasingLine.Stroke = Brushes.Red;
            erasingLine.StrokeThickness = 8;
            source.RenderInCanvas(erasingLine);
        }

        public void UpdateErasingArea(Point mousePosition)
        {
            Line newErasingLine = grid.ClosestGridLine(mousePosition.X, mousePosition.Y);
            erasingLine.X1 = newErasingLine.X1;
            erasingLine.Y1 = newErasingLine.Y1;
            erasingLine.X2 = newErasingLine.X2;
            erasingLine.Y2 = newErasingLine.Y2;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            UpdateErasingArea(mousePosition);
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }
    }
}