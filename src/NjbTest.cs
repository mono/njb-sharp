/***************************************************************************
 *  NjbTest.cs
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
using Njb;

public class Test
{
    public static void HalFdiDump()
    {
        Console.WriteLine("    <match key=\"info.bus\" string=\"usb\">\n\n");
        Console.WriteLine("    <!-- Begin NJB devices (generated by njb-sharp for libnjb compatible devices) -->\n");
        foreach(DeviceId device in DeviceId.ListAll) {
            Console.WriteLine("      <!-- {0} -->", device.Name);
            Console.WriteLine("      <match key=\"usb.vendor_id\" int=\"0x{0:x4}\">", device.VendorId);
            Console.WriteLine("        <match key=\"usb.product_id\" int=\"0x{0:x4}\">", device.ProductId);
            Console.WriteLine("          <append key=\"info.capabilities\" type=\"strlist\">portable_audio_player</append>");
            Console.WriteLine("          <merge key=\"info.category\" type=\"string\">portable_audio_player</merge>");
            Console.WriteLine("          <merge key=\"portable_audio_player.type\" type=\"string\">njb</merge>");
            Console.WriteLine("          <merge key=\"portable_audio_player.access_method\" type=\"string\">user</merge>");
            
            foreach(string output_format in device.OutputFormats) {
                Console.WriteLine("          <append key=\"portable_audio_player.output_formats\" type=\"strlist\">{0}</append>", output_format);
            }
            
            foreach(string input_format in device.InputFormats) {
                Console.WriteLine("          <append key=\"portable_audio_player.input_formats\" type=\"strlist\">{0}</append>", input_format);
            }
            
            Console.WriteLine("        </match>");
            Console.WriteLine("      </match>\n"); 
        }
        
        Console.WriteLine("    <!-- End NJB devices -->");
        
        Console.WriteLine("\n\n    </match>");
    }

    public static void Assert(bool assert, string msg)
    {
        if(!assert) {
            throw new ApplicationException("Assertion failed: " + msg);
        }
    }
    
    public static void Main(string [] args)
    {
        if(args[0] == "--hal-fdi-dump") {
            HalFdiDump();
            return;
        }
    
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
                        
                        device.ProgressChanged += OnProgress;
                        device.ReadSong(song, filename);
                        device.ProgressChanged -= OnProgress;
                        
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
