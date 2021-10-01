// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Text;

namespace Volga_IT.Helpers
{
    public static class StringBuilderHelper
    {
        public static Int32 LastIndexOf(this StringBuilder builder, Char value)
        {
            return LastIndexOf(builder, value, 0);
        }

        public static Int32 LastIndexOf(this StringBuilder builder, Char value, Int32 start)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return LastIndexOf(builder, value, start, start + 1);
        }

        public static Int32 LastIndexOf(this StringBuilder builder, Char value, Int32 start, Int32 count)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (start + count > builder.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            
            for (Int32 i = builder.Length - count; i >= start; i--)
            {
                if (builder[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}