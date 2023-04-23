using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

public class DataChannel : Channel
{
    private readonly int _datachannel;
    
    public DataChannel(int datachannel)
    {
        _channel=datachannel;
        _datachannel = datachannel;
        Id = (IntPtr)_datachannel;
        setUserPointer(Id);
        if (instances == null)
            instances = new Dictionary<IntPtr, Channel>();

        instances[Id] = this;
    }
    
    public string label()
    {
        IntPtr strPtr = Marshal.AllocHGlobal(512);
        int size=512;
        DataChannel_label(_datachannel,strPtr,size);
        string label = Marshal.PtrToStringAnsi(strPtr);
        Marshal.FreeHGlobal(strPtr);
        return label;
    }

    [DllImport(DLL.DLL_NAME)]
    private static extern int DataChannel_label(int datachannel, IntPtr buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern void DataChannel_close(int datachannel);
}