﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Aubio.NET.Vectors
{
    internal sealed class CVecBufferPhas : CVecBuffer
    {
        public unsafe CVecBufferPhas([NotNull] CVec cVec, [NotNull] float* data, int length)
            : base(cVec, data, length)
        {
        }

        public override unsafe float this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();

                return cvec_phas_get_sample(CVec.Handle, (uint) index);
            }
            set
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();

                cvec_phas_set_sample(CVec.Handle, value, (uint) index);
            }
        }

        public override unsafe float* GetData()
        {
            return cvec_phas_get_data(CVec.Handle);
        }

        public override unsafe void SetAll(float value)
        {
            cvec_phas_set_all(CVec.Handle, value);
        }

        public override unsafe void Ones()
        {
            cvec_phas_ones(CVec.Handle);
        }

        public override unsafe void Zeros()
        {
            cvec_phas_zeros(CVec.Handle);
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* cvec_phas_get_data(
            CVec__* cVec
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float cvec_phas_get_sample(
            CVec__* cVec,
            uint position
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void cvec_phas_ones(
            CVec__* cVec
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void cvec_phas_set_all(
            CVec__* cVec,
            float value
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void cvec_phas_set_sample(
            CVec__* cVec,
            float value,
            uint position
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport("aubio", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void cvec_phas_zeros(
            CVec__* cVec
        );
    }
}