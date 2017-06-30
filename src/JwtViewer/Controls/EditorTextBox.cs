using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JwtViewer.Controls
{
    public partial class EditorTextBox : TextBox
    {
        private readonly IList<Token> _tokens = new List<Token>();
        private string Tab = "  ";

        

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            Text = Text.Replace("\t", Tab);
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
            drawingContext.DrawRectangle(Background, new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            var formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection,
                new Typeface(FontFamily.Source),
                FontSize, Foreground);

            foreach (var token in _tokens)
            {
                formattedText.SetForegroundBrush(token.Brush, token.Start, token.Length);
            }
            var topMargin = 2.0 + BorderThickness.Top;
            var leftBorder = GetRectFromCharacterIndex(0).Left;
            var origin = new Point(leftBorder - HorizontalOffset, topMargin - VerticalOffset);
            drawingContext.DrawText(formattedText, origin);
        }

        private static IEnumerable<Token> Parse(string text)
        {
            var index = -1;
            var start = 0;
            var length = 0;
            var brushes = new Brush[] {Brushes.Red, Brushes.Blue, Brushes.DarkCyan};
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
                    brushIndex = brushIndex >= brushes.Length
                        ? 0
                        : brushIndex + 1;

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
    }

    public struct Token
    {
        public Brush Brush { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return $"{Start}:{Length} {Brush}";
        }
    }
}