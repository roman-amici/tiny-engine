using System.Drawing;
using TinyEngine.General;

namespace TinyEngine.Drawing;

public struct Sprite<T>(T key)
{
    public T SpriteKey {get; set;} = key;
    public Color Tint {get; set;} = Color.White;
    public Rect2D Transform {get; set;}
    public double Rotation {get; set;}
}