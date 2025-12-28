namespace TinyEngine.Drawing;

public class Animation<T>(Frame<T>[] frames) where T : notnull
{
    public Frame<T>[] Frames {get;} = frames;

    public bool Cycle {get;} = true;

    public FrameIndex NextIndex(FrameIndex f)
    {
        while (true)
        {
            var frame = Frames[f.Index];
            if (f.FrameTimeElapsed < frame.Duration)
            {
                return f;
            }

            f.FrameTimeElapsed -= frame.Duration;

            if (Cycle)
            {
                f.Index = (f.Index + 1) % Frames.Length;
            }
            else
            {
                f.Index = Math.Min(f.Index + 1, Frames.Length-1);
            }
        }
    }
}

public struct Sprite<T>(T key)
{
    public T SpriteKey {get; set;} = key;
    public double Scale {get; set;} = 1.0;
}

public struct Frame<T>(Sprite<T> sprite, TimeSpan duration) where T : notnull
{
    public Sprite<T> Sprite {get;} = sprite;

    public TimeSpan Duration {get;} = duration;
}

public struct FrameIndex(int index, TimeSpan frameTimeElapsed)
{
    public int Index {get; set;} = index;
    public TimeSpan FrameTimeElapsed {get; set;} = frameTimeElapsed;
}