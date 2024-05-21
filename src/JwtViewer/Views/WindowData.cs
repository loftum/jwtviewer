namespace JwtViewer.Views;

public class WindowData
{
    public int X { get; set; }
    public int Y { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }

    public WindowData()
    {
        X = 0;
        Y = 0;
        Height = 600;
        Width = 800;
    }
}