using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private CanvasDrawings cd;
        private Grid grid;

        internal DrawingTool CurrentTool { private set; get; }

        public App()
        {
            InitializeComponent();
            cd = null;
        }

        public void InitializeCanvasDrawing(Canvas canvas)
        {
            cd = new CanvasDrawings(canvas);
            grid = new Grid(cd.Width, cd.Height, 64);
            grid.GridLines.ForEach(shape => cd.AddBackground(shape));
            CurrentTool = new SnapLineTool(cd, grid);
        }

        public void Command(Controls control)
        {
            switch (control)
            {
                case Controls.ToggleGrid:
                    grid.ToggleVisibility();
                    break;

                case Controls.SaveToPng:
                    cd.SaveToPNG("replace this later");
                    break;

                case Controls.Undo:
                    cd.UndoDrawing();
                    break;

                case Controls.Redo:
                    cd.RedoDrawing();
                    break;

                case Controls.SelectPencilTool:
                    CurrentTool = new PencilTool(cd);
                    break;

                case Controls.SelectLineTool:
                    CurrentTool = new SnapLineTool(cd, grid);
                    break;

                case Controls.SelectRectangleTool:
                    CurrentTool = new RectangleTool(cd, grid);
                    break;

                case Controls.SelectEraserTool:
                    CurrentTool = new EraserTool(cd, grid);
                    break;

                case Controls.Print:
                    cd.Print();
                    break;

                case Controls.PrintPreview:
                    cd.PrintPreview();
                    break;

                case Controls.ClearCanvas:
                    cd.ClearCanvas();
                    break;

                case Controls.AutoSave:
                    cd.SaveState();
                    break;

                case Controls.LoadPreviousAutoSave:
                    cd.LoadState();
                    break;

                case Controls.SaveAs:
                    cd.SaveAs();
                    break;

                case Controls.LoadFile:
                    cd.LoadFile();
                    break;
            }
        }

        public void PlaceIcon(Point position, Image icon)
        {
            Image iconCopy = new Image();
            iconCopy.Source = icon.Source;
            iconCopy.Height = grid.SquareSize;
            iconCopy.Width = grid.SquareSize;
            Point snapped = grid.SnapToGridCorners(position.X - grid.SquareSize / 2, position.Y - grid.SquareSize / 2);
            cd.DrawToCanvas(iconCopy, snapped.X, snapped.Y);
        }

        //second param is for the selected icon
        public void CommandWithButton(Controls control, Image icon)
        {
            switch (control)
            {
                case Controls.SelectIcon:
                    CurrentTool = new IconTool(cd, grid, icon);
                    break;
            }
        }
    }
}