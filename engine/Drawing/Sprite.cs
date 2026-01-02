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

public class RefSprite<TKey>(SpriteAtlas<TKey> atlas, TKey key)
    where TKey : notnull
{
    public SpriteAtlas<TKey> Atlas {get; set;} = atlas;
    public TKey SpriteKey {get; set;} = key;
    public Color Tint {get; set;} = Color.White;
    public Rect2D Transform {get; set;}
    public double Rotation {get; set;}
    public int Layer {get; set;}
}