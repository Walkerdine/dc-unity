using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DataChannel : Channel
{
    private readonly int _datachannel;
    
    public DataChannel(int datachannel)
    {
        _datachannel = datachannel;
        Id = datachannel;
        setUserPointer((IntPtr)Id);
        ChannelCallbackBridge.SetInstance(this);
    }
    
    public string label()
    {
        //IntPtr strPtr;
        //int buffer_size;
        //DataChannel_label(_datachannel, strPtr, buffer_size);
        return "";
    }

    [DllImport(DLL.DLL_NAME)]
    private static extern int DataChannel_label(int datachannel, IntPtr buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern void DataChannel_close(int datachannel);
}