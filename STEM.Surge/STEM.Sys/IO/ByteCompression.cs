/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;

namespace STEM.Sys.IO
{
    /// <summary>
    /// Utility for compression of byte arrays
    /// </summary>
    public static class ByteCompression
    {
        /// <summary>
        /// Compress a buffer
        /// </summary>
        /// <param name="buf">Bytes to be compressed</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <returns>Compressed buffer</returns>
        public static byte[] Compress(byte[] buf, int dataLength)
        {
            if (buf == null || dataLength == 0)
                return null;

            MemoryStream ms = new MemoryStream();
            ms.Write(new byte[] { 23, 32, 32, 23 }, 0, 4);
            ms.Write(System.BitConverter.GetBytes(dataLength), 0, 4);
            ms.Position = 12;

            using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zs.Write(buf, 0, dataLength);
            }

            int fullLength = (int)ms.Position;
            ms.Position = 8;
            ms.Write(System.BitConverter.GetBytes(fullLength), 0, 4);
            ms.Position = 0;
            return ms.GetBuffer().Take(fullLength).ToArray();
        }

        /// <summary>
        /// Decompress buffer
        /// </summary>
        /// <param name="buf">Compressed buffer</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <returns>Decompressed buffer</returns>
        public static byte[] Decompress(byte[] buf, int dataLength)
        {
            int len = 0;
            return Decompress(buf, 0, dataLength, ref len);
        }

        /// <summary>
        /// Create a buffer, without compressing, that can be handled by Decompress(this byte[] buf) without error
        /// </summary>
        /// <param name="buf">Bytes to be wrapped</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <returns>Faux compressed buffer</returns>
        public static byte[] FauxCompress(this byte[] buf, int dataLength)
        {
            if (buf == null || dataLength == 0)
                return Array.Empty<byte>();

            byte[] ret = new byte[dataLength + 14];

            Array.Copy(new byte[] { 23, 32, 32, 23 }, 0, ret, 0, 4);
            Array.Copy(System.BitConverter.GetBytes(dataLength), 0, ret, 4, 4);
            Array.Copy(System.BitConverter.GetBytes(dataLength + 14), 0, ret, 8, 4);
            Array.Copy(new byte[] { 32, 32 }, 0, ret, 12, 2);
            Array.Copy(buf, 0, ret, 14, dataLength);

            return ret;
        }


        internal static void Recycle(byte[] buf)
        {
            if (buf == null)
                return;

            lock (_Recycler)
            {
                if (_Recycler.Count < 50)
                    _Recycler.Add(buf);
            }
        }

        static List<byte[]> _Recycler = new List<byte[]>();

        static byte[] GetBuffer(int size)
        {
            byte[] ret = null;

            lock (_Recycler)
            {
                ret = _Recycler.OrderBy(i => i.Length).FirstOrDefault(i => i.Length >= size);

                if (ret != null)
                    _Recycler.Remove(ret);
                else if (_Recycler.Count == 50)
                    _Recycler.RemoveAt(0);
            }

            if (ret == null)
                ret = new byte[size];

            return ret;
        }

        /// <summary>
        /// Decompress buffer
        /// </summary>
        /// <param name="buf">Compressed buffer</param>
        /// <param name="offset">Offset into buf to begin decompression</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <param name="bytesUsed">Used to inform caller of the number of bytes from buf that were actually decompressed</param>
        /// <returns>Decompressed buffer</returns>
        public static byte[] Decompress(byte[] buf, int offset, int dataLength, ref int bytesUsed)
        {
            int bytesReturned = 0;
            return Decompress(buf, offset, dataLength, ref bytesReturned, ref bytesUsed, false);
        }


        /// <summary>
        /// Decompress buffer
        /// </summary>
        /// <param name="buf">Compressed buffer</param>
        /// <param name="offset">Offset into buf to begin decompression</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <param name="bytesReturned">Used to inform caller of the number of bytes of decompressed data in the return buffer</param>
        /// <param name="bytesUsed">Used to inform caller of the number of bytes from buf that were actually decompressed</param>
        /// <returns>Decompressed buffer</returns>
        public static byte[] Decompress(byte[] buf, int offset, int dataLength, ref int bytesReturned, ref int bytesUsed)
        {
            return Decompress(buf, offset, dataLength, ref bytesReturned, ref bytesUsed, true);
        }

        /// <summary>
        /// Decompress buffer
        /// </summary>
        /// <param name="buf">Compressed buffer</param>
        /// <param name="offset">Offset into buf to begin decompression</param>
        /// <param name="dataLength">The number of actual data bytes in buf</param>
        /// <param name="bytesReturned">Used to inform caller of the number of bytes of decompressed data in the return buffer</param>
        /// <param name="bytesUsed">Used to inform caller of the number of bytes from buf that were actually decompressed</param>
        /// <param name="useRecycledBuffer">Should a recycled buffer be useable</param>
        /// <returns>Decompressed buffer</returns>
        public static byte[] Decompress(byte[] buf, int offset, int dataLength, ref int bytesReturned, ref int bytesUsed, bool useRecycledBuffer)
        {
            if (buf == null || buf.Length == 0)
                throw new ArgumentNullException(nameof(buf));

            try
            {
                int bytesSkipped = 0;

                while (dataLength > offset && buf[offset] == 0)
                {
                    bytesSkipped++;
                    offset++;
                }

                if ((offset + 14) >= dataLength)
                {
                    bytesUsed = bytesSkipped;
                    return null;
                }

                bool magicFound = false;
                bool faux = false;
                int index = offset;

                while (index + 11 < dataLength)
                {
                    if (buf[index] == 23 && buf[index + 1] == 32 && buf[index + 2] == 32 && buf[index + 3] == 23 && buf[index + 12] == 0x1f && buf[index + 13] == 0x8b)
                    {
                        magicFound = true;
                        break;
                    }
                    else if (buf[index] == 23 && buf[index + 1] == 32 && buf[index + 2] == 32 && buf[index + 3] == 23 && buf[index + 12] == 32 && buf[index + 13] == 32)
                    {
                        magicFound = true;
                        faux = true;
                        break;
                    }
                    else
                    {
                        bytesSkipped++;
                        index++;
                    }
                }

                if (!magicFound)
                {
                    bytesUsed = bytesSkipped;
                    return null;
                }

                if (index != offset)
                    STEM.Sys.EventLog.WriteEntry("ByteCompression.Decompress", "Skipping " + (index - offset) + " bytes!", STEM.Sys.EventLog.EventLogEntryType.Error);

                offset = index;

                int length = System.BitConverter.ToInt32(buf, offset + 4);
                int block = System.BitConverter.ToInt32(buf, offset + 8);

                if (block > dataLength - index)
                {
                    bytesUsed = 0;
                    return null;
                }

                if (block < 1 || length < 1)
                {
                    STEM.Sys.EventLog.WriteEntry("ByteCompression.Decompress", "Valid header but zero data length.", STEM.Sys.EventLog.EventLogEntryType.Error);
                    bytesUsed = 0;
                    return Decompress(buf, index + 4, dataLength, ref bytesReturned, ref bytesUsed, useRecycledBuffer);
                }

                bytesUsed = block + bytesSkipped;

                byte[] ret = null;

                if (faux)
                {
                    if (useRecycledBuffer)
                        ret = GetBuffer(length);
                    else
                        ret = new byte[length];

                    bytesReturned = length;
                    Array.Copy(buf, index + 14, ret, 0, length);
                    return ret;
                }

                int retry = 10;
                while (true)
                    try
                    {
                        retry--;

                        MemoryStream ms = new MemoryStream(buf);
                        ms.Position = index + 12;

                        if (useRecycledBuffer)
                            ret = GetBuffer(length);
                        else
                            ret = new byte[length];

                        bytesReturned = length;

                        using (GZipStream zs = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            int pos = 0;

                            while (pos < length)
                            {
                                int read = zs.Read(ret, pos, length-pos);
                                pos += read;

                                if (read == 0)
                                    break;
                            }

                            if (pos < length)
                            {
                                bytesReturned = 0;
                                return null;
                            }
                        }

                        break;
                    }
                    catch (System.OutOfMemoryException ex)
                    {
                        if (retry < 0)
                            throw new Exception("ByteCompression.Decompress", ex);

                        System.Threading.Thread.Sleep(10);
                    }

                return ret;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("ByteCompression.Decompress", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                throw;
            }
        }
    }
}
