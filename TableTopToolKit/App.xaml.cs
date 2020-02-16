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

            SaveToPng, Print, PrintPreview,
            Undo, Redo,
            ClearCanvas,
            SelectLineTool, SelectRectangleTool, SelectPencilTool, SelectEraserTool, SelectPencilEraser, SelectRulerTool,
            AutoSave, LoadPreviousAutoSave, SaveAs, LoadFile,
            ToggleGridMode, ToggleGridDisplay,
            SetStandardTheme, SetInkTheme, SetBlueprintTheme,
            Zoom
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
                case Controls.ToggleGridDisplay:
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

                case Controls.SelectPencilEraser:
                CurrentTool = new PencilEraserTool(canvasDrawings);
                break;

                case Controls.Print:
                canvasDrawings.Print();
                break;

                case Controls.PrintPreview:
                canvasDrawings.PrintPreview();
                break;

                case Controls.ClearCanvas:
                canvasDrawings.ClearCanvas("Starting a new file will discard any unsaved progress\nAre you sure you would like to proceed?");
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
                RulerTool.theme = Theme.standard;
                break;

                case Controls.SetInkTheme:
                canvasDrawings.ChangeTheme(Theme.ink);
                grid.ChangeTheme(Theme.ink.GridTheme);
                EraserTool.theme = Theme.ink;
                RulerTool.theme = Theme.ink;
                break;

                case Controls.SetBlueprintTheme:
                canvasDrawings.ChangeTheme(Theme.blueprint);
                grid.ChangeTheme(Theme.blueprint.GridTheme);
                EraserTool.theme = Theme.blueprint;
                RulerTool.theme = Theme.blueprint;
                break;
            }
        }
    }
}