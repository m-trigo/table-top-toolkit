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
        public enum Controls { SelectNext, SelectPrevious };

        private CanvasDrawings cd;
        private DrawingTool currentTool;

        public App()
        {
            InitializeComponent();
            cd = null;
        }

        public void InitializeCanvasDrawing(Canvas canvas)
        {
            cd = new CanvasDrawings(canvas);
            currentTool = new FreeHandTool(cd);
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            currentTool.MouseMove(mousePosition, mouseEvent);
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
            }
        }
    }
}