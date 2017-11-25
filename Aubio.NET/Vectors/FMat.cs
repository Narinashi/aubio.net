﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Aubio.NET.Vectors
{
    [PublicAPI]
    public sealed class FMat : AubioObject, IEnumerable<IEnumerable<float>>
    {
        #region Fields

        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly unsafe FMat__* _fMat;

        #endregion

        #region Constructors

        internal unsafe FMat([NotNull] FMat__* fMat)
        {
            if (fMat == null)
                throw new ArgumentNullException(nameof(fMat));

            _fMat = fMat;
        }

        public unsafe FMat(int rows, int columns)
            : this(new_fmat(rows.ToUInt32(), columns.ToUInt32()))
        {
        }

        #endregion

        #region Public Members

        [PublicAPI]
        public unsafe int Rows => _fMat->Height.ToInt32();

        [PublicAPI]
        public unsafe int Columns => _fMat->Length.ToInt32();

        [PublicAPI]
        public float this[int row, int column]
        {
            get
            {
                ThrowOnInvalidIndex(row, column);
                return fmat_get_sample(this, row.ToUInt32(), column.ToUInt32());
            }
            set
            {
                ThrowOnInvalidIndex(row, column);
                fmat_set_sample(this, value, row.ToUInt32(), column.ToUInt32());
            }
        }

        public IEnumerator<IEnumerable<float>> GetEnumerator()
        {
            IEnumerable<IEnumerable<float>> Rows()
            {
                IEnumerable<float> Row(int i)
                {
                    for (var col = 0; col < Columns; col++)
                        yield return this[i, col];
                }

                for (var i = 0; i < this.Rows; i++)
                    yield return Row(i);
            }

            var enumerable = Rows();
            var enumerator = enumerable.GetEnumerator();
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [PublicAPI]
        public void Copy([NotNull] FMat target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));


            fmat_copy(this, target);
        }

        /// <summary>
        ///     see Remarks.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     Internally, aubio assign a reference to the returned copy, therefore,
        ///     changes in <see cref="FVec" /> and <see cref="FMat" /> are reflected.
        ///     Note that you can safely leverage the 'using' pattern on returned instance
        ///     as Aubio.NET handles such special cases internally.
        /// </remarks>
        [PublicAPI]
        public FVec GetChannel(int row)
        {
            ThrowOnInvalidRow(row);
            var output = new FVec(Columns, false);
            fmat_get_channel(this, row.ToUInt32(), output);
            return output;
        }

        [PublicAPI]
        public unsafe float* GetChannelData(int channel)
        {
            return fmat_get_channel_data(this, channel.ToUInt32());
        }

        [PublicAPI]
        public unsafe float** GetData()
        {
            return fmat_get_data(this);
        }

        [PublicAPI]
        public void Ones()
        {
            fmat_ones(this);
        }

        [PublicAPI]
        public void Print()
        {
            fmat_print(this);
        }

        [PublicAPI]
        public void Rev()
        {
            fmat_rev(this);
        }

        [PublicAPI]
        public void Set(float value)
        {
            fmat_set(this, value);
        }

        [PublicAPI]
        public void VecMul([NotNull] FVec scale, [NotNull] FVec output)
        {
            if (scale == null)
                throw new ArgumentNullException(nameof(scale));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (scale.Length != Columns)
                throw new ArgumentOutOfRangeException(nameof(scale));

            if (output.Length != Rows)
                throw new ArgumentOutOfRangeException(nameof(output));

            fmat_vecmul(this, scale, output);
        }

        [PublicAPI]
        public void Weight([NotNull] FMat weight)
        {
            if (weight == null)
                throw new ArgumentNullException(nameof(weight));

            fmat_weight(this, weight);
        }

        [PublicAPI]
        public void Zeros()
        {
            fmat_zeros(this);
        }

        private void ThrowOnInvalidIndex(int row, int column)
        {
            ThrowOnInvalidRow(row);
            ThrowOnInvalidColumn(column);
        }

        private void ThrowOnInvalidRow(int row)
        {
            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row));
        }

        private void ThrowOnInvalidColumn(int column)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column));
        }

        #endregion

        #region AubioObject Members

        protected override void DisposeNative()
        {
            del_fmat(this);
        }

        internal override unsafe IntPtr ToPointer()
        {
            return new IntPtr(_fMat);
        }

        #endregion

        #region Native Methods

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe FMat__* new_fmat(
            uint height,
            uint length
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void del_fmat(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_copy(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat target
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_get_channel(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            uint channel,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FVec output
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* fmat_get_channel_data(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            uint channel
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float** fmat_get_data(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern float fmat_get_sample(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            uint channel,
            uint position
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_ones(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_print(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_rev(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_set(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            float value
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_set_sample(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            float value,
            uint channel,
            uint position
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_vecmul(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FVec scale,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FVec output
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_weight(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat weight
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void fmat_zeros(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AubioObjectMarshaler))] FMat fMat
        );

        #endregion
    }
}