// Modified after: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Index.cs
// MIT licensed.
#if !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace System;
[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct Index : IEquatable<Index>
{
    private readonly int m_value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        if (fromEnd)
        {
            m_value = ~value;
        }
        else
        {
            m_value = value;
        }
    }
    private Index(int value)
    {
        m_value = value;
    }
    public static Index Start => new Index(0);
    public static Index End => new Index(~0);
    public int Value => m_value < 0 ? ~m_value : m_value;
    public bool IsFromEnd => m_value < 0;
    public static implicit operator Index(int value) => FromStart(value);
    public static bool operator ==(Index left, Index right) => left.Equals(right);
    public static bool operator !=(Index left, Index right) => !(left == right);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromStart(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        return new Index(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromEnd(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        return new Index(~value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetOffset(int length) => IsFromEnd ? m_value + length + 1 : m_value;
    public override bool Equals(object value) => value is Index && m_value == ((Index)value).m_value;
    public bool Equals(Index other) => m_value == other.m_value;
    public override int GetHashCode() => m_value;
    public override string ToString() => IsFromEnd ? ToStringFromEnd() : ((uint)Value).ToString();
    private string ToStringFromEnd() => '^' + Value.ToString();
}
#endif