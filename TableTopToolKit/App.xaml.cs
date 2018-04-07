using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public partial class App : Application
    {
        public enum Controls
        {
            ToggleGrid, SaveToPng, Undo, Redo, ClearCanvas,
            SelectPencilTool, SelectLineTool, SelectRectangleTool, SelectEraserTool,
            Print, PrintPreview,
            AutoSave, LoadPreviousAutoSave, ToggleIconView, SaveAs, LoadFile,
            SelectIcon
        };

        private CanvasDrawings canvasDrawings;
        private Grid grid;

        public DrawingTool CurrentTool { private set; get; }

        public App()
        {
            InitializeComponent();
            canvasDrawings = null;
        }

        public void InitializeCanvasDrawing(Canvas canvas)
        {
            canvasDrawings = new CanvasDrawings(canvas);
            grid = new Grid(canvasDrawings.Width, canvasDrawings.Height, 64, canvasDrawings);
            CurrentTool = new SnapLineTool(canvasDrawings, grid);
        }

        private void ChangeTool(DrawingTool newTool)
        {
            CurrentTool.Close();
            CurrentTool = newTool;
        }

        public void Command(Controls control)
        {
            switch (control)
            {
                case Controls.ToggleGrid:
                    grid.ToggleVisibility();
                    break;

                case Controls.SaveToPng:
                    canvasDrawings.SaveToPNG("replace this later");
                    break;

                case Controls.Undo:
                    canvasDrawings.UndoLast();
                    break;

                case Controls.Redo:
                    canvasDrawings.RedoLast();
                    break;

                case Controls.SelectPencilTool:
                    ChangeTool(new PencilTool(canvasDrawings));
                    break;

                case Controls.SelectLineTool:
                    ChangeTool(new SnapLineTool(canvasDrawings, grid));
                    break;

                case Controls.SelectRectangleTool:
                    ChangeTool(new RectangleTool(canvasDrawings, grid));
                    break;

                case Controls.SelectEraserTool:
                    ChangeTool(new EraserTool(canvasDrawings, grid));
                    break;

                case Controls.Print:
                    canvasDrawings.Print();
                    break;

                case Controls.PrintPreview:
                    canvasDrawings.PrintPreview();
                    break;

                case Controls.ClearCanvas:
                    canvasDrawings.ClearCanvas();
                    break;

                case Controls.AutoSave:
                    canvasDrawings.SaveState();
                    break;

                case Controls.LoadPreviousAutoSave:
                    canvasDrawings.LoadState();
                    break;

                case Controls.SaveAs:
                    canvasDrawings.SaveAs();
                    break;

                case Controls.LoadFile:
                    canvasDrawings.LoadFile();
                    break;
            }
        }

        public void PlaceIcon(Point position, Image icon)
        {
            Image iconCopy = new Image();
            iconCopy.Source = icon.Source;
            iconCopy.Height = grid.SquareSize;
            iconCopy.Width = grid.SquareSize;

            Rectangle newImage = new Rectangle();
            newImage.Width = iconCopy.Width;
            newImage.Height = iconCopy.Height;

            ImageBrush brush = new ImageBrush(iconCopy.Source);
            newImage.Fill = brush;

            Point snapped = grid.SnapToGridCorners(position.X - grid.SquareSize / 2, position.Y - grid.SquareSize / 2);
            Canvas.SetLeft(newImage, snapped.X);
            Canvas.SetTop(newImage, snapped.Y);

            canvasDrawings.AddDrawing(new Drawing(newImage));
        }
    }
}