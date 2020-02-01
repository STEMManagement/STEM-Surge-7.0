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
using System.Text;

namespace STEM.Sys.IO
{
    /// <summary>
    /// Utility for compression of strings
    /// </summary>
    public static class StringCompression
    {
        /// <summary>
        /// Compress a string
        /// </summary>
        /// <param name="str">String to compress</param>
        /// <returns>Compressed buffer</returns>
        public static byte[] Compress(this string str)
        {
            return CompressString(str);
        }

        /// <summary>
        /// Decompress a string
        /// </summary>
        /// <param name="str">Compressed buffer</param>
        /// <returns>Decompressed string</returns>
        public static string DecompressString(this byte[] str, int dataLength)
        {
            int len = 0;
            int bytesReturned = 0;
            byte[] buf = ByteCompression.Decompress(str, 0, dataLength, ref bytesReturned, ref len);

            if (buf == null)
                return null;

            try
            {
                return Encoding.Unicode.GetString(buf, 0, bytesReturned);
            }
            finally
            {
                ByteCompression.Recycle(buf);
            }
        }

        /// <summary>
        /// Compress a string
        /// </summary>
        /// <param name="text">String to compress</param>
        /// <returns>Compressed buffer</returns>
        public static byte[] CompressString(string text)
        {
            if (text == null || text.Length == 0)
                return null;

            byte[] buf = Encoding.Unicode.GetBytes(text);

            return ByteCompression.Compress(buf, buf.Length);
        }

        /// <summary>
        /// Decompress a string
        /// </summary>
        /// <param name="compressedText">Compressed buffer</param>
        /// <param name="offset">Offset into buf to begin decompression</param>
        /// <returns>Decompressed string</returns>
        public static string DecompressString(byte[] compressedText, int offset, int dataLength)
        {
            int len = 0;
            int bytesReturned = 0;
            byte[] buf = ByteCompression.Decompress(compressedText, offset, dataLength, ref bytesReturned, ref len);

            if (buf == null)
                return null;

            try
            {
                return Encoding.Unicode.GetString(buf, 0, bytesReturned);
            }
            finally
            {
                ByteCompression.Recycle(buf);
            }
        }

        /// <summary>
        /// Decompress a string
        /// </summary>
        /// <param name="compressedText">Compressed buffer</param>
        /// <param name="offset">Offset into buf to begin decompression</param>
        /// <param name="bytesUsed">Used to inform caller of the number of bytes from buf that were actually decompressed</param>
        /// <returns>Decompressed string</returns>
        public static string DecompressString(byte[] compressedText, int offset, int dataLength, ref int bytesUsed)
        {
            try
            {
                int len = 0;
                int bytesReturned = 0;
                byte[] buf = ByteCompression.Decompress(compressedText, offset, dataLength, ref bytesReturned, ref len);
                bytesUsed = len;

                if (buf == null)
                    return null;

                try
                {
                    if (bytesReturned < 1048576)
                    {
                        return Encoding.Unicode.GetString(buf, 0, bytesReturned);
                    }
                    else
                    {
                        int retry = 10;
                        while (true)
                            try
                            {
                                retry--;

                                string ret = "";

                                int o = 0;

                                while (o < bytesReturned)
                                {
                                    int read = 1048576;
                                    if ((bytesReturned - o) < 1048576)
                                        read = (bytesReturned - o);

                                    ret += Encoding.Unicode.GetString(buf, o, read);

                                    o += read;
                                }

                                return ret;
                            }
                            catch (System.OutOfMemoryException ex)
                            {
                                if (retry < 0)
                                    throw new Exception(System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".StringCompression.DecompressString", ex);

                                System.Threading.Thread.Sleep(10);
                            }
                    }
                }
                finally
                {
                    ByteCompression.Recycle(buf);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry(System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".StringCompression.DecompressString", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                throw;
            }
        }
    }
}
