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

        public void Erase(Line eraserRedLine)
        {
            foreach(Drawing drawing in drawings)
            {
                foreach(Shape shape in drawing.Shapes)
                {
                    Line line = shape as Line;
                    if(line != null)
                    {
                        if ((line.X1 <= eraserRedLine.X1 && eraserRedLine.X2 <= line.X2 && line.Y1 == eraserRedLine.Y1 && line.Y2 == eraserRedLine.Y2) ||
                           (line.Y1 <= eraserRedLine.Y1 && eraserRedLine.Y2 <= line.Y2 && line.X1 == eraserRedLine.X1 && line.X2 == eraserRedLine.X2))
                        {
                            UndoDrawing();
                            UndrawPartOfDrawingFromCanvas(drawing, line);

                            Line temp1 = new Line();
                            temp1.X1 = line.X1;
                            temp1.Y1 = line.Y1;
                            temp1.X2 = eraserRedLine.X1;
                            temp1.Y2 = eraserRedLine.Y1;
                            StartDrawing(temp1);

                            Line temp2 = new Line();
                            temp2.X1 = eraserRedLine.X2;
                            temp2.Y1 = eraserRedLine.Y2;
                            temp2.X2 = line.X2;
                            temp2.Y2 = line.Y2;
                            StartDrawing(temp2);
                            return;
                        }
                    }
                }
            }
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