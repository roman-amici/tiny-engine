namespace TinyEngine.Input;

public class InputState
{
    public HashSet<Key> KeysDown {get;} = [];
}

public enum Key
{
    Up,
    Down,
    Left,
    Right,
    Space,
}