namespace TinyEngine.General;
public struct Rect2D
{
    public Rect2D()
    {
    }

    public Rect2D(Point2D topLeft, Point2D bottomRight)
    {
      if (topLeft.X > bottomRight.X ||
            topLeft.Y > bottomRight.Y)
        {
            throw new InvalidOperationException("Top left < Bottom right");
        }

        TopLeft = topLeft;
        BottomRight = bottomRight;

  
    }

    public Rect2D(Point2D topLeft, double width, double height)
    {
        if (width < 0 || height < 0)
        {
            throw new InvalidOperationException("Width, height cannot be negative");
        }

        TopLeft = topLeft;
        BottomRight = new Point2D(TopLeft.X + width, TopLeft.Y  + height);
    }

    public Point2D TopLeft { get; set; }
    public Point2D BottomRight { get; set; }
    public Point2D TopRight => new(TopLeft.X + Width, TopLeft.Y);
    public Point2D BottomLeft => new(TopLeft.X, TopLeft.Y + Height);
    public double Width => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;

    public Point2D Center => new(TopLeft.X + (Width/2), TopLeft.Y + (Height / 2));

    public bool Intersects(Rect2D other)
    {
        if (TopLeft.X + Width < other.TopLeft.X ||
            TopLeft.X > other.TopLeft.X + other.Width ||
            TopLeft.Y + Height < other.TopLeft.Y ||
            TopLeft.Y > other.TopLeft.Y + other.Height)
        {
            return false;
        }

        return true;
    }

    public bool Contains(Point2D point)
    {
        return TopLeft.X <= point.X &&
            TopLeft.Y <= point.Y &&
            BottomRight.X >= point.X &&
            BottomRight.Y >= point.Y;
    }

    public Rect2D WithTopLeft(Point2D topLeft)
    {
        return new(topLeft, Width, Height);
    }

    public Rect2D WithTopRight(Point2D topRight)
    {
        var topLeft = new Point2D(topRight.X - Width, topRight.Y);

        return new(topLeft, Width, Height);
    }

    public Rect2D WithBottomRight(Point2D bottomRight)
    {
        var topLeft = new Point2D(bottomRight.X - Width, bottomRight.Y - Height);
        return new(topLeft, Width, Height);

    }

    public Rect2D WithBottomLeft(Point2D bottomLeft)
    {
        var topLeft = new Point2D(bottomLeft.X, bottomLeft.Y - Height);
        return new(topLeft, Width, Height);
    }

    public Rect2D WithCenter(Point2D center)
    {
        var topLeft = new Point2D(center.X - Width / 2, center.Y - Height / 2);
        return new(topLeft, Width, Height);
    }

    public Rect2D Translated(Vector2D vector)
    {
        var topLeft = TopLeft + vector;

        return new(topLeft, Width, Height);
    }

    public Rect2D Scaled(double scale)
    {
        return new(TopLeft, Width*scale,Height*scale);
    }
}