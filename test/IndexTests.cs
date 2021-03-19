// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using Xunit;

namespace LostTech.LargeCollections
{
    public class IndexTests
    {
        [Fact]
        public void CreationTest()
        {
            var index = new Index<nint>(1, fromEnd: false);
            Assert.Equal(1, index.Value);
            Assert.False(index.IsFromEnd);

            index = new Index<nint>(11, fromEnd: true);
            Assert.Equal(11, index.Value);
            Assert.True(index.IsFromEnd);

            index = Index<nint>.Start;
            Assert.Equal(0, index.Value);
            Assert.False(index.IsFromEnd);

            index = Index<nint>.End;
            Assert.Equal(0, index.Value);
            Assert.True(index.IsFromEnd);

            index = Index<nint>.FromStart(3);
            Assert.Equal(3, index.Value);
            Assert.False(index.IsFromEnd);

            index = Index<nint>.FromEnd(10);
            Assert.Equal(10, index.Value);
            Assert.True(index.IsFromEnd);
        }

        [Fact]
        public void GetOffsetTest()
        {
            Index<nint> index = Index<nint>.FromStart(3);
            Assert.Equal(3, index.GetOffset(3));
            Assert.Equal(3, index.GetOffset(10));
            Assert.Equal(3, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.Equal(3, index.GetOffset(2));

            index = Index<nint>.FromEnd(3);
            Assert.Equal(0, index.GetOffset(3));
            Assert.Equal(7, index.GetOffset(10));
            Assert.Equal(17, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.Equal((nint)(IntPtr.Zero - 1), index.GetOffset(2));
        }

        [Fact]
        public void ImplicitCastTest()
        {
            Index<nint> index = 10;
            Assert.Equal(10, index.Value);
            Assert.False(index.IsFromEnd);

            index = new Index(10);
            Assert.Equal(10, index.Value);
            Assert.False(index.IsFromEnd);

            index = ^10;
            Assert.Equal(10, index.Value);
            Assert.True(index.IsFromEnd);
        }

        [Fact]
        public void EqualityTest()
        {
            Index<nint> index1 = 10;
            Index<nint> index2 = 10;
            Assert.True(index1.Equals(index2));
            Assert.True(index1.Equals((object)index2));

            index2 = new Index<nint>(10, fromEnd: true);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));

            index2 = new Index<nint>(9, fromEnd: false);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));
        }

        [Fact]
        public void HashCodeTest()
        {
            Index<nint> index1 = 10;
            Index<nint> index2 = 10;
            Assert.Equal(index1.GetHashCode(), index2.GetHashCode());

            index2 = new Index<nint>(10, fromEnd: true);
            Assert.NotEqual(index1.GetHashCode(), index2.GetHashCode());

            index2 = new Index<nint>(99999, fromEnd: false);
            Assert.NotEqual(index1.GetHashCode(), index2.GetHashCode());
        }

        [Fact]
        public void ToStringTest()
        {
            Index<nint> index1 = 100;
            Assert.Equal(100.ToString(), index1.ToString());

            index1 = new Index<nint>(50, fromEnd: true);
            Assert.Equal("^" + 50.ToString(), index1.ToString());
        }

        [Fact]
        public void CollectionTest()
        {
            using var array = new Array<int>(11);
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
            // List<int> list = new List<int>(array);

            for (nint i = 0; i < array.Length; i++)
            {
                //Assert.Equal(i, list[Index<nint>.FromStart(i)]);
                //Assert.Equal(list.Count - i - 1, array[^(i + 1)]);

                Assert.Equal(array.Length - i - 1, array[Index<nint>.FromEnd(i + 1)]);
                Assert.Equal(i, array[Index<nint>.FromStart(i)]);

                //Assert.Equal(array.AsSpan(i, array.Length - i).ToArray(), array[i..]);
            }
        }
    }
}
