using System.Windows.Media;

namespace JwtViewer.Controls
{
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