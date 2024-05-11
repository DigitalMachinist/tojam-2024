
using System;

public enum CardType : int
{
    None,
    Earth,
    Fire,
    Ice,
    Shock,
    Spaaace,
    Joker,
}

public static class RandomUtils
{
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        var index = UnityEngine.Random.Range(0, values.Length);
        return (T) values.GetValue(index);
    }
}