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
    public class CanvasDrawings
    {
        private class Command
        {
            private Action doAction;
            private Action undoAction;

            public Command(Action doAction, Action undoAction)
            {
                this.doAction = doAction;
                this.undoAction = undoAction;
            }

            public void Do()
            {
                doAction();
            }

            public void Undo()
            {
                undoAction();
            }
        }

        private const string AUTO_SAVE_FILE_PATH = "autosave.xml";

        private Canvas canvas;
        private List<Drawing> drawings;
        private List<Command> commandHistory;
        private int commandHistoryIndex;

        public int Width => (int)canvas.Width;
        public int Height => (int)canvas.Height;

        public Brush ForegroundColor
        {
            get => Brushes.Black.Clone();
        }

        public Brush BackgroundColor
        {
            get => Brushes.LightGray.Clone();
        }

        public double ForegroundThickness
        {
            get => 3;
        }

        public double BackgroundThickness
        {
            get => 1;
        }

        public CanvasDrawings(Canvas source)
        {
            canvas = source;
            drawings = new List<Drawing>();
            commandHistory = new List<Command>();
            commandHistoryIndex = -1;
        }

        public IEnumerable<Drawing> Drawings
        {
            get
            {
                foreach (Drawing drawing in drawings)
                {
                    yield return drawing;
                }
            }
        }

        private void RenderToCanvas(UIElement element)
        {
            UnRenderFromCanvas(element);
            canvas.Children.Add(element);
        }

        private void RenderToCanvas(Drawing drawing)
        {
            UnRenderFromCanvas(drawing);
            foreach (UIElement element in drawing.Elements)
            {
                canvas.Children.Add(element);
            }
        }

        private void UnRenderFromCanvas(UIElement element)
        {
            canvas.Children.Remove(element);
        }

        private void UnRenderFromCanvas(Drawing drawing)
        {
            foreach (UIElement element in drawing.Elements)
            {
                canvas.Children.Remove(element);
            }
        }

        public void RemoveDrawing(Drawing drawing)
        {
            drawings.Remove(drawing);
            UnRenderFromCanvas(drawing);
        }

        public void AddDrawing(Drawing drawing)
        {
            Command addNewDrawing = new Command
            (
                doAction: () =>
                {
                    RemoveDrawing(drawing);
                    drawings.Add(drawing);
                    RenderToCanvas(drawing);
                },

                undoAction: () =>
                {
                    RemoveDrawing(drawing);
                }
            );
            AddNewCommand(addNewDrawing);
        }

        private void AddNewCommand(Command command)
        {
            command.Do();
            commandHistory.Add(command);
            commandHistoryIndex++;
            commandHistory.RemoveRange(commandHistoryIndex, commandHistory.Count - (commandHistoryIndex + 1));
        }

        public void UndoLast()
        {
            if (commandHistoryIndex > -1 && commandHistory.Count > 0)
            {
                commandHistory[commandHistoryIndex].Undo();
                commandHistoryIndex--;
            }
        }

        public void RedoLast()
        {
            if (commandHistoryIndex < commandHistory.Count - 1)
            {
                commandHistoryIndex++;
                commandHistory[commandHistoryIndex].Do();
            }
        }

        public void AddNonDrawing(IEnumerable<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                canvas.Children.Add(element);
            }
        }

        public void RemoveNonDrawing(IEnumerable<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                canvas.Children.Remove(element);
            }
        }

        public void Erase(IEnumerable<EraserTool.EraseData> eraseRequests)
        {
            List<Command> eraseCommands = new List<Command>();
            foreach (EraserTool.EraseData data in eraseRequests)
            {
                eraseCommands.Add(EraseFromDrawingCommand(data.SourceDrawing, data.DrawingLine, data.ErasedSegment));
            }

            Command groupedErasesCommand = new Command
            (
                doAction: () =>
                {
                    foreach (Command command in eraseCommands)
                    {
                        command.Do();
                    }
                },

                undoAction: () =>
                {
                    foreach (Command command in eraseCommands)
                    {
                        command.Undo();
                    }
                }
            );

            AddNewCommand(groupedErasesCommand);
        }

        private Command EraseFromDrawingCommand(Drawing drawing, Line original, Line eraser)
        {
            Brush brush = original.Stroke;
            double thickness = original.StrokeThickness;

            eraser.X1 = Math.Round(eraser.X1);
            eraser.Y1 = Math.Round(eraser.Y1);
            eraser.X2 = Math.Round(eraser.X2);
            eraser.Y2 = Math.Round(eraser.Y2);

            bool vertical = Math.Abs(original.X1 - original.X2) < Double.Epsilon;
            if ((vertical && original.Y1 == eraser.Y1 && original.Y2 == eraser.Y2)
            || (!vertical && original.X1 == eraser.X1 && original.X2 == eraser.X2))
            {
                return new Command
                (
                    doAction: () =>
                    {
                        drawing.Elements.Remove(original);
                        UnRenderFromCanvas(original);
                    },

                    undoAction: () =>
                    {
                        drawing.Elements.Add(original);
                        RenderToCanvas(original);
                    }
                );
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
                    Stroke = brush,
                    StrokeThickness = thickness
                };

                secondSegment = new Line()
                {
                    X1 = points[2].X,
                    Y1 = points[2].Y,
                    X2 = points[3].X,
                    Y2 = points[3].Y,
                    Stroke = brush,
                    StrokeThickness = thickness
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
                    Stroke = brush,
                    StrokeThickness = thickness
                };

                secondSegment = new Line()
                {
                    X1 = points[2].X,
                    Y1 = points[2].Y,
                    X2 = points[3].X,
                    Y2 = points[3].Y,
                    Stroke = brush,
                    StrokeThickness = thickness
                };
            }

            return new Command
            (
               doAction: () =>
               {
                   drawing.Elements.Remove(original);
                   UnRenderFromCanvas(original);

                   drawing.Elements.Add(firstSegment);
                   drawing.Elements.Add(secondSegment);
                   RenderToCanvas(firstSegment);
                   RenderToCanvas(secondSegment);
               },

               undoAction: () =>
               {
                   drawing.Elements.Remove(firstSegment);
                   UnRenderFromCanvas(firstSegment);
                   drawing.Elements.Remove(secondSegment);
                   UnRenderFromCanvas(secondSegment);

                   drawing.Elements.Add(original);
                   RenderToCanvas(original);
               }
            );
        }

        public bool ClearCanvas(string confirmationPrompt = null)
        {
            if (drawings.Count == 0)
            {
                return true;
            }

            MessageBoxResult result = MessageBoxResult.Yes;

            if (confirmationPrompt != null)
            {
                result = MessageBox.Show(confirmationPrompt, "Confirmation", MessageBoxButton.YesNo);
            }

            if (result == MessageBoxResult.Yes)
            {
                IEnumerable<Drawing> erasedDrawings = new List<Drawing>(drawings.ToArray());

                Command clearCanvas = new Command
                (
                    doAction: () =>
                    {
                        foreach (Drawing drawing in drawings)
                        {
                            UnRenderFromCanvas(drawing);
                        }
                        drawings.Clear();
                    },

                    undoAction: () =>
                    {
                        foreach (Drawing drawing in erasedDrawings)
                        {
                            drawings.Add(drawing);
                            RenderToCanvas(drawing);
                        }
                    }
                );

                AddNewCommand(clearCanvas);
            }

            return result == MessageBoxResult.Yes;
        }

        public void RestoreCanvas()
        {
            foreach (Drawing drawing in drawings)
            {
                UnRenderFromCanvas(drawing);
                RenderToCanvas(drawing);
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
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }

                bool accepted = ClearCanvas("Loading a file will overrite any current progress\nAre you sure you would like to proceed?");
                if (!accepted)
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
                commandHistory.Clear();
                commandHistoryIndex = -1;
            }
            catch (Exception)
            {
                MessageBox.Show($"The save file <{filePath}> is invalid and could not be loaded", "File Load Failed");
            }
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