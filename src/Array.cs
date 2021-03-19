using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LostTech.LargeCollections
{
    public sealed unsafe class Array<T> : IEnumerable<T>, IDisposable where T : unmanaged
    {
        private T* _buffer;
        /// <summary>Number of elements in the array</summary>
        public nint Length { get; }

        public Array(nint length)
        {
            if (length == 0) return;
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            long size = checked(Marshal.SizeOf<T>() * length);
            _buffer = (T*)Marshal.AllocHGlobal((IntPtr)size);
            Length = length;
            GC.AddMemoryPressure(size);
        }
        // TODO: currently, the buffer is assumed to be owned by the instance,
        // and it will be disposed using Marshal.FreeHGlobal.
        internal Array(T* buffer, nint length)
        {
            _buffer = buffer;
            Length = length;
        }

        /// <summary>
        /// Returns a reference, that points to the element at the specified index.
        ///
        /// <para>
        /// If the <see cref="Array{T}"/> is disposed or garbage collected, or
        /// if the <paramref name="index"/> is out of bounds, the returned
        /// reference will be/become invalid.
        /// </para>
        ///
        /// <para>
        /// Accessing invalid reference is undefined behavior.
        /// </para>
        /// </summary>
        public ref T UnsafeRef(nint index) => ref Unsafe.AsRef<T>(_buffer + index);
        /// <summary>
        /// Returns a reference, that points to the element at the specified index.
        ///
        /// <para>
        /// If the <see cref="Array{T}"/> is disposed or garbage collected, or
        /// if the <paramref name="index"/> is out of bounds, the returned
        /// reference will be/become invalid.
        /// </para>
        ///
        /// <para>
        /// Accessing invalid reference is undefined behavior.
        /// </para>
        /// </summary>
        public ref T UnsafeRef(Index<nint> index) => ref Unsafe.AsRef<T>(_buffer + index.GetOffset(Length));

        /// <summary>
        /// Gets or sets element at the specified index. Checks bounds.
        /// </summary>
        public T this[nint index]
        {
            get
            {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                if (index >= Length || index < 0) throw new IndexOutOfRangeException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                if (_buffer == null) throw new ObjectDisposedException("Array");

                return UnsafeRef(index);
            }

            set
            {
                if (index >= Length || index < 0) throw new IndexOutOfRangeException();
                if (_buffer == null) throw new ObjectDisposedException("Array");

                UnsafeRef(index) = value;
            }
        }
        /// <summary>
        /// Gets or sets element at the specified index. Checks bounds.
        /// </summary>
        public T this[Index<nint> index]
        {
            get => this[index.GetOffset(Length)];
            set => this[index.GetOffset(Length)] = value;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_buffer == null) return;

            GC.SuppressFinalize(this);

            Marshal.FreeHGlobal((IntPtr)_buffer);
            _buffer = null;

            long size = checked(Marshal.SizeOf<T>() * (nint)Length);
            GC.RemoveMemoryPressure(size);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (nint index = 0; index < Length; index++)
            {
                yield return UnsafeRef(index);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        ~Array()
        {
            Dispose();
        }
    }
}
