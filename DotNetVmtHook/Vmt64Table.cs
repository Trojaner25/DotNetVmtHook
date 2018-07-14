﻿using System;
using System.Runtime.InteropServices;

namespace DotNetVmtHook
{
    public unsafe class Vmt64Table : VmtTable
    {
        public Vmt64Table(IntPtr instanceAddress) : base(instanceAddress)
        {
            if (IntPtr.Size < 8)
                throw new Exception("Can not use Vmt64Table in non-64 bit environment.");
        }

        public override void Init()
        {
            base.Init();

            OriginalVmtAddress = (IntPtr)((long**)InstanceAddress)[0];

            var vmt = (int**)OriginalVmtAddress;
            Length = CountFuncs((long**)vmt) * sizeof(long);

            int copySize = Length + sizeof(long);
            long** newVmt = (long**)Marshal.AllocHGlobal(copySize);
            Buffer.MemoryCopy((long*)vmt, (long*)newVmt, copySize, copySize);

            VmtAddress = (IntPtr)newVmt;
            SetVmt(VmtAddress);
        }

        protected override IntPtr ReadIndex(IntPtr ptr, int index)
        {
            return (IntPtr)((long**)ptr)[index];
        }

        protected override void WriteIndex(IntPtr ptr, int index, IntPtr value)
        {
            ((long**)ptr)[index] = (long*)value;
        }
    }
}