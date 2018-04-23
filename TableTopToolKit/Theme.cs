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

        public Brush EraserColor { get; set; }

        public Brush EraserSelectionColor { get; set; }

        public Brush IconSelectionColor { get; set; }

        public Brush RulerColor { get; set; }

        public GridTheme GridTheme { get; set; }

        public Brush BackgroundColor { get; set; }

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
            IconsColor = Brushes.Black,
            EraserColor = Brushes.LightPink,
            EraserSelectionColor = Brushes.Red,
            IconSelectionColor = new SolidColorBrush(Color.FromArgb(255, 37, 124, 203)),
            RulerColor = new SolidColorBrush(Color.FromArgb(255, 37, 124, 203)),
            BackgroundColor = Brushes.White
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
            IconsColor = Brushes.DarkBlue,
            EraserColor = Brushes.LightPink,
            EraserSelectionColor = Brushes.Red,
            IconSelectionColor = new SolidColorBrush(Color.FromArgb(255, 37, 124, 203)),
            RulerColor = Brushes.Goldenrod,
            BackgroundColor = Brushes.White
        };

        public static Theme blueprint = new Theme()
        {
            GridTheme = new GridTheme
            {
                GridLinesColor = new SolidColorBrush(Color.FromArgb(255, 159, 175, 224)),
                GridDotsColor = new SolidColorBrush(Color.FromArgb(255, 159, 175, 224)),
                GridLinesThickness = 1,
                GridDotsThickness = 3,
            },

            LinesThickness = 3,
            DrawingsColor = Brushes.WhiteSmoke,
            IconsColor = Brushes.WhiteSmoke,
            EraserColor = Brushes.HotPink,
            EraserSelectionColor = Brushes.Red,
            IconSelectionColor = new SolidColorBrush(Color.FromArgb(255, 37, 124, 203)),
            RulerColor = Brushes.Goldenrod,
            BackgroundColor = new BrushConverter().ConvertFromString("#0f4490") as Brush
        };
    }
}