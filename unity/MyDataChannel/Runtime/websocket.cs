using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class WebSocket : Channel
{
    private int _websocket;

    public WebSocket()
    {
        
    }

    ~WebSocket()
    {
        close();
    }

    public void open(string url)
    {
        IntPtr pUrl = Marshal.StringToHGlobalAnsi(url);
        _websocket = WebSocket_open(pUrl);
        Marshal.FreeHGlobal(pUrl);
        Id = _websocket;
        setUserPointer((IntPtr)Id);
        ChannelCallbackBridge.SetInstance(this);
    }

    public void close() => WebSocket_close(_websocket);

    public override bool send(byte[] data, int size) => WebSocket_send(_websocket, data, size);

    public override void onOpenCallback(OpenCallback callback) => WebSocket_onOpen(_websocket, Marshal.GetFunctionPointerForDelegate(callback));
    
    public override void onErrorCallback(ErrorCallback callback) => WebSocket_onError(_websocket, Marshal.GetFunctionPointerForDelegate(callback));
    
    public override void onMessageCallback(MessageCallback callback) => WebSocket_onMessage(_websocket, Marshal.GetFunctionPointerForDelegate(callback));
    
    public override void setUserPointer(IntPtr ptr) => WebSocket_setUserPointer(_websocket, ptr);

    [DllImport(DLL.DLL_NAME)]
    private static extern int WebSocket_open(IntPtr url);

    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_close(int websocket);

    [DllImport(DLL.DLL_NAME)]
    private static extern bool WebSocket_send(int websocket, byte[] data, int size);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onOpen(int websocket, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onError(int websocket, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onMessage(int websocket, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_setUserPointer(int websocket, IntPtr ptr);
}