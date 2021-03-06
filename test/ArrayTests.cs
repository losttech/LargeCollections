using System;
using System.Collections.Generic;

using LostTech.LargeCollections;

using Xunit;

namespace Memory
{
    public class ArrayTests
    {
        [Fact]
        public void AllocLarge()
        {
            Assert.True(LargeSize > uint.MaxValue);
            using var arr = new Array<byte>(LargeSize);
        }

        [Fact]
        public void AccessLast()
        {
            using var arr = new Array<byte>(LargeSize);
            arr[LargeSize - 1] = 42;
            Assert.Equal(42, arr[LargeSize - 1]);
        }

        nuint LargeSize
        {
            get
            {
                uint uintMax = uint.MaxValue;
                return uintMax + (nuint)2;
            }
        }
    }
}
