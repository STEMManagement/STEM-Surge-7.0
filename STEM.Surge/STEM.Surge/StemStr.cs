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

namespace STEM.Surge
{
    /// <summary>
    /// I know!!!!!!!!!
    /// But this is so fast, I promise
    /// We do so much string modification and garbage collection gets out of hand without this
    /// And if it's bad, make it better please
    /// </summary>
    public class StemStr
    {
        char[] _CharBuf;                
        int _StrLen;

        static void BlockCopy(char[] source, int sourceIndex, char[] destination, int destinationIndex, int dataLength, bool reverse = false)
        {
            if (dataLength == 0)
                return;
            
            unsafe
            {
                fixed (char* s = source, d = destination)
                {
                    if (reverse)
                    {
                        for (int x = dataLength - 1; x >= 0; x--)
                        {
                            *(d + destinationIndex + x) = *(s + sourceIndex + x);
                        }
                    }
                    else
                    {
                        for (int x = 0; x < dataLength; x++)
                        {
                            *(d + destinationIndex + x) = *(s + sourceIndex + x);
                        }
                    }
                }
            }
        }


        static void BlockCopy(string source, int sourceIndex, char[] destination, int destinationIndex, int dataLength, bool reverse = false)
        {
            if (dataLength == 0)
                return;
            
            unsafe
            {
                fixed (char* s = source, d = destination)
                {
                    if (reverse)
                    {
                        for (int x = dataLength - 1; x >= 0; x--)
                        {
                            *(d + destinationIndex + x) = *(s + sourceIndex + x);
                        }
                    }
                    else
                    {
                        for (int x = 0; x < dataLength; x++)
                        {
                            *(d + destinationIndex + x) = *(s + sourceIndex + x);
                        }
                    }
                }
            }
        }

        public StemStr(String value, int capacity)
        {
            if (value == null)
                value = String.Empty;

            if (capacity < value.Length)
                capacity = value.Length;

            _CharBuf = new char[capacity];
            _StrLen = value.Length;

            if (_StrLen > 0)
                BlockCopy(value, 0, _CharBuf, 0, _StrLen);

            _LastBuild = value;
        }

        public void Reset(String value)
        {
            lock (this)
            {
                if (value == null)
                    value = String.Empty;

                if (value.Length > _CharBuf.Length)
                    MaxLength = value.Length;

                _StrLen = value.Length;

                if (_StrLen > 0)
                    BlockCopy(value, 0, _CharBuf, 0, _StrLen);

                _LastBuild = value;
            }
        }

        public void Append(String value)
        {
            lock (this)
            {
                if (System.String.IsNullOrEmpty(value))
                    value = String.Empty;

                if ((value.Length + _StrLen) > _CharBuf.Length)
                    MaxLength = (value.Length + _StrLen);

                if (value.Length > 0)
                {
                    BlockCopy(value, 0, _CharBuf, _StrLen, value.Length);
                    _LastBuild = null;
                }

                _StrLen = value.Length + _StrLen;
            }
        }

        public int MaxLength
        {
            get { return _CharBuf.Length; }
            set
            {
                lock (this)
                {
                    if (_CharBuf.Length < value)
                    {
                        char[] newArray = new char[value];
                        BlockCopy(_CharBuf, 0, newArray, 0, _StrLen);
                        _CharBuf = newArray;
                    }
                }
            }
        }

        String _LastBuild = null;
        public override String ToString()
        {
            lock (this)
            {
                if (_StrLen == 0)
                    return String.Empty;

                if (_LastBuild == null)
                    _LastBuild = new String(_CharBuf, 0, _StrLen);

                return _LastBuild;
            }
        }

        public string Substring(int startIndex, int length)
        {
            lock (this)
            {
                if (length < 1)
                    return String.Empty;

                if (startIndex + length > _StrLen)
                    length = _StrLen - startIndex;

                return new String(_CharBuf, startIndex, length);
            }
        }
                
        public int Length
        {
            get
            {
                return _StrLen;
            }

            set
            {
                lock (this)
                {
                    _StrLen = value;

                    if (_StrLen > _CharBuf.Length)
                        MaxLength = _StrLen;
                }
            }
        }

        public void Overwrite(String value, int startIndex)
        {
            lock (this)
            {
                if (System.String.IsNullOrEmpty(value))
                    return;

                _LastBuild = null;

                BlockCopy(value, 0, _CharBuf, startIndex, value.Length);
            }
        }

        public void Replace(String oldValue, String newValue, int startIndex)
        {
            if (System.String.IsNullOrEmpty(oldValue))
                throw new System.ArgumentNullException(nameof(oldValue));

            lock (this)
            {
                if (newValue == null)
                    newValue = "";
                
                List<int> replacementIndex = new List<int>();

                int index = startIndex;

                while ((index = IndexOf(oldValue, index)) > -1)
                {
                    replacementIndex.Add(index);
                    index += oldValue.Length;
                }

                if (replacementIndex.Count == 0)
                    return;

                _LastBuild = null;

                int delta = (newValue.Length - oldValue.Length) * replacementIndex.Count;

                if (_CharBuf.Length < (delta + _StrLen))
                    MaxLength = delta + _StrLen;
                
                if (delta == 0)
                {
                    for (int i = 0; i < replacementIndex.Count; i++)
                    {
                        startIndex = replacementIndex[i];

                        BlockCopy(newValue, 0, _CharBuf, startIndex, newValue.Length);
                    }
                }
                else if (delta > 0)
                {
                    for (int i = replacementIndex.Count; i > 0;)
                    {
                        int endIndex = _StrLen;
                        if (i < replacementIndex.Count)
                            endIndex = replacementIndex[i];

                        i--;

                        startIndex = (replacementIndex[i] + oldValue.Length);
                        int dataLen = endIndex - startIndex;
                        int shift = (i + 1) * (newValue.Length - oldValue.Length);
                        
                        int newArrayStart = (startIndex + shift) - newValue.Length;

                        BlockCopy(_CharBuf, startIndex, _CharBuf, startIndex + shift, dataLen, true);
                        BlockCopy(newValue, 0, _CharBuf, newArrayStart, newValue.Length);
                    }

                    _StrLen += delta;
                }
                else
                {
                    for (int i = 0; i < replacementIndex.Count; i++)
                    {
                        int endIndex = _StrLen;
                        if (i < replacementIndex.Count - 1)
                            endIndex = replacementIndex[i+1];

                        startIndex = replacementIndex[i] + oldValue.Length;
                        int dataLen = endIndex - startIndex;
                        int shift = (i + 1) * (newValue.Length - oldValue.Length);
                        
                        int newArrayStart = (startIndex + shift) - newValue.Length;

                        BlockCopy(_CharBuf, startIndex, _CharBuf, startIndex + shift, dataLen);
                        BlockCopy(newValue, 0, _CharBuf, newArrayStart, newValue.Length);
                    }

                    _StrLen += delta;
                }
            }
        }

        public int IndexOf(string value, int startIndex)
        {
            if (System.String.IsNullOrEmpty(value))
                throw new System.ArgumentNullException(nameof(value));

            if (startIndex >= _StrLen)
                return -1;
            
            int index = startIndex;
            unsafe
            {
                fixed (char* v = value, b = _CharBuf)
                {
                    while (index < _StrLen)
                    {
                        for (int i = 0; i < value.Length;)
                        {
                            if (*(v + i) != *(b + index + i))
                                break;

                            i++;

                            if (i == value.Length)
                                return index;
                        }

                        index++;
                    }
                }
            }

            return -1;
        }

        public bool StartsWith(string value, int startIndex)
        {
            if (System.String.IsNullOrEmpty(value))
                throw new System.ArgumentNullException(nameof(value));

            unsafe
            {
                fixed (char* v = value, b = _CharBuf)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (*(v+i) != *(b + startIndex + i))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}