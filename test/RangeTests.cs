// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace LostTech.LargeCollections
{
    public class RangeTests
    {
        [Fact]
        public void CreationTest()
        {
            var range = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(2, fromEnd: true));
            Assert.Equal(10, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal(2, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range<nint>.StartAt(new Index<nint>(7, fromEnd: false));
            Assert.Equal(7, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal(0, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range<nint>.EndAt(new Index<nint>(3, fromEnd: true));
            Assert.Equal(0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal(3, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range<nint>.All;
            Assert.Equal(0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal(0, range.End.Value);
            Assert.True(range.End.IsFromEnd);
        }

        [Fact]
        public void GetOffsetAndLengthTest()
        {
            var range = Range<nint>.StartAt(new Index<nint>(5));
            (nint offset, nint length) = range.GetOffsetAndLength(20);
            Assert.Equal(5, offset);
            Assert.Equal(15, length);

            (offset, length) = range.GetOffsetAndLength(5);
            Assert.Equal(5, offset);
            Assert.Equal(0, length);

            // unlike standard Range, in generic one we always validate out of bounds
            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(-10));

            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(4));

            range = Range<nint>.EndAt(new Index<nint>(4));
            (offset, length) = range.GetOffsetAndLength(20);
            Assert.Equal(0, offset);
            Assert.Equal(4, length);
            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(1));
        }

        [Fact]
        public void EqualityTest()
        {
            var range1 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: false));
            var range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: false));
            Assert.True(range1.Equals(range2));
            Assert.True(range1.Equals((object)range2));

            range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: true));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));

            range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(21, fromEnd: false));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));
        }

        [Fact]
        public void HashCodeTest()
        {
            var range1 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: false));
            var range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: false));
            Assert.Equal(range1.GetHashCode(), range2.GetHashCode());

            range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: true));
            Assert.NotEqual(range1.GetHashCode(), range2.GetHashCode());

            range2 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(21, fromEnd: false));
            Assert.NotEqual(range1.GetHashCode(), range2.GetHashCode());
        }

        [Fact]
        public void ToStringTest()
        {
            var range1 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: false));
            Assert.Equal(10.ToString() + ".." + 20.ToString(), range1.ToString());

            range1 = new Range<nint>(new Index<nint>(10, fromEnd: false), new Index<nint>(20, fromEnd: true));
            Assert.Equal(10.ToString() + "..^" + 20.ToString(), range1.ToString());
        }
    }
}
