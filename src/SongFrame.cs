/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  SongFrame.cs
 *
 *  Copyright (C) 2005 Novell
 *  Written by Aaron Bockover (aaron@aaronbock.net)
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
    public enum SongFrameType {
        String = 0x00,
        UInt16 = 0x02,
        UInt32 = 0x03
    }
    
    public class SongFrame
    {
        private string label;
        private SongFrameType frameType;
        
        private string data_str;
        private ushort data_short;
        private uint data_int;
 
        [DllImport("libnjbglue")]
        private static extern IntPtr NJB_Glue_Song_Frame_Get_Label(IntPtr frame);
        
        [DllImport("libnjbglue")]
        private static extern byte NJB_Glue_Song_Frame_Get_Type(IntPtr frame);
        
        [DllImport("libnjbglue")]
        private static extern IntPtr NJB_Glue_Song_Frame_Get_Data_String(IntPtr frame);
        
        [DllImport("libnjbglue")]
        private static extern ushort NJB_Glue_Song_Frame_Get_Data_UInt16(IntPtr frame);
        
        [DllImport("libnjbglue")]
        private static extern uint NJB_Glue_Song_Frame_Get_Data_UInt32(IntPtr frame);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Findframe(HandleRef handle, IntPtr label);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Frame_New_String(IntPtr label, IntPtr value);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Frame_New_Uint16(IntPtr label, ushort value);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Frame_New_Uint32(IntPtr label, uint value);

        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Frame_New_Codec(IntPtr codec);
        
        private IntPtr handle;
        
        public static SongFrame Find(Song song, string label)
        {
            IntPtr ptr = NJB_Songid_Findframe(song.Handle, Utility.Utf8StringToPtr(label));
            if(ptr == IntPtr.Zero) {
                return null;
            }
            
            return new SongFrame(ptr);
        }
        
        public static SongFrame New(string label, string value)
        {
            if(value == null) {
                return null;
            }
        
            IntPtr label_ptr = Utility.Utf8StringToPtr(label);
            IntPtr value_ptr = Utility.Utf8StringToPtr(value);
            
            SongFrame frame = new SongFrame(NJB_Songid_Frame_New_String(label_ptr, value_ptr));
            
            Utility.FreeStringPtr(label_ptr);
            Utility.FreeStringPtr(value_ptr);
            
            return frame;
        }
                
        public static SongFrame New(string label, uint value)
        {
            IntPtr label_ptr = Utility.Utf8StringToPtr(label);
            SongFrame frame = new SongFrame(NJB_Songid_Frame_New_Uint32(label_ptr, value));
            Utility.FreeStringPtr(label_ptr);
            return frame;
        }
                
        public static SongFrame New(string label, ushort value)
        {
            IntPtr label_ptr = Utility.Utf8StringToPtr(label);
            SongFrame frame = new SongFrame(NJB_Songid_Frame_New_Uint16(label_ptr, value));
            Utility.FreeStringPtr(label_ptr);
            return frame;
        }
        
                
        public static SongFrame NewCodec(string codec)
        {
            IntPtr codec_ptr = Utility.Utf8StringToPtr(codec);
            SongFrame frame = new SongFrame(NJB_Songid_Frame_New_Codec(codec_ptr));
            Utility.FreeStringPtr(codec_ptr);
            return frame;
        }
        
        public SongFrame(IntPtr framePtr)
        {
            if(framePtr == IntPtr.Zero) {
                throw new ApplicationException("Could not create SongFrame");
            }
        
            label = Utility.PtrToUtf8String(NJB_Glue_Song_Frame_Get_Label(framePtr));
            handle = framePtr;
            
            switch(NJB_Glue_Song_Frame_Get_Type(framePtr)) {
                case 0x00: 
                    frameType = SongFrameType.String; 
                    data_str = Utility.PtrToUtf8String(NJB_Glue_Song_Frame_Get_Data_String(framePtr));
                    break;
                case 0x02: 
                    frameType = SongFrameType.UInt16; 
                    data_short = NJB_Glue_Song_Frame_Get_Data_UInt16(framePtr);
                    break;
                case 0x03: 
                    frameType = SongFrameType.UInt32;
                    data_int = NJB_Glue_Song_Frame_Get_Data_UInt32(framePtr);
                    break;
                default:
                    throw new ApplicationException("Unknown frame type");
            }
        }

        public IntPtr Handle {
            get { return handle; }
        }

        public string Label {
            get {
                return label;
            }
        }

        public SongFrameType FrameType {
            get {
                return frameType;
            }
        } 

        public string DataString {
            get {
                if(FrameType != SongFrameType.String) {
                    throw new ApplicationException("Frame data is not string");
                }
                
                return data_str;
            }
        }
        
        public ushort DataShort {
            get {
                if(FrameType != SongFrameType.UInt16) {
                    throw new ApplicationException("Frame data is not uint16");
                }
                
                return data_short;
            }
        }
        
        public uint DataInt {
            get {
                if(FrameType != SongFrameType.UInt32) {
                    throw new ApplicationException("Frame data is not uint32");
                }
                
                return data_int;
            }
        }
        
        public override string ToString()
        {
            string ret = Label + " = ";
            
            switch(FrameType) {
                case SongFrameType.String:
                    ret += DataString;
                    break;
                case SongFrameType.UInt16:
                    ret += DataShort;
                    break;
                case SongFrameType.UInt32:
                    ret += DataInt;
                    break;
                default:
                    return "INVALID FRAME";
            }
            
            return ret;
        }
    }
}
