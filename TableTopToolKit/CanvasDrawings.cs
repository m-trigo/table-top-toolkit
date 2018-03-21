using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace TableTopToolKit
{
    internal class CanvasDrawings
    {
        private const string AUTO_SAVE_FILE_PATH = "autosave.xml";

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

        public List<Drawing> Drawings()
        {
            return drawings;
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

        public void UndrawPartOfDrawingFromCanvas(Drawing drawing, Shape shapeToUndraw)
        {
            drawing.Shapes.Remove(shapeToUndraw);
            canvas.Children.Remove(shapeToUndraw);
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

        public void EraseLineFromDrawing(Drawing drawing, Line original, Line eraser)
        {
            string log = "";
            log += "original: X1:" + original.X1 + " Y1: " + original.Y1 + ", X2: " + original.X2 + ", Y2: " + original.Y2 + "\n";
            log += "eraser: X1:" + eraser.X1 + " Y1: " + eraser.Y1 + ", X2: " + eraser.X2 + ", Y2: " + eraser.Y2 + "\n";
            UndoDrawing();
            UndrawPartOfDrawingFromCanvas(drawing, original);
            
            if (original.Y1 < eraser.Y1 || original.Y1 == eraser.Y1 && original.X1 < eraser.X1)
            {
                Line part1 = new Line();
                part1.X1 = original.X1;
                part1.Y1 = original.Y1;
                part1.X2 = eraser.X1;
                part1.Y2 = eraser.Y1;
                drawing.Shapes.Add(part1);
                StartDrawing(part1);

                log += "\npart1: X1:" + part1.X1 + " Y1: " + part1.Y1 + ", X2: " + part1.X2 + ", Y2: " + part1.Y2 + "\n";
            }

            if (eraser.Y2 < original.Y2 || eraser.Y2 == original.Y2 && eraser.X2 < original.X2)
            {
                Line part2 = new Line();
                part2.X1 = eraser.X2;
                part2.Y1 = eraser.Y2;
                part2.X2 = original.X2;
                part2.Y2 = original.Y2;
                drawing.Shapes.Add(part2);
                ContinueDrawing(part2);

                log += "part1: X1:" + part2.X1 + " Y1: " + part2.Y1 + ", X2: " + part2.X2 + ", Y2: " + part2.Y2 + "\n";
            }

            //MessageBox.Show(log);
        }
        
        public void ClearCanvas()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the canvas, you will NOT be able to undo this action?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                foreach (Drawing drawing in drawings)
                {
                    UndrawFromCanvas(drawing);
                }
                undoDrawings.Clear();
                drawings.Clear();
            }
        }

        public void RestoreCanvas()
        {
            undoDrawings.Clear();
            foreach (Drawing drawing in drawings)
            {
                DrawToCanvas(drawing);
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

        public void SaveState()
        {
            CanvasState canvasState = new CanvasState() { Drawings = drawings };
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CanvasState));
            using (FileStream file = File.Create(AUTO_SAVE_FILE_PATH))
            {
                xmlSerializer.Serialize(file, canvasState);
            }
        }

        public void LoadState()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CanvasState));
            using (StreamReader file = new StreamReader(AUTO_SAVE_FILE_PATH))
            {
                CanvasState canvasState = xmlSerializer.Deserialize(file) as CanvasState;
                drawings = canvasState.Drawings;
            }

            RestoreCanvas();
        }
    }
}