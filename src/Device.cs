/* -*- Mode: csharp; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: t -*- */
/***************************************************************************
 *  Device.cs
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
    public delegate void ForeachErrorCallback(string error);
    
    public class Device
    {
        private Discoverer discoverer;
        private int index;

        [DllImport("libnjb")]
        private static extern int NJB_Open(IntPtr njb);

        [DllImport("libnjb")]
        private static extern void NJB_Close(IntPtr njb);
    
        [DllImport("libnjb")]
        private static extern int NJB_Capture(IntPtr njb);

        [DllImport("libnjb")]
        private static extern int NJB_Release(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern void NJB_Ping(IntPtr njb);
    
        [DllImport("libnjb")]
        private static extern int NJB_Error_Pending(IntPtr njb);
    
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Error_Geterror(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Get_Owner_String(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern int NJB_Set_Owner_String(IntPtr njb, 
            IntPtr strPtr);
        
        [DllImport("libnjb")]
        private static extern IntPtr NJB_Get_Device_Name(IntPtr njb, int type);

        [DllImport("libnjb")]
        private static extern int NJB_Get_Battery_Level(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern int NJB_Get_Battery_Charging(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern int NJB_Get_Auxpower(IntPtr njb);
        
        [DllImport("libnjb")]
        private static extern int NJB_Get_Disk_Usage(IntPtr njb, 
            out ulong btotal, out ulong bfree);
            
        [DllImport("libnjb")]
        private static extern int NJB_Get_Firmware_Revision(IntPtr njb,
            out byte major, out byte minor, out byte release);
            
        [DllImport("libnjb")]
        private static extern int NJB_Get_Hardware_Revision(IntPtr njb,
            out byte major, out byte minor, out byte release);
            
        [DllImport("libnjb")]
        private static extern int NJB_Get_SDMI_ID(IntPtr njb, IntPtr smdiid);
            
        [DllImport("libnjbglue")]
        private static extern IntPtr NJB_Glue_Get_Device(int index);

        public Device(Discoverer discoverer, int index)
        {
            this.discoverer = discoverer;
            this.index = index;
        }

        public bool Open()
        {
            return NJB_Open(Handle) != -1;
        }

        public void Close()
        {
            NJB_Close(Handle);
        }
        
        public bool Capture()
        {
            return NJB_Capture(Handle) != -1;
        }
        
        public bool Release()
        {
            return NJB_Release(Handle) != -1;
        }

        public void Ping()
        {
            NJB_Ping(Handle);
        }

        public void GetDiskUsage(out ulong bytesTotal, out ulong bytesFree)
        {
            if(NJB_Get_Disk_Usage(Handle, out bytesTotal, 
                out bytesFree) == -1) {
                bytesTotal = 0;
                bytesFree = 0;
            }
        }

        public IntPtr Handle
        {
            get {
                return NJB_Glue_Get_Device(index);
            }
        }

        public int Index
        {
            get {
                return index;
            }
        }

        public Discoverer Discoverer
        {
            get {
                return discoverer;
            }
        }

        public string Name
        {
            get {
                IntPtr ptr = NJB_Get_Device_Name(Handle, 0);
                return Utility.PtrToUtf8String(ptr);
            }
        }
        
        public string UsbName
        {
            get {
                IntPtr ptr = NJB_Get_Device_Name(Handle, 1);
                return Utility.PtrToUtf8String(ptr);
            }
        }

        public string Owner
        {
            get {
                IntPtr ptr = NJB_Get_Owner_String(Handle);
                return Utility.PtrToUtf8String(ptr);
            }
            
            set {
                IntPtr ptr = Utility.Utf8StringToPtr(value);
                if(NJB_Set_Owner_String(Handle, ptr) == -1) {
                    Marshal.FreeHGlobal(ptr);
                    throw new ApplicationException("Could not set owner");
                }
                
                Marshal.FreeHGlobal(ptr);       
            }
        }

        public int BatteryLevel
        {
            get {
                return NJB_Get_Battery_Level(Handle);
            }
        }
        
        public bool BatteryCharging
        {
            get {
                return NJB_Get_Battery_Charging(Handle) == 1;
            }
        }
        
        public bool AuxilaryPower
        {
            get {
                return NJB_Get_Auxpower(Handle) == 1;
            }
        }
        
        public ulong DiskFree
        {
            get {
                ulong total, free;
                GetDiskUsage(out total, out free);
                return free;
            }
        }
        
        public ulong DiskTotal
        {
            get {
                ulong total, free;
                GetDiskUsage(out total, out free);
                return total;
            }
        }
        
        public Revision FirmwareRevision
        {
            get {
                Revision rev = new Revision();
                
                if(NJB_Get_Firmware_Revision(Handle, out rev.Major,
                    out rev.Minor, out rev.Release) == 0) {
                    return rev;
                }
                
                return null;
            }
        }
        
        public Revision HardwareRevision
        {
            get {
                Revision rev = new Revision();
                
                if(NJB_Get_Hardware_Revision(Handle, out rev.Major,
                    out rev.Minor, out rev.Release) == 0) {
                    return rev;
                }
                
                return null;
            }
        } 
        
        public byte [] SdmiId 
        {
            get {
                IntPtr memalloc = Marshal.AllocHGlobal(16);
                
                if(NJB_Get_SDMI_ID(Handle, memalloc) == -1) {
                    Marshal.FreeHGlobal(memalloc);
                    return null;
                }
                
                byte [] sdmiid = new byte[16];
                
                for(int i = 0; i < 16; i++) {
                    sdmiid[i] = Marshal.ReadByte(memalloc, i);
                }
                
                Marshal.FreeHGlobal(memalloc);
                
                return sdmiid;
            }
        }
        
        public string SdmiIdString
        {
            get {
                string idstr = String.Empty;
                byte [] id = SdmiId;
                
                for(int i = 0; i < SdmiId.Length; i++) {
                    idstr += String.Format("{0:x2}", id[i]);
                }
                
                return idstr;
            }
        }
        
        public string NextError
        {
            get {
                IntPtr ptr = NJB_Error_Geterror(Handle);
                if(ptr == IntPtr.Zero) {
                    return null;
                }
                
                string error = Utility.PtrToUtf8String(ptr);

                if(error == null || error == String.Empty) {
                    return null;
                }

                return error;
            }
        }

        public bool IsErrorPending
        {
            get {
                return NJB_Error_Pending(Handle) != 0;
            }
        }

        public string [] ErrorsPending
        {
            get {
                if(!IsErrorPending) {
                    return null;
                }
            
                ArrayList errorList = new ArrayList();
                
                while(IsErrorPending) {
                    string error = NextError;

                    if(error != null) {
                        errorList.Add(error);
                    }
                }

                return errorList.ToArray(typeof(string)) as string [];
            }
        }
        
        public void ForeachError(ForeachErrorCallback callback)
        {
            foreach(string error in ErrorsPending) {
                callback(error);
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            
            text.AppendFormat("Index              {0}\n", Index);
            text.AppendFormat("DeviceName:        {0}\n", Name);
            text.AppendFormat("USB Name:          {0}\n", UsbName);
            text.AppendFormat("Owner:             {0}\n", Owner);
            text.AppendFormat("Battery Level:     {0}%\n", BatteryLevel);
            text.AppendFormat("Battery Charging:  {0}\n", BatteryCharging ? "YES" : "NO");
            text.AppendFormat("Aux. Power:        {0}\n", AuxilaryPower ? "YES" : "NO");
            text.AppendFormat("Disk Total:        {0}\n", DiskTotal);
            text.AppendFormat("Disk Free:         {0}\n", DiskFree);
            text.AppendFormat("Firmware Revision: {0}\n", FirmwareRevision);
            text.AppendFormat("Hardware Revision: {0}\n", HardwareRevision);
            text.AppendFormat("SMDI ID:           {0}\n", SdmiIdString);
                       
            return text.ToString();
        }
    }
}
