/***************************************************************************
 *  Utility.cs
 *
 *  Copyright (C) 2005-2006 Novell, Inc.
 *  Written by Aaron Bockover <aaron@abock.org>
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW: 
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),  
 *  to deal in the Software without restriction, including without limitation  
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,  
 *  and/or sell copies of the Software, and to permit persons to whom the  
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 *  DEALINGS IN THE SOFTWARE.
 */
 
using System;
using System.Runtime.InteropServices;

namespace Njb
{
    internal class Utility
    {
        private static int Utf8StringLength(IntPtr ptr)
        {
            int size;
            for(size = 0; Marshal.ReadByte(ptr, size) != 0; size++); 
            return size;
        }
        
        public static string PtrToUtf8String(IntPtr ptr)
        {
            if(ptr == IntPtr.Zero) {
                return null;
            }

            byte [] bytes = new byte[Utf8StringLength(ptr)];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static IntPtr Utf8StringToPtr(string str)
        {
            byte [] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            IntPtr memalloc = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, memalloc, bytes.Length);
            Marshal.WriteByte(memalloc, bytes.Length, 0);
            return memalloc;
        }
        
        public static void FreeStringPtr(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
