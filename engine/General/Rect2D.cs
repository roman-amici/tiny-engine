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

    public bool Intersects(Rect2D other)
    {
        if (TopLeft.X < other.BottomRight.X && BottomRight.X > other.TopLeft.X &&
        TopLeft.Y > other.BottomRight.Y && BottomRight.Y < other.TopLeft.Y )
        {
            return true;
        } 

        return false;
    }

    public bool Contains(Point2D point)
    {
        return TopLeft.X <= point.X &&
            TopLeft.Y <= point.Y &&
            BottomRight.X >= point.X &&
            BottomRight.Y >= point.Y;
    }

    public Rect2D Translate(Point2D point)
    {
        var topLeft = new Point2D(TopLeft.X + point.X,TopLeft.Y + point.Y);

        return new(topLeft, Width, Height);
    }
}