// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Volga_IT.Helpers
{
    public static class StreamHelper
    {
        public static Char? TryReadChar(this Stream stream)
        {
            return TryReadChar(stream, Encoding.UTF8);
        }

        // Вроде должно работать. Но я вам этого не говорил, потому как внутри происходит магия декодинга :)
        public static Char? TryReadChar(this Stream stream, Encoding? encoding)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream not support reading");
            }

            encoding ??= Encoding.UTF8;
            
            Span<Byte> first = stackalloc Byte[1];
            if (stream.Read(first) != 1)
            {
                return null;
            }

            Span<Char> symbol = stackalloc Char[1];
            
            if (first[0] <= 0x7F)
            {
                if (encoding.GetChars(first, symbol) != 1)
                {
                    return null;
                }

                return symbol[0];
            }

            Int32 remaining = (first[0] & 240) == 240 ? 3 : (first[0] & 224) == 224 ? 2 : (first[0] & 192) == 192 ? 1 : -1;
            
            if (remaining <= 0)
            {
                return null;
            }

            Span<Byte> buffer = stackalloc Byte[remaining + 1];
            buffer[0] = first[0];

            if (stream.Read(buffer.Slice(1)) != remaining)
            {
                return null;
            }

            if (encoding.GetChars(buffer, symbol) != 1)
            {
                return null;
            }

            return symbol[0];
        }

        public static IEnumerable<Char> ReadCharSequence(this Stream stream)
        {
            return ReadCharSequence(stream, Encoding.UTF8);
        }

        public static IEnumerable<Char> ReadCharSequence(this Stream stream, Encoding? encoding)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            while (stream.TryReadChar(encoding) is Char symbol)
            {
                yield return symbol;
            }
        }
    }
}