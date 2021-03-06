using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LostTech.LargeCollections
{
    public sealed unsafe class Array<T> : IDisposable where T : unmanaged
    {
        private T* _buffer;
        /// <summary>Number of elements in the array</summary>
        public nuint Length { get; }

        public Array(nuint length)
        {
            if (length == 0) return;

            long size = checked(Marshal.SizeOf<T>() * (nint)length);
            _buffer = (T*)Marshal.AllocHGlobal((IntPtr)size);
            Length = length;
            GC.AddMemoryPressure(size);
        }
        // TODO: currently, the buffer is assumed to be owned by the instance,
        // and it will be disposed using Marshal.FreeHGlobal.
        internal Array(T* buffer, nuint length)
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
        public ref T UnsafeRef(nuint index) => ref Unsafe.AsRef<T>(_buffer + index);

        /// <summary>
        /// Gets or sets element at the specified index. Checks bounds.
        /// </summary>
        public T this[nuint index]
        {
            get
            {
                if (index >= Length) throw new IndexOutOfRangeException();
                if (_buffer == null) throw new ObjectDisposedException("Array");

                return UnsafeRef(index);
            }

            set
            {
                if (index >= Length) throw new IndexOutOfRangeException();
                if (_buffer == null) throw new ObjectDisposedException("Array");

                UnsafeRef(index) = value;
            }
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

        ~Array()
        {
            Dispose();
        }
    }
}
