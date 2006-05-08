/***************************************************************************
 *  Song.cs
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
    internal struct njb_songid_t
    {
        public UInt32 trid;
        public UInt16 nframes;
        public IntPtr first;
        public IntPtr last;
        public IntPtr cur;
        public IntPtr next;
    }
    
    internal sealed class FrameName
    {
        public static readonly string 
            Album = "ALBUM",
            Artist = "ARTIST",
            Bitrate = "BITRATE",
            Codec = "CODEC", 
            Comment = "COMMENT",
            FileName = "FNAME",
            Folder = "FOLDER",
            Genre = "GENRE",
            Length = "LENGTH",
            Protected = "PlayOnly",
            FileSize = "FILE SIZE",
            Title = "TITLE",
            TrackNumber = "TRACK NUM",
            Year = "YEAR";
    }
    
    public class Song : IDisposable
    {
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_New();
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Destroy(HandleRef handle);   
        
        [DllImport("libnjb")]
        private static extern void NJB_Songid_Reset_Getframe(HandleRef handle);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Songid_Getframe(HandleRef handle);
        
        [DllImport("libnjb")]
        private static extern void NJB_Songid_Addframe(HandleRef handle, IntPtr frame);
        
        private HandleRef handle;
        private int id;
        private int nframes;
        private Device device;
        
        public Song(Device device) : this(NJB_Songid_New(), device)
        {
        }
        
        public Song(IntPtr songidPtr, Device device)
        {
            handle = new HandleRef(this, songidPtr);
            njb_songid_t songidRaw = (njb_songid_t)Marshal.PtrToStructure(songidPtr, typeof(njb_songid_t));
            id = (int)songidRaw.trid;
            nframes = (int)songidRaw.nframes;
            this.device = device;
        }
        
        internal HandleRef Handle {
            get {
                return handle;
            }
        }
        
        public Device Device {
            get {
                return device;
            }
        }
        
        public int Id {
            get {
                return id;
            }
        }
        
        public int FrameCount {
            get {
                return nframes;
            }
        }
        
        private SongFrame FindFrame(string label)
        {
            return SongFrame.Find(this, label);
        }
        
        private string GetFrameString(string label)
        {
            SongFrame frame = FindFrame(label);
            if(frame == null) {
                return null;
            }

            switch(frame.FrameType) {
                case SongFrameType.UInt16:
                    return Convert.ToString(frame.DataShort);
                case SongFrameType.UInt32:
                    return Convert.ToString(frame.DataInt);
                default:
                    return frame.DataString;
            }
        }
        
        private ushort GetFrameShort(string label)
        {
            SongFrame frame = FindFrame(label);
            if(frame == null) {
                return 0;
            }
                 
            switch(frame.FrameType) {
                case SongFrameType.String:
                    return Convert.ToUInt16(frame.DataString);
                case SongFrameType.UInt32:
                    return (ushort)frame.DataInt;
                default:
                    return frame.DataShort;
            }
      }
        
        private uint GetFrameInt(string label)
        {
            SongFrame frame = FindFrame(label);
            if(frame == null) {
                return 0;
            }
             
            switch(frame.FrameType) {
                case SongFrameType.String:
                    return Convert.ToUInt32(frame.DataString);
                case SongFrameType.UInt16:
                    return (uint)frame.DataShort;
                default:
                    return frame.DataInt;
            }
        }
        
        private void AddFrame(SongFrame frame)
        {
            if(frame == null) {
                return;
            }
            
            NJB_Songid_Addframe(Handle, frame.Handle);
        }
        
        private void AddFrame(string label, string value)
        {
            AddFrame(SongFrame.New(label, value));
        }
        
        private void AddFrame(string label, uint value)
        {
            AddFrame(SongFrame.New(label, value));
        }
        
        private void AddFrame(string label, ushort value)
        {
            AddFrame(SongFrame.New(label, value));
        }
        
        public string Album {
            get { return GetFrameString(FrameName.Album); }
            set { AddFrame(FrameName.Album, value); }
        }
        
        public string Artist {
            get { return GetFrameString(FrameName.Artist); }
            set { AddFrame(FrameName.Artist, value); }
        }
        
        public uint Bitrate {
            get { return GetFrameInt(FrameName.Bitrate); }
            set { AddFrame(FrameName.Bitrate, value); }
        }
        
        public string Codec {
            get { return GetFrameString(FrameName.Codec); }
            set { AddFrame(SongFrame.NewCodec(value)); }
        }
        
        public string Comment {
            get { return GetFrameString(FrameName.Comment); }
            set { AddFrame(FrameName.Comment, value); }
        }
        
        public string FileName {
            get { return GetFrameString(FrameName.FileName); }
            set { AddFrame(FrameName.FileName, value); }
        }
        
        public uint FileSize {
            get { return GetFrameInt(FrameName.FileSize); }
            set { AddFrame(FrameName.FileSize, value); }
        }
        
        public string Folder {
            get { return GetFrameString(FrameName.Folder); }
            set { AddFrame(FrameName.Folder, value); }
        }
        
        public string Genre {
            get { return GetFrameString(FrameName.Genre); }
            set { AddFrame(FrameName.Genre, value); }
        }
        
        public TimeSpan Duration {
            get { return new TimeSpan(GetFrameShort(FrameName.Length) * TimeSpan.TicksPerSecond); }
            set { AddFrame(FrameName.Length, (ushort)value.TotalSeconds); }
        }
        
        public bool IsProtected {
            get { return GetFrameShort(FrameName.Protected) != 0; }
            set { AddFrame(FrameName.Protected, (uint)(value == true ? 1 : 0)); }
        }
        
        public string Title {
            get { return GetFrameString(FrameName.Title); }
            set { AddFrame(FrameName.Title, value); }
        }
        
        public ushort TrackNumber {
            get { return GetFrameShort(FrameName.TrackNumber); }
            set { AddFrame(FrameName.TrackNumber, value); }
        }
        
        public ushort Year {
            get { return GetFrameShort(FrameName.Year); }
            set { AddFrame(FrameName.Year, value); }
        }
        
        public ICollection GetFrames()
        {
            ArrayList frames = new ArrayList();
            IntPtr framePtr = IntPtr.Zero;

            NJB_Songid_Reset_Getframe(handle);

            while((framePtr = NJB_Songid_Getframe(handle)) != IntPtr.Zero) {
                frames.Add(new SongFrame(framePtr));
            }

            return frames;
        }
        
        public void Dispose()
        {
            NJB_Songid_Destroy(handle);
        }
        
        public override string ToString()
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            
            text.AppendFormat("Song ID     : {0}\n", Id);
            text.AppendFormat("Frame Count : {0}\n", FrameCount);

            foreach(SongFrame frame in GetFrames()) {
                text.AppendFormat("  {0}\n", frame);
            }
            
            return text.ToString();
        }
    }
    
    public sealed class Codec
    {
        public static readonly string Mp3 = "MP3";
        public static readonly string Wma = "WMA";
        public static readonly string Wav = "WAV";
        public static readonly string Audible = "AA";
    }
}
