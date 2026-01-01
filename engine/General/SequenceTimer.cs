
namespace TinyEngine.General;

public struct SequenceElement<T>(T elem, TimeSpan duration)
{
    public TimeSpan Duration {get;} = duration;
    public T Element {get;} = elem;
}

public class TimedSequence<T>(
    SequenceElement<T>[] sequence, 
    bool cycle = false)
{
    public SequenceElement<T>[] Sequence {get;} = sequence;

    public bool Cycle {get;} = cycle;

    public bool IsComplete(TimerIndex timerIndex)
    {
        if (Cycle)
        {
            return false;
        }

        if (timerIndex.Index >= Sequence.Length)
        {
            return true;
        }

        if (timerIndex.Index == Sequence.Length-1 && timerIndex.TimeInState >= Sequence.Last().Duration)
        {
            return true;
        }

        return false;
    }

    public TimerIndex NextIndex(TimerIndex f)
    {
        if (IsComplete(f))
        {
            return new TimerIndex(Sequence.Length-1, Sequence.Last().Duration);
        }

        while (true)
        {
            var frame = Sequence[f.Index];

            if (f.TimeInState < frame.Duration)
            {
                return f;
            }

            f.TimeInState -= frame.Duration;

            if (Cycle)
            {
                f.Index = (f.Index + 1) % Sequence.Length;
            }
            else
            {
                f.Index = Math.Min(f.Index + 1, Sequence.Length-1);
            }
        }
    }
}

public struct TimerIndex(int index, TimeSpan timeInState)
{
    public int Index {get; set;} = index;
    public TimeSpan TimeInState {get; set;} = timeInState;
}

public class SequenceState<T>(TimedSequence<T> sequence, TimedSequence<T>? next = null)
{
    public TimedSequence<T> Sequence {get; private set;} = sequence;

    public TimerIndex Index {get; set;} = default;

    public TimedSequence<T>? Next {get;} = next;

    public void Update(TimeSpan delta)
    {
        var advance = Index;
        advance.TimeInState += delta;
        
        Index = Sequence.NextIndex(advance);
    }
}