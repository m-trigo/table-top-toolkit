using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class CanvasDrawings
    {
        private Canvas canvas;
        private List<Drawing> drawings;
        private List<Drawing> undoDrawings;

        public Brush backgroundBrush;
        public Brush foregroundBrush;

        private double backgroundThickness;
        private double foregroundThickness;

        public int Width => (int)canvas.Width;
        public int Height => (int)canvas.Height;

        public CanvasDrawings(Canvas source)
        {
            canvas = source;
            drawings = new List<Drawing>();
            undoDrawings = new List<Drawing>();
            backgroundBrush = Brushes.LightGray;
            foregroundBrush = Brushes.Black;
            backgroundThickness = 1;
            foregroundThickness = 3;
        }

        public void AddBackground(Shape element)
        {
            element.StrokeThickness = backgroundThickness;
            element.Stroke = backgroundBrush;
            canvas.Children.Add(element);
        }

        public void AddForeGround(Shape element)
        {
            element.StrokeThickness = foregroundThickness;
            element.Stroke = element.Stroke ?? foregroundBrush;
            canvas.Children.Add(element);
        }

        public void StartDrawing(Shape element)
        {
            undoDrawings.Clear();
            drawings.Add(new Drawing(element));
            AddForeGround(element);
        }

        public void ContinueDrawing(Shape element)
        {
            drawings[drawings.Count - 1].AddToDrawing(element);
            AddForeGround(element);
        }

        public void UndrawFromCanvas(Drawing drawing)
        {
            foreach (Shape shape in drawing.Shapes)
            {
                canvas.Children.Remove(shape);
            }
        }

        public void DrawToCanvas(Drawing drawing)
        {
            foreach (Shape shape in drawing.Shapes)
            {
                canvas.Children.Add(shape);
            }
        }

        public void UndoDrawing()
        {
            if (drawings.Count > 0)
            {
                Drawing drawing = drawings[drawings.Count - 1];
                undoDrawings.Add(drawing);
                drawings.Remove(drawing);
                UndrawFromCanvas(drawing);
            }
        }

        public void RedoDrawing()
        {
            if (undoDrawings.Count > 0)
            {
                Drawing drawing = undoDrawings[undoDrawings.Count - 1];
                undoDrawings.Remove(drawing);
                drawings.Add(drawing);
                DrawToCanvas(drawing);
            }
        }

        public void ClearCanvas()
        {

            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the canvas, you will NOT be able to undo this action?", "Confirmation", MessageBoxButton.YesNo);

            if(result == MessageBoxResult.Yes)
            {
                foreach (Drawing drawing in drawings)
                {
                    UndrawFromCanvas(drawing);
                    undoDrawings.Clear();
                }
                drawings.Clear();
            }
            
        }

        public void SaveToPNG(string filename)
        {
            ConvertToImage.SaveToPng(canvas);
        }

        public void Print()
        {
            ConvertToImage.Print(canvas);
        }

        public void PrintPreview()
        {
            ConvertToImage.Preview(canvas);
        }
    }
}