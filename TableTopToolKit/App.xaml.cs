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
        public enum Controls { SelectNext, SelectPrevious, ToggleGrid, Undo, Redo };

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
            grid = new Grid(cd.Width, cd.Height, 30);
            grid.GridLines.ForEach(shape => cd.AddBackground(shape));
            //CurrentTool = new FreeHandTool(cd);
            CurrentTool = new SnapLineTool(cd, grid);
        }

        public void Command(Controls control)
        {
            switch (control)
            {
                case Controls.SelectNext:
                    cd.SelectDrawing();
                    break;

                case Controls.SelectPrevious:
                    cd.SelectDrawing(false);
                    break;

                case Controls.ToggleGrid:
                    grid.ToggleVisibility();
                    break;
                case Controls.Undo:
                    cd.UndoDrawing();
                    break;
                case Controls.Redo:
                    cd.RedoDrawing();
                    break;
            }
        }
    }
}