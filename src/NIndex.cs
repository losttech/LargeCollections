// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;

namespace LostTech.LargeCollections
{
    public readonly struct NIndex : IEquatable<NIndex>
    {
        /// <summary>Construct an NIndex using a value and indicating if the index is from the start or from the end.</summary>
        /// <param name="value">The index value. it has to be zero or positive number.</param>
        /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
        /// <remarks>
        /// If the NIndex constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NIndex(nuint value, bool fromEnd = false)
        {
            Value = value;
            IsFromEnd = fromEnd;
        }

        /// <summary>Create an NIndex pointing at first element.</summary>
        public static NIndex Start => new(0);

        /// <summary>Create an NIndex pointing at beyond last element.</summary>
        public static NIndex End => new(0, fromEnd: true);

        /// <summary>Create an NIndex from the start at the position indicated by the value.</summary>
        /// <param name="value">The index value from the start.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NIndex FromStart(nuint value)
        {
            return new NIndex(value);
        }

        /// <summary>Create an NIndex from the end at the position indicated by the value.</summary>
        /// <param name="value">The index value from the end.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NIndex FromEnd(nuint value)
        {
            return new NIndex(value, fromEnd: true);
        }

        /// <summary>Returns the index value.</summary>
        public nuint Value { get; }

        /// <summary>Indicates whether the index is from the start or the end.</summary>
        public bool IsFromEnd { get; }

        /// <summary>Calculate the offset from the start using the giving collection length.</summary>
        /// <param name="length">The length of the collection that the NIndex will be used with. length has to be a positive value</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
        /// we don't validate either the returned offset is greater than the input length.
        /// It is expected NIndex will be used with collections which always have non negative length/count. If the returned offset is negative and
        /// then used to index a collection will get out of range exception which will be same affect as the validation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nuint GetOffset(nuint length) => IsFromEnd ? length - Value : Value;

        /// <summary>Indicates whether the current NIndex object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
        public override bool Equals(object? value) => value is NIndex other && other.Equals(this);

        /// <summary>Indicates whether the current NIndex object is equal to another NIndex object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(NIndex other) => Value == other.Value && IsFromEnd == other.IsFromEnd;

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode() => HashCode.Combine(Value, IsFromEnd);

        /// <summary>Converts integer number to an <see cref="NIndex"/>.</summary>
        public static implicit operator NIndex(nuint value) => FromStart(value);
        /// <summary>Converts <see cref="Index"/> to an <see cref="NIndex"/>.</summary>
        public static implicit operator NIndex(Index index) => new ((nuint)index.Value, fromEnd: index.IsFromEnd);

        /// <summary>Converts the value of the current NIndex object to its equivalent string representation.</summary>
        public override string ToString()
        {
            if (IsFromEnd)
                return ToStringFromEnd();

            return ((uint)Value).ToString();
        }

        private string ToStringFromEnd()
        {
#if (!NETSTANDARD2_0 && !NETFRAMEWORK)
            Span<char> span = stackalloc char[11]; // 1 for ^ and 10 for longest possible uint value
            bool formatted = ((uint)Value).TryFormat(span.Slice(1), out int charsWritten);
            Debug.Assert(formatted);
            span[0] = '^';
            return new string(span.Slice(0, charsWritten + 1));
#else
            return '^' + Value.ToString();
#endif
        }
    }
}
