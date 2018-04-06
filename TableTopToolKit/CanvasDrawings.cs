using Microsoft.Win32;
using System;
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

        private Brush backgroundBrush;
        private Brush foregroundBrush;

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

        public IEnumerable<Drawing> Drawings()
        {
            foreach (Drawing drawing in drawings)
            {
                yield return drawing;
            }
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

        public void RenderInCanvas(Shape element)
        {
            canvas.Children.Add(element);
        }

        public void UnRenderFromCanvas(Shape element)
        {
            canvas.Children.Remove(element);
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

        public void DrawToCanvas(Image image, double x, double y)
        {
            undoDrawings.Clear();
            Rectangle newImage = new Rectangle();

            newImage.Width = image.Width;
            newImage.Height = image.Height;

            Canvas.SetLeft(newImage, x);
            Canvas.SetTop(newImage, y);

            ImageBrush brush = new ImageBrush(image.Source);
            newImage.Fill = brush;
            drawings.Add(new Drawing(newImage));
            canvas.Children.Add(newImage);
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
            drawing.Shapes.Remove(original);
            canvas.Children.Remove(original);

            eraser.X1 = Math.Round(eraser.X1);
            eraser.Y1 = Math.Round(eraser.Y1);
            eraser.X2 = Math.Round(eraser.X2);
            eraser.Y2 = Math.Round(eraser.Y2);

            bool vertical = Math.Abs(original.X1 - original.X2) < Double.Epsilon;
            if ((vertical && original.Y1 == eraser.Y1 && original.Y2 == eraser.Y2)
            || (!vertical && original.X1 == eraser.X1 && original.X2 == eraser.X2))
            {
                return; // whole line is erased, no need to rebuild additional ones
            }

            List<Point> points = new List<Point>()
            {
                new Point(){X = original.X1, Y = original.Y1},
                new Point(){X = original.X2, Y = original.Y2},
                new Point(){X = eraser.X1, Y = eraser.Y1},
                new Point(){X = eraser.X2, Y = eraser.Y2}
            };

            Line firstSegment;
            Line secondSegment;

            if (vertical)
            {
                points.Sort((lhs, rhs) => { return (int)(lhs.Y - rhs.Y); });

                firstSegment = new Line()
                {
                    X1 = points[0].X,
                    Y1 = points[0].Y,
                    X2 = points[1].X,
                    Y2 = points[1].Y,
                    Stroke = foregroundBrush,
                    StrokeThickness = foregroundThickness
                };

                secondSegment = new Line()
                {
                    X1 = points[2].X,
                    Y1 = points[2].Y,
                    X2 = points[3].X,
                    Y2 = points[3].Y,
                    Stroke = foregroundBrush,
                    StrokeThickness = foregroundThickness
                };
            }
            else
            {
                points.Sort((lhs, rhs) => { return (int)(lhs.X - rhs.X); });

                firstSegment = new Line()
                {
                    X1 = points[0].X,
                    Y1 = points[0].Y,
                    X2 = points[1].X,
                    Y2 = points[1].Y,
                    Stroke = foregroundBrush,
                    StrokeThickness = foregroundThickness
                };

                secondSegment = new Line()
                {
                    X1 = points[2].X,
                    Y1 = points[2].Y,
                    X2 = points[3].X,
                    Y2 = points[3].Y,
                    Stroke = foregroundBrush,
                    StrokeThickness = foregroundThickness
                };
            }

            drawing.Shapes.Add(firstSegment);
            drawing.Shapes.Add(secondSegment);
            RenderInCanvas(firstSegment);
            RenderInCanvas(secondSegment);
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
            foreach (Drawing drawing in drawings)
            {
                DrawToCanvas(drawing);
            }
            undoDrawings.Clear();
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

        public void SaveState(string filePath = AUTO_SAVE_FILE_PATH)
        {
            if (drawings.Count == 0)
            {
                return;
            }

            CanvasState canvasState = new CanvasState() { Drawings = drawings };
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CanvasState));
            using (FileStream file = File.Create(filePath))
            {
                xmlSerializer.Serialize(file, canvasState);
            }
        }

        public void LoadState(string filePath = AUTO_SAVE_FILE_PATH)
        {
            ClearCanvas();

            if (!File.Exists(filePath))
            {
                return;
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CanvasState));
            using (StreamReader file = new StreamReader(filePath))
            {
                CanvasState canvasState = xmlSerializer.Deserialize(file) as CanvasState;
                drawings = canvasState.Drawings;
            }

            RestoreCanvas();
        }

        public void SaveAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".xml";
            saveDialog.Filter = "XML File|*.xml";
            if (saveDialog.ShowDialog() == true)
            {
                SaveState(saveDialog.SafeFileName);
            }
        }

        public void LoadFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFile.Filter = "XML File|*xml";
            if (openFile.ShowDialog() == true)
            {
                LoadState(openFile.SafeFileName);
            }
        }
    }
}