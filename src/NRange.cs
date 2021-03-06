// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;

namespace LostTech.LargeCollections
{
    /// <summary>Represents a range, that has start and end indexes.</summary>
    public readonly struct NRange : IEquatable<NRange>
    {
        /// <summary>Represent the inclusive start index of the NRange.</summary>
        public NIndex Start { get; }

        /// <summary>Represent the exclusive end index of the NRange.</summary>
        public NIndex End { get; }

        /// <summary>Construct a NRange object using the start and end indexes.</summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public NRange(NIndex start, NIndex end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Indicates whether the current NRange object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
        public override bool Equals(object? value) =>
            value is NRange r &&
            r.Start.Equals(Start) &&
            r.End.Equals(End);

        /// <summary>Indicates whether the current NRange object is equal to another NRange object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(NRange other) => other.Start.Equals(Start) && other.End.Equals(End);

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        /// <summary>Converts the value of the current NRange object to its equivalent string representation.</summary>
        public override string ToString()
        {
            return Start.ToString() + ".." + End.ToString();
        }

        /// <summary>Create a NRange object starting from start index to the end of the collection.</summary>
        public static NRange StartAt(NIndex start) => new(start, NIndex.End);

        /// <summary>Create a NRange object starting from first element in the collection to the end NIndex.</summary>
        public static NRange EndAt(NIndex end) => new(NIndex.Start, end);

        /// <summary>Create a NRange object starting from first element to the end.</summary>
        public static NRange All => new(NIndex.Start, NIndex.End);

        /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
        /// <param name="length">The length of the collection that the range will be used with. length has to be a positive value.</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter against negative values.
        /// It is expected NRange will be used with collections which always have non negative length/count.
        /// We validate the range is inside the length scope though.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (nuint Offset, nuint Length) GetOffsetAndLength(nuint length)
        {
            nuint start;
            NIndex startIndex = Start;
            if (startIndex.IsFromEnd)
                start = length - startIndex.Value;
            else
                start = startIndex.Value;

            nuint end;
            NIndex endIndex = End;
            if (endIndex.IsFromEnd)
                end = length - endIndex.Value;
            else
                end = endIndex.Value;

            if (end > length || start > end)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, end - start);
        }
    }
}
