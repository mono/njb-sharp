/***************************************************************************
 *  njb-glue.c
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

#include <libnjb.h>
#include <usb.h>

static njb_t *njb_devices[NJB_MAX_DEVICES];

njb_t *NJB_Glue_Get_Device_Array()
{
    return (njb_t *)njb_devices;
}

njb_t *NJB_Glue_Get_Device(int i)
{
    return (njb_t *)&njb_devices[i];
}

const char *NJB_Glue_Device_Get_Usb_Filename(njb_t *njb)
{
    return njb->device->filename;
}

const char *NJB_Glue_Device_Get_Usb_Bus_Path(njb_t *njb)
{
    return njb->device->bus->dirname;
}

const char *NJB_Glue_Song_Frame_Get_Label(njb_songid_frame_t *frame)
{
    return frame->label;
}

u_int8_t NJB_Glue_Song_Frame_Get_Type(njb_songid_frame_t *frame)
{
    return frame->type;
}

const char *NJB_Glue_Song_Frame_Get_Data_String(njb_songid_frame_t *frame)
{
    return frame->data.strval;
}

u_int16_t NJB_Glue_Song_Frame_Get_Data_UInt16(njb_songid_frame_t *frame)
{
    return frame->data.u_int16_val;
}

u_int32_t NJB_Glue_Song_Frame_Get_Data_UInt32(njb_songid_frame_t *frame)
{
    return frame->data.u_int32_val;
}
