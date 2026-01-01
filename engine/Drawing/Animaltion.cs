using System.Drawing;
using TinyEngine.General;

namespace TinyEngine.Drawing;

public class Animation<T>(SequenceElement<T>[] frames, bool cycle = true) : TimedSequence<T>(frames, cycle)
{
}