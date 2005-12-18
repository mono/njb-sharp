/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  Discoverer.cs
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
    public class Discoverer : IEnumerable
    {
        [DllImport("libnjb")]
        private static extern int NJB_Discover(IntPtr njbs, int limit, out int count);

        [DllImport("libnjbglue")]
        private static extern IntPtr NJB_Glue_Get_Device_Array();
   
        private IntPtr njbs;
        private Device [] devices;

        public Discoverer()
        {
            Global.Unicode = true;
            Rescan();
        }

        public void Rescan()
        {
            int count;

            if(NJB_Discover(NJB_Glue_Get_Device_Array(), 0, out count) == -1) {
                throw new ApplicationException("Could not run NJB Discover");
            }

            devices = new Device[count];
           
            for(int i = 0; i < count; i++) {
                devices[i] = new Device(this, i);
            }
        }

        internal IntPtr ArrayHandle {
            get {
                return njbs;
            }
        }

        public Device this [int i] {
            get {
                return devices[i];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new ArrayList(devices).GetEnumerator();
        }
    }
}
