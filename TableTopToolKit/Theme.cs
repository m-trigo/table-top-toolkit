using System.Windows.Media;

namespace TableTopToolKit
{

    public class GridTheme
    {
        public Brush GridLinesColor { get; set; }

        public Brush GridDotsColor { get; set; }

        public int GridLinesThickness { get; set; }

        public int GridDotsThickness { get; set; }
    }


    public class Theme {

        public int LinesThickness { get; set; }

        public Brush DrawingsColor { get; set; }  // lines, rectangles, pencil

        public Brush IconsColor { get; set; }

        public GridTheme GridTheme { get; set; }


        public static Theme standard = new Theme() 
        {
            GridTheme = new GridTheme
            {
                GridLinesColor = Brushes.LightGray,
                GridDotsColor = Brushes.DarkGray,
                GridLinesThickness = 1,
                GridDotsThickness = 3,
            },
            
            LinesThickness = 3,
            DrawingsColor = Brushes.Black,
            IconsColor = Brushes.Black
        };

        public static Theme ink = new Theme()
        {
            GridTheme = new GridTheme
            {
                GridLinesColor = Brushes.CornflowerBlue,
                GridDotsColor = Brushes.CornflowerBlue,
                GridLinesThickness = 1,
                GridDotsThickness = 3,
            },

            LinesThickness = 3,
            DrawingsColor = Brushes.DarkBlue,
            IconsColor = Brushes.DarkBlue
        };
    }
}
