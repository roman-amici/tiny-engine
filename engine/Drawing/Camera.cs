using TinyEngine.General;

namespace TinyEngine.Drawing;

public class Camera
{
    public Camera(double width, double height, double zoom)
    {
        WorldViewport = new(new(0,0),new(width,height));
        Zoom = zoom;
    }

    public Rect2D WorldViewport {get; set;}

    public double Zoom {get; set;} = 1.0;

    public Point2D ToScreenSpace(Point2D worldPoint)
    {
        var xScreen = (worldPoint.X - WorldViewport.TopLeft.X) * Zoom;
        var yScreen = (worldPoint.Y - WorldViewport.TopLeft.Y) * Zoom;

        return new Point2D(xScreen, yScreen);
    }

    public Rect2D ToScreenSpace(Rect2D worldRect)
    {
        return new Rect2D(ToScreenSpace(worldRect.TopLeft), ToScreenSpace(worldRect.BottomRight));
    }

    public bool IsVisible(Point2D worldPoint)
    {
        if (worldPoint.X < WorldViewport.TopLeft.X || worldPoint.X > WorldViewport.BottomRight.X)
        {
            return false;
        }

        if (worldPoint.Y < WorldViewport.TopLeft.Y || worldPoint.Y > WorldViewport.BottomRight.Y)
        {
            return false;
        }

        return true;
    }

    public bool IsVisible(Rect2D worldRect)
    {
        return IsVisible(worldRect.TopLeft) ||
            IsVisible(worldRect.TopRight) ||
            IsVisible(worldRect.BottomLeft) ||
            IsVisible(worldRect.BottomRight);
    }
}