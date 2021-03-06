// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using Xunit;

namespace LostTech.LargeCollections
{
    public class NIndexTests
    {
        [Fact]
        public void CreationTest()
        {
            var index = new NIndex(1, fromEnd: false);
            Assert.Equal((nuint)1, index.Value);
            Assert.False(index.IsFromEnd);

            index = new NIndex(11, fromEnd: true);
            Assert.Equal((nuint)11, index.Value);
            Assert.True(index.IsFromEnd);

            index = NIndex.Start;
            Assert.Equal((nuint)0, index.Value);
            Assert.False(index.IsFromEnd);

            index = NIndex.End;
            Assert.Equal((nuint)0, index.Value);
            Assert.True(index.IsFromEnd);

            index = NIndex.FromStart(3);
            Assert.Equal((nuint)3, index.Value);
            Assert.False(index.IsFromEnd);

            index = NIndex.FromEnd(10);
            Assert.Equal((nuint)10, index.Value);
            Assert.True(index.IsFromEnd);
        }

        [Fact]
        public void GetOffsetTest()
        {
            NIndex index = NIndex.FromStart(3);
            Assert.Equal((nuint)3, index.GetOffset(3));
            Assert.Equal((nuint)3, index.GetOffset(10));
            Assert.Equal((nuint)3, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.Equal((nuint)3, index.GetOffset(2));

            index = NIndex.FromEnd(3);
            Assert.Equal((nuint)0, index.GetOffset(3));
            Assert.Equal((nuint)7, index.GetOffset(10));
            Assert.Equal((nuint)17, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.Equal((nuint)(UIntPtr.Zero - 1), index.GetOffset(2));
        }

        [Fact]
        public void ImplicitCastTest()
        {
            NIndex index = 10;
            Assert.Equal((nuint)10, index.Value);
            Assert.False(index.IsFromEnd);

            index = new Index(10);
            Assert.Equal((nuint)10, index.Value);
            Assert.False(index.IsFromEnd);

            index = ^10;
            Assert.Equal((nuint)10, index.Value);
            Assert.True(index.IsFromEnd);
        }

        [Fact]
        public void EqualityTest()
        {
            NIndex index1 = 10;
            NIndex index2 = 10;
            Assert.True(index1.Equals(index2));
            Assert.True(index1.Equals((object)index2));

            index2 = new NIndex(10, fromEnd: true);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));

            index2 = new NIndex(9, fromEnd: false);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));
        }

        [Fact]
        public void HashCodeTest()
        {
            NIndex index1 = 10;
            NIndex index2 = 10;
            Assert.Equal(index1.GetHashCode(), index2.GetHashCode());

            index2 = new NIndex(10, fromEnd: true);
            Assert.NotEqual(index1.GetHashCode(), index2.GetHashCode());

            index2 = new NIndex(99999, fromEnd: false);
            Assert.NotEqual(index1.GetHashCode(), index2.GetHashCode());
        }

        [Fact]
        public void ToStringTest()
        {
            NIndex index1 = 100;
            Assert.Equal(100.ToString(), index1.ToString());

            index1 = new NIndex(50, fromEnd: true);
            Assert.Equal("^" + 50.ToString(), index1.ToString());
        }

        [Fact]
        public void CollectionTest()
        {
            using var array = new Array<uint>(11);
            for(uint i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
            // List<int> list = new List<int>(array);

            for (nuint i = 0; i < array.Length; i++)
            {
                //Assert.Equal(i, list[NIndex.FromStart(i)]);
                //Assert.Equal(list.Count - i - 1, array[^(i + 1)]);

                Assert.Equal(array.Length - i - 1, array[NIndex.FromEnd(i + 1)]);
                Assert.Equal(i, array[NIndex.FromStart(i)]);

                //Assert.Equal(array.AsSpan(i, array.Length - i).ToArray(), array[i..]);
            }
        }
    }
}
