// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;

using Genumerics;

namespace LostTech.LargeCollections
{
    /// <summary>Represents a range, that has start and end indexes.</summary>
    public readonly struct Range<T> : IEquatable<Range<T>>
                            where T : IComparable<T>, IEquatable<T>, new()
    {
        /// <summary>Represent the inclusive start index of the Range.</summary>
        public Index<T> Start { get; }

        /// <summary>Represent the exclusive end index of the Range.</summary>
        public Index<T> End { get; }

        /// <summary>Construct a Range<T>object using the start and end indexes.</summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public Range(Index<T> start, Index<T> end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Indicates whether the current Range<T>object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
        public override bool Equals(object? value) =>
            value is Range<T>r &&
            r.Start.Equals(Start) &&
            r.End.Equals(End);

        /// <summary>Indicates whether the current Range<T>object is equal to another Range<T>object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Range<T>other) => other.Start.Equals(Start) && other.End.Equals(End);
        public static bool operator ==(Range<T> a, Range<T> b) => a.Equals(b);
        public static bool operator !=(Range<T> a, Range<T> b) => !a.Equals(b);

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        /// <summary>Converts the value of the current Range<T>object to its equivalent string representation.</summary>
        public override string ToString()
        {
            return Start.ToString() + ".." + End.ToString();
        }

        /// <summary>Create a Range<T>object starting from start index to the end of the collection.</summary>
        public static Range<T>StartAt(Index<T> start) => new(start, Index<T>.End);

        /// <summary>Create a Range<T>object starting from first element in the collection to the end Index<T>.</summary>
        public static Range<T>EndAt(Index<T> end) => new(Index<T>.Start, end);

        /// <summary>Create a Range<T>object starting from first element to the end.</summary>
        public static Range<T>All => new(Index<T>.Start, Index<T>.End);

        /// <summary>Calculate the start offset and length of Range<T>object using a collection length.</summary>
        /// <param name="length">The length of the collection that the Range<T>will be used with. length has to be a positive value.</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter against negative values.
        /// It is expected Range<T>will be used with collections which always have non negative length/count.
        /// We validate the Range<T>is inside the length scope though.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T Offset, T Length) GetOffsetAndLength(T length)
        {
            T start;
            Index<T> startIndex = Start;
            if (startIndex.IsFromEnd)
                start = Number.Subtract(length, startIndex.Value);
            else
                start = startIndex.Value;

            T end;
            Index<T> endIndex = End;
            if (endIndex.IsFromEnd)
                end = Number.Subtract(length, endIndex.Value);
            else
                end = endIndex.Value;

            if (end.CompareTo(length) > 0 || start.CompareTo(end) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, Number.Subtract(end,  start));
        }
    }
}
