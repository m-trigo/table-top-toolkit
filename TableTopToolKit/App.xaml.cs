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
            SelectPencilTool, SelectLineTool, SelectRectangleTool, SelectEraserTool, SelectRulerTool, SelectIconTool,
            Print, PrintPreview,
            AutoSave, LoadPreviousAutoSave, SaveAs, LoadFile,
            ToggleIconView, ToggleGridMode,
            SetStandardTheme, SetInkTheme,
            SelectIcon,
            Zoom,
            RotateIcon
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
            grid = new Grid(canvasDrawings.Width, canvasDrawings.Height, 64, canvasDrawings, Theme.standard.GridTheme);
            CurrentTool = new SnapLineTool(canvasDrawings, grid);
        }

        public void Command(Controls control)
        {
            CurrentTool.Close(); // Allows the current tool to cleanup before anything else can occur

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
                CurrentTool = new PencilTool(canvasDrawings);
                break;

                case Controls.SelectLineTool:
                CurrentTool = new SnapLineTool(canvasDrawings, grid);
                break;

                case Controls.SelectRectangleTool:
                CurrentTool = new RectangleTool(canvasDrawings, grid);
                break;

                case Controls.SelectEraserTool:
                CurrentTool = new EraserTool(canvasDrawings, grid);
                break;

                case Controls.SelectRulerTool:
                CurrentTool = new RulerTool(canvasDrawings, grid);
                break;

                case Controls.SelectIconTool:
                CurrentTool = new SelectIconTool(canvasDrawings, grid);
                break;

                case Controls.Print:
                canvasDrawings.Print();
                break;

                case Controls.PrintPreview:
                canvasDrawings.PrintPreview();
                break;

                case Controls.ClearCanvas:
                canvasDrawings.ClearCanvas("Are you sure you would like to clear the canvas?");
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

                case Controls.ToggleGridMode:
                grid.ToggleMode();
                break;

                case Controls.SetStandardTheme:
                canvasDrawings.ChangeTheme(Theme.standard);
                grid.ChangeTheme(Theme.standard.GridTheme);
                EraserTool.theme = Theme.standard;
                SelectIconTool.theme = Theme.standard;
                break;

                case Controls.SetInkTheme:
                canvasDrawings.ChangeTheme(Theme.ink);
                grid.ChangeTheme(Theme.ink.GridTheme);
                EraserTool.theme = Theme.ink;
                SelectIconTool.theme = Theme.ink;
                break;

                case Controls.RotateIcon:
                SelectIconTool iconTool = CurrentTool as SelectIconTool;
                if (iconTool != null)
                {
                    iconTool.Rotate();
                }
                break;
            }
        }

        public void PlaceIcon(Point position, Image icon)
        {
            Point snapped = grid.SnapToGridCorners(position.X - grid.SquareSize / 2, position.Y - grid.SquareSize / 2);
            canvasDrawings.PlaceIcon(snapped, icon, grid.SquareSize, grid.SquareSize);
        }
    }
}