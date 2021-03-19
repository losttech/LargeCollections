// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;

namespace LostTech.LargeCollections
{
    public readonly struct Index<T> : IEquatable<Index<T>>
                            where T : IComparable<T>, IEquatable<T>, new()
    {
        /// <summary>Construct an Index<T> using a value and indicating if the Index<T> is from the start or from the end.</summary>
        /// <param name="value">The Index<T> value. it has to be zero or positive number.</param>
        /// <param name="fromEnd">Indicating if the Index<T> is from the start or from the end.</param>
        /// <remarks>
        /// If the Index<T> constructed from the end, Index<T> value 1 means pointing at the last element and Index<T> value 0 means pointing at beyond last element.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(T value, bool fromEnd = false)
        {
            if (value.CompareTo(new T()) < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
            IsFromEnd = fromEnd;
        }

        /// <summary>Create an Index<T> pointing at first element.</summary>
        public static Index<T> Start => new(new T());

        /// <summary>Create an Index<T> pointing at beyond last element.</summary>
        public static Index<T> End => new(new T(), fromEnd: true);

        /// <summary>Create an Index<T> from the start at the position indicated by the value.</summary>
        /// <param name="value">The Index<T> value from the start.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index<T> FromStart(T value)
        {
            return new Index<T>(value);
        }

        /// <summary>Create an Index<T> from the end at the position indicated by the value.</summary>
        /// <param name="value">The Index<T> value from the end.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index<T> FromEnd(T value)
        {
            return new Index<T>(value, fromEnd: true);
        }

        /// <summary>Returns the Index<T> value.</summary>
        public T Value { get; }

        /// <summary>Indicates whether the Index<T> is from the start or the end.</summary>
        public bool IsFromEnd { get; }

        /// <summary>Calculate the offset from the start using the giving collection length.</summary>
        /// <param name="length">The length of the collection that the Index<T> will be used with. length has to be a positive value</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
        /// we don't validate either the returned offset is greater than the input length.
        /// It is expected Index<T> will be used with collections which always have non negative length/count. If the returned offset is negative and
        /// then used to Index<T> a collection will get out of range exception which will be same affect as the validation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetOffset(T length) =>
            IsFromEnd ? Genumerics.Number.Subtract(length, Value) : Value;

        /// <summary>Indicates whether the current Index<T> object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
        public override bool Equals(object? value) => value is Index<T> other && other.Equals(this);

        /// <summary>Indicates whether the current Index<T> object is equal to another Index<T> object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Index<T> other) => Value.Equals(other.Value) && IsFromEnd == other.IsFromEnd;
        public static bool operator ==(Index<T> a, Index<T> b) => a.Equals(b);
        public static bool operator !=(Index<T> a, Index<T> b) => !a.Equals(b);

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode() => HashCode.Combine(Value, IsFromEnd);

        /// <summary>Converts integer number to an <see cref="Index"/>.</summary>
        public static implicit operator Index<T>(T value) => FromStart(value);
        /// <summary>Converts <see cref="Index"/> to an <see cref="Index"/>.</summary>
        public static implicit operator Index<T>(System.Index index) => new((T)(dynamic)index.Value, fromEnd: index.IsFromEnd);

        /// <summary>Converts the value of the current Index<T> object to its equivalent string representation.</summary>
        public override string? ToString()
        {
            if (IsFromEnd)
                return ToStringFromEnd();

            return Value.ToString();
        }

        private string ToStringFromEnd()
        {
            return '^' + Value.ToString();
        }
    }
}
