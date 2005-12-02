/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  Song.cs
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
        
        private HandleRef handle;
        private int id;
        private int nframes;
        
        public Song() : this(NJB_Songid_New())
        {
        }
        
        public Song(IntPtr songidPtr)
        {
            handle = new HandleRef(this, songidPtr);
            njb_songid_t songidRaw = (njb_songid_t)Marshal.PtrToStructure(songidPtr, typeof(njb_songid_t));
            id = (int)songidRaw.trid;
            nframes = (int)songidRaw.nframes;
        }
        
        internal HandleRef Handle {
            get {
                return handle;
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
            
            return frame.DataString;
        }
        
        private ushort GetFrameShort(string label)
        {
            SongFrame frame = FindFrame(label);
            if(frame == null) {
                return 0;
            }
            
            return frame.DataShort;
        }
        
        private uint GetFrameInt(string label)
        {
            SongFrame frame = FindFrame(label);
            if(frame == null) {
                return 0;
            }
            
            return frame.DataInt;
        }
        
        public string Album {
            get {
                return GetFrameString(FrameName.Album);
            }
        }
        
        public string Artist {
            get {
                return GetFrameString(FrameName.Artist);
            }
        }
        
        public uint Bitrate {
            get {
                return GetFrameInt(FrameName.Bitrate);
            }
        }
        
        public string Codec {
            get {
                return GetFrameString(FrameName.Codec);
            }
        }
        
        public string Comment {
            get {
                return GetFrameString(FrameName.Comment);
            }
        }
        
        public string FileName {
            get {
                return GetFrameString(FrameName.FileName);
            }
        }
        
        public uint FileSize {
            get {
                return GetFrameInt(FrameName.FileSize);
            }
        }
        
        public string Folder {
            get {
                return GetFrameString(FrameName.Folder);
            }
        }
        
        public string Genre {
            get {
                return GetFrameString(FrameName.Genre);
            }
        }
        
        public ushort Length {
            get {
                return GetFrameShort(FrameName.Length);
            }
        }
        
        public bool IsProtected {
            get {
                return GetFrameShort(FrameName.Protected) != 0;
            }
        }
        
        public string Title {
            get {
                return GetFrameString(FrameName.Title);
            }
        }
        
        public ushort TrackNumber {
            get {
                return GetFrameShort(FrameName.TrackNumber);
            }
        }
        
        public ushort Year {
            get {
                return GetFrameShort(FrameName.Year);
            }
        }
        
        public SongFrame [] Frames {
            get {
                ArrayList frames = new ArrayList();
                IntPtr framePtr = IntPtr.Zero;
            
                NJB_Songid_Reset_Getframe(handle);
            
                while((framePtr = NJB_Songid_Getframe(handle)) != IntPtr.Zero) {
                    frames.Add(new SongFrame(framePtr));
                }
                
                return frames.ToArray(typeof(SongFrame)) as SongFrame [];
            }
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

            foreach(SongFrame frame in Frames) {
                text.AppendFormat("  {0}\n", frame);
            }
            
            return text.ToString();
        }
    }
}
