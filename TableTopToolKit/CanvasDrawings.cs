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

        public void DrawToCanvas(Image image, double x, double y)
        {
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
            if (eraser.X1 == eraser.X2 && eraser.Y1 == eraser.Y2)
            {
                return;
            }

            UndoDrawing();
            UndrawPartOfDrawingFromCanvas(drawing, original);

            bool drawingPartOne = false;

            if (original.Y1 < eraser.Y1 || original.Y1 == eraser.Y1 && original.X1 < eraser.X1)
            {
                Line part1 = new Line();
                part1.X1 = original.X1;
                part1.Y1 = original.Y1;
                part1.X2 = eraser.X1;
                part1.Y2 = eraser.Y1;
                drawing.Shapes.Add(part1);
                StartDrawing(part1);

                drawingPartOne = true;
            }

            if (eraser.Y2 < original.Y2 || eraser.Y2 == original.Y2 && eraser.X2 < original.X2)
            {
                Line part2 = new Line();
                part2.X1 = eraser.X2;
                part2.Y1 = eraser.Y2;
                part2.X2 = original.X2;
                part2.Y2 = original.Y2;
                drawing.Shapes.Add(part2);

                if (drawingPartOne)
                {
                    ContinueDrawing(part2);
                }
                else
                {
                    StartDrawing(part2);
                }
            }
        }
        
        public void ClearCanvas()
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            if (drawings.Count > 0)
            {
                result = MessageBox.Show("Are you sure you want to clear the canvas, you will NOT be able to undo this action?", "Confirmation", MessageBoxButton.YesNo);
            }

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