using System;

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

        [Fact]
        public void ZeroSized()
        {
            using var arr = new Array<int>(0);
            Assert.Equal(arr.Length, (nuint)0);
        }

        [Fact]
        public void BoundsCheck_Get()
        {
            using var arr = new Array<int>(42);
            Assert.Throws<IndexOutOfRangeException>(() => arr[42]);
        }

        [Fact]
        public void BoundsCheck_Set()
        {
            using var arr = new Array<int>(42);
            Assert.Throws<IndexOutOfRangeException>(() => arr[42] = 1);
        }

        [Fact]
        public void DisposedCheck_Get()
        {
            var arr = new Array<int>(42);
            arr.Dispose();
            Assert.Throws<ObjectDisposedException>(() => arr[1]);
        }

        [Fact]
        public void DisposedCheck_Set()
        {
            var arr = new Array<int>(42);
            arr.Dispose();
            Assert.Throws<ObjectDisposedException>(() => arr[1] = 42);
        }

        [Fact]
        public void DoubleDispose()
        {
            var arr = new Array<int>(42);
            arr.Dispose();
            arr.Dispose();
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
