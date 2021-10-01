// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Runtime.CompilerServices;

namespace Volga_IT.Helpers
{
    public static class BitHelper
    {
        public const Int32 BitInByte = 8;
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static UInt64 BitwiseRotateLeft(this UInt64 value, Int32 offset)
        {
            const Int32 size = sizeof(UInt64) * BitInByte;

            unchecked
            {
                switch (offset)
                {
                    case 0:
                        return value;
                    case > 0:
                        offset %= size;
                        return (value << offset) | (value >> (size - offset));
                    default:
                        offset = -offset % size;
                        return (value >> offset) | (value << (size - offset));
                }
            }
        }
    }
}