using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace JwtViewer.Controls
{
    public partial class JwtTextBox : TextBox
    {
        public static readonly DependencyProperty ForegroundBrushProperty =
            DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(JwtTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(JwtTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CursorColorProperty =
            DependencyProperty.Register("CursorColor", typeof(Color), typeof(JwtTextBox), new FrameworkPropertyMetadata(Colors.Black, OnCursorColorChanged));

        public Brush ForegroundBrush
        {
            get => (Brush)GetValue(ForegroundBrushProperty);
            set => SetValue(ForegroundBrushProperty, value);
        }

        public Brush BackgroundBrush
        {
            get => (Brush)GetValue(BackgroundBrushProperty);
            set => SetValue(BackgroundBrushProperty, value);
        }

        public Color CursorColor
        {
            get => (Color)GetValue(CursorColorProperty);
            set => SetValue(CursorColorProperty, value);
        }

        private readonly IList<Token> _tokens = new List<Token>();

        public JwtTextBox()
        {
            ForegroundProperty.OverrideMetadata(typeof(JwtTextBox),
                new FrameworkPropertyMetadata(Brushes.Transparent, OnForegroundChanged));

            BackgroundProperty.OverrideMetadata(typeof(JwtTextBox),
                new FrameworkPropertyMetadata(OnBackgroundChanged));

            Loaded += OnLoaded;
            TextWrapping = TextWrapping.Wrap;
            ForegroundBrush = Brushes.Black;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(this) > 0)
            {
                var border = VisualTreeHelper.GetChild(this, 0); // Border
                var grid = VisualTreeHelper.GetChild(border, 0); // Grid
                var container = VisualTreeHelper.GetChild(grid, 0);
                if (container is ScrollViewer s)
                {
                    s.ScrollChanged += (s2, e2) => InvalidateVisual();
                }
                else if (container is AdornerDecorator d)
                {
                    
                    d.SizeChanged += (s2, e2) => InvalidateVisual();
                }
                InvalidateVisual();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var a = "hei";
            base.OnPreviewKeyDown(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            _tokens.Clear();
            foreach (var token in Parse(Text))
            {
                _tokens.Add(token);
            }
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
            drawingContext.DrawRectangle(BackgroundBrush, new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            var formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.Source),
                FontSize, ForegroundBrush);

            foreach (var token in _tokens)
            {
                formattedText.SetForegroundBrush(token.Brush, token.Start, token.Length);
            }
            formattedText.Trimming = TextTrimming.None;
            formattedText.MaxTextWidth = ActualWidth;
            var origin = new Point(0, 0);
            drawingContext.DrawText(formattedText, origin);
        }

        private static IEnumerable<Token> Parse(string text)
        {
            var index = -1;
            var start = 0;
            var length = 0;
            var brushes = new Brush[] {Brushes.Red, Brushes.DarkOrchid, Brushes.DeepSkyBlue, Brushes.Gray};
            var brushIndex = 0;
            foreach (var c in text)
            {
                index++;
                if (c == '.')
                {
                    yield return new Token
                    {
                        Start = start,
                        Length = length,
                        Brush = brushes[brushIndex]
                    };
                    brushIndex = brushIndex < brushes.Length -1
                        ? brushIndex + 1
                        : brushIndex;

                    start = index + 1;
                    length = 0;
                }
                else
                {
                    length++;
                }
            }
            if (length > 0)
            {
                yield return new Token
                {
                    Start = start,
                    Length = length,
                    Brush = brushes[brushIndex]
                };
            }
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Equals(e.NewValue, Brushes.Transparent))
            {
                ((JwtTextBox)d).Foreground = Brushes.Transparent;
            }
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sh = (JwtTextBox)d;

            var brush = new SolidColorBrush(GetAlphaColor(sh.CursorColor));
            if (!(e.NewValue is SolidColorBrush a) || a.Color != brush.Color)
            {
                sh.Background = brush;
            }
        }

        private static void OnCursorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sh = (JwtTextBox)d;
            var c = (Color)e.NewValue;
            sh.Background = new SolidColorBrush(GetAlphaColor(c));
        }

        private static Color GetAlphaColor(Color color)
        {
            return Color.FromArgb(0, (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
        }
    }
}