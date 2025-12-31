using System.Drawing;

namespace TinyEngine.Drawing;

public class Animation<T>(Frame<T>[] frames, bool cycle = true) where T : notnull
{
    public Frame<T>[] Frames {get;} = frames;

    public bool Cycle {get;} = cycle;

    public bool IsComplete(FrameIndex frameIndex)
    {
        if (Cycle)
        {
            return false;
        }

        if (frameIndex.Index >= Frames.Length)
        {
            return true;
        }

        if (frameIndex.Index == Frames.Length-1 && frameIndex.FrameTimeElapsed >= Frames.Last().Duration)
        {
            return true;
        }

        return false;
    }

    public FrameIndex NextIndex(FrameIndex f)
    {
        if (IsComplete(f))
        {
            return new FrameIndex(Frames.Length-1, Frames.Last().Duration);
        }

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

public struct Frame<T>(T spriteKey, TimeSpan duration) where T : notnull
{
    public T SpriteKey {get;} = spriteKey;

    public TimeSpan Duration {get;} = duration;
}

public struct FrameIndex(int index, TimeSpan frameTimeElapsed)
{
    public int Index {get; set;} = index;
    public TimeSpan FrameTimeElapsed {get; set;} = frameTimeElapsed;
}