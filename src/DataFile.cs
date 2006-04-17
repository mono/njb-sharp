/***************************************************************************
 *  DataFile.cs
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
using System.Collections;
using System.Runtime.InteropServices;

namespace Njb
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct njb_datafile_t 
    {
        public IntPtr filename;
        public IntPtr folder;
        public uint timestamp;
        public uint flags;
        public uint dfid;
        public ulong filesize;
        
        #pragma warning disable 0169
        private IntPtr nextdf;
        #pragma warning restore 0169
    }
    
    public class DataFile : IDisposable
    {
        [DllImport("libnjb")]
        private static extern void NJB_Datafile_Destroy(HandleRef df);
        
        private HandleRef handle;
        private string filename;
        private string folder;
        private uint timestamp;
        private uint flags;
        private uint dfid;
        private ulong filesize;
        
        internal DataFile(IntPtr df_ptr)
        {
            handle = new HandleRef(this, df_ptr);
            njb_datafile_t df_raw = (njb_datafile_t)Marshal.PtrToStructure(df_ptr, typeof(njb_datafile_t));

            filename = Utility.PtrToUtf8String(df_raw.filename);
            folder = Utility.PtrToUtf8String(df_raw.folder);
            timestamp = df_raw.timestamp;
            flags = df_raw.flags;
            dfid = df_raw.dfid;
            filesize = df_raw.filesize;
        }
        
        public void Dispose()
        {
            NJB_Datafile_Destroy(handle);
        }
        
        public string FileName {
            get {
                return filename;
            }
        }
        
        public string Folder {
            get {
                return folder;
            }
        }
        
        public uint TimeStamp {
            get {
                return timestamp;
            }
        }
        
        public uint Flags {
            get {
                return flags;
            }
        }
        
        public uint Id {
            get {
                return dfid;
            }
        }
        
        public ulong FileSize {
            get {
                return filesize;
            }
        }
        
        public override string ToString()
        {
            return String.Format("{0}/{1} ({2}) T:{3}", Folder, FileName, FileSize, TimeStamp);
        }
    }
}
