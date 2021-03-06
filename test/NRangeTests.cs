// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace LostTech.LargeCollections
{
    public class NRangeTests
    {
        [Fact]
        public void CreationTest()
        {
            var range = new NRange(new NIndex(10, fromEnd: false), new NIndex(2, fromEnd: true));
            Assert.Equal((nuint)10, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal((nuint)2, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = NRange.StartAt(new NIndex(7, fromEnd: false));
            Assert.Equal((nuint)7, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal((nuint)0, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = NRange.EndAt(new NIndex(3, fromEnd: true));
            Assert.Equal((nuint)0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal((nuint)3, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = NRange.All;
            Assert.Equal((nuint)0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.Equal((nuint)0, range.End.Value);
            Assert.True(range.End.IsFromEnd);
        }

        [Fact]
        public void GetOffsetAndLengthTest()
        {
            var range = NRange.StartAt(new NIndex(5));
            (nuint offset, nuint length) = range.GetOffsetAndLength(20);
            Assert.Equal((nuint)5, offset);
            Assert.Equal((nuint)15, length);

            (offset, length) = range.GetOffsetAndLength(5);
            Assert.Equal((nuint)5, offset);
            Assert.Equal((nuint)0, length);

            // we don't validate the length in the GetOffsetAndLength so passing negative length will just return the regular calculation according to the length value.
            (offset, length) = range.GetOffsetAndLength(unchecked((nuint)(-10)));
            Assert.Equal((nuint)5, offset);
            Assert.Equal(unchecked((nuint)(-15)), length);

            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(4));

            range = NRange.EndAt(new NIndex(4));
            (offset, length) = range.GetOffsetAndLength(20);
            Assert.Equal((nuint)0, offset);
            Assert.Equal((nuint)4, length);
            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(1));
        }

        [Fact]
        public void EqualityTest()
        {
            var range1 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: false));
            var range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: false));
            Assert.True(range1.Equals(range2));
            Assert.True(range1.Equals((object)range2));

            range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: true));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));

            range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(21, fromEnd: false));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));
        }

        [Fact]
        public void HashCodeTest()
        {
            var range1 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: false));
            var range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: false));
            Assert.Equal(range1.GetHashCode(), range2.GetHashCode());

            range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: true));
            Assert.NotEqual(range1.GetHashCode(), range2.GetHashCode());

            range2 = new NRange(new NIndex(10, fromEnd: false), new NIndex(21, fromEnd: false));
            Assert.NotEqual(range1.GetHashCode(), range2.GetHashCode());
        }

        [Fact]
        public void ToStringTest()
        {
            var range1 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: false));
            Assert.Equal(10.ToString() + ".." + 20.ToString(), range1.ToString());

            range1 = new NRange(new NIndex(10, fromEnd: false), new NIndex(20, fromEnd: true));
            Assert.Equal(10.ToString() + "..^" + 20.ToString(), range1.ToString());
        }
    }
}
