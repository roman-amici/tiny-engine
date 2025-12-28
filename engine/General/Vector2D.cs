namespace TinyEngine.General;

public struct Vector2D(double x, double y)
{
    public double X {get; set;} = x;
    public double Y {get; set;} = y;

    public double Magnitude()
    {
        return Math.Sqrt(X*X + Y*Y);
    }

    public static Vector2D operator * (double scalar, Vector2D vec)
    {
        return new(vec.X * scalar, vec.Y * scalar);
    }

    public static Vector2D operator + (Vector2D vec1, Vector2D vec2)
    {
        return new(vec1.X + vec2.X, vec1.Y + vec2.Y);
    }

    public static Point2D operator + (Point2D point, Vector2D vec)
    {
        return new(point.X + vec.X, point.Y + vec.Y);
    }

    public static Point2D operator - (Vector2D vec)
    {
        return new(-vec.X, -vec.Y);
    }
}