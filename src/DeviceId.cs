/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  DeviceId.cs
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

namespace Njb
{
    public class DeviceId
    {
        private static DeviceId [] device_id_list = {
            //           Full Device Name                            Display/Short Name    Vendor ID   Product ID   Output formats                  Input Formats 
            new DeviceId("Creative Nomad Jukebox",                   "Nomad Jukebox",      0x0471,     0x0222,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox 2",                 "Nomad Jukebox",      0x041e,     0x4100,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox 3",                 "Nomad Jukebox",      0x041e,     0x4101,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen",               "Zen Jukebox",        0x041e,     0x4108,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen USB 2.0",       "Zen Jukebox",        0x041e,     0x410b,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen NX",            "Zen NX Jukebox",     0x041e,     0x4109,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen Xtra",          "Zen Xtra Jukebox",   0x041e,     0x4110,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Dell Digital Jukebox",                     "Dell Jukebox",       0x041e,     0x4111,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen Touch",         "Nomad Jukebox",      0x041e,     0x411b,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Zen (Zen Micro variant)",         "Zen Micro",          0x041e,     0x411d,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Nomad Jukebox Zen Micro",         "Zen Micro",          0x041e,     0x411e,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Second Generation Dell Digital Jukebox",   "Dell Jukebox",       0x041e,     0x4126,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Dell Pocket DJ",                           "Dell Pocket DJ",     0x041e,     0x4127,      new string [] { "audio/mpeg" }, null),
            new DeviceId("Creative Zen Sleek",                       "Zen Sleek",          0x041e,     0x4136,      new string [] { "audio/mpeg" }, null)
        };
        
        private static string [] EmptyArray = new string [] { };

        public static DeviceId [] ListAll {
            get { return device_id_list; }
        }
        
        public static bool IsNjbDevice(short vendorId, short productId)
        {
            return GetDeviceId(vendorId, productId) != null;
        }
        
        public static DeviceId GetDeviceId(short vendorId, short productId)
        {
            foreach(DeviceId id in ListAll) {
                if(id.VendorId == vendorId && id.ProductId == productId) {
                    return id;
                }
            }
            
            return null;
        }
        
        private string name;
        private string display_name;
        private short vendor_id;
        private short product_id;
        private string [] output_formats;
        private string [] input_formats;
        
        private DeviceId(string name, string displayName, short vendorId, short productId, 
            string [] outputFormats, string [] inputFormats)
        {
            this.name = name;
            this.display_name = displayName;
            this.vendor_id = vendorId;
            this.product_id = productId;
            this.output_formats = outputFormats;
            this.input_formats = inputFormats;
        }
        
        public string Name {
            get { return name; }
        }
        
        public string DisplayName {
            get { return display_name; }
        }
        
        public short VendorId {
            get { return vendor_id; }
        }
        
        public short ProductId {
            get { return product_id; }
        }
        
        public string [] OutputFormats {
            get { return output_formats == null ? EmptyArray : output_formats; }
        }
        
        public string [] InputFormats {
            get { return input_formats == null ? EmptyArray : input_formats; }
        }
    }
}
