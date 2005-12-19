/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  NjbTest.cs
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
using Njb;

public class Test
{
    public static void Assert(bool assert, string msg)
    {
        if(!assert) {
            throw new ApplicationException("Assertion failed: " + msg);
        }
    }
    
    public static void Main(string [] args)
    {
        Discoverer discoverer = new Discoverer();
        //Global.Debug = Global.DebugFlags.ALL;
        
        foreach(Device device in discoverer) {
            try {
                device.Open();
            } catch(ApplicationException) {
               device.ForeachError(delegate(string error) {
                    Console.WriteLine(error);
                });
                
                continue;
            }
            
            device.Capture();
            
            Console.WriteLine(device);

            if(args.Length == 0) {
                foreach(Song song in device.GetSongs()) {
                    Console.WriteLine(song);
                }
            } else {
                bool get_all = args[0] == "all";
                
                foreach(string sid in args) {
                    int id =0;
                    
                    if(!get_all) {
                        try {
                            id = Convert.ToInt32(sid);
                        } catch(Exception) {
                            continue;
                        }
                    }
                    
                    foreach(Song song in device.GetSongs()) {
                        if(song.Id != id && !get_all) {
                            continue;
                        }
                        
                        string filename = song.TrackNumber.ToString("00") + ". " + song.Artist 
                            + " - " + song.Title + "." + song.Codec.ToLower();
                        
                        device.ReadProgressChanged += OnProgress;
                        device.ReadSong(song, filename);
                        device.ReadProgressChanged -= OnProgress;
                        
                        Console.WriteLine("");
                    }
                }
            }

            device.Release();
            device.Dispose();
        }
    }
    
    public static void OnProgress(object o, TransferProgressArgs args)
    {
        Console.Write("Transferring {0} - {1}: {2}%\r", args.Song.Artist, args.Song.Title, 
            (args.Current * 100) / args.Total);
    }
}
