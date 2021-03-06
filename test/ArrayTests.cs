﻿using System;
using System.Runtime.InteropServices;

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

        [Fact]
        public unsafe void FromPointerAndSize_Check()
        {
            const uint size = 13;
            int* buffer = (int*)Marshal.AllocHGlobal(new IntPtr(Marshal.SizeOf<int>() * size));
            using var arr = new Array<int>(buffer, size);
            Assert.Equal(size, arr.Length);
        }

        [Fact]
        public void TooLarge()
        {
            ArgumentOutOfRangeException? error = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Array<byte>(unchecked((nuint)(-1))));
            Assert.Equal("length", error.ParamName);
        }

        nuint LargeSize
        {
            get
            {
#pragma warning disable RCS1118 // https://github.com/dotnet/roslyn/issues/51714
                uint uintMax = uint.MaxValue;
#pragma warning restore RCS1118 // Re-enable: Mark local variable as const.
                return uintMax + (nuint)2;
            }
        }
    }
}
