using System;
using System.Buffers;
using System.IO;
using Volga_IT.Helpers;

namespace NetExtender.Crypto.Hashes.XXHash
{
    public static partial class XXHash64
    {
        /// <summary>
        /// Compute xxHash for the stream
        /// </summary>
        /// <param name="stream">The stream of data</param>
        /// <param name="bufferSize">The buffer size</param>
        /// <param name="seed">The seed number</param>
        /// <returns>The hash</returns>
        public static UInt64 ComputeHash(Stream stream, Int32 bufferSize = 8192, UInt64 seed = 0)
        {
            // Optimizing memory allocation
            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(bufferSize + 32);

            Int32 offset = 0;
            Int64 length = 0;

            // Prepare the seed vector
            UInt64 v1 = seed + P1 + P2;
            UInt64 v2 = seed + P2;
            UInt64 v3 = seed + 0;
            UInt64 v4 = seed - P1;

            try
            {
                // Read flow of bytes
                Int32 readBytes;
                while ((readBytes = stream.Read(buffer, offset, bufferSize)) > 0)
                {
                    length += readBytes;
                    offset += readBytes;

                    if (offset < 32)
                    {
                        continue;
                    }

                    Int32 r = offset % 32; // remain
                    Int32 l = offset - r; // length

                    // Process the next chunk 
                    UnsafeAlign(buffer, l, ref v1, ref v2, ref v3, ref v4);

                    // Put remaining bytes to buffer
                    BufferHelper.BlockCopyUnsafe(buffer, l, buffer, 0, r);
                    offset = r;
                }

                // Process the final chunk
                UInt64 h64 = UnsafeFinal(buffer, offset, ref v1, ref v2, ref v3, ref v4, length, seed);

                return h64;
            }
            finally
            {
                // Free memory
                ArrayPool<Byte>.Shared.Return(buffer);
            }
        }
    }
}