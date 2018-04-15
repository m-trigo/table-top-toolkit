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

    public class Theme
    {
        public int LinesThickness { get; set; }

        public Brush DrawingsColor { get; set; }  // lines, rectangles, pencil

        public Brush IconsColor { get; set; }

        public GridTheme GridTheme { get; set; }

        public static Theme standard = new Theme()
        {
            GridTheme = new GridTheme
            {
                GridLinesColor = Brushes.DarkGray,
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
                GridLinesColor = new SolidColorBrush(Color.FromArgb(255, 159, 175, 224)),
                GridDotsColor = new SolidColorBrush(Color.FromArgb(255, 159, 175, 224)),
                GridLinesThickness = 1,
                GridDotsThickness = 3,
            },

            LinesThickness = 3,
            DrawingsColor = new SolidColorBrush(Color.FromArgb(255, 49, 68, 152)),
            IconsColor = Brushes.DarkBlue
        };
    }
}