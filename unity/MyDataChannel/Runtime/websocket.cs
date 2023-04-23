using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

public class WebSocket : Channel
{
    private int _websocket;
    
    public WebSocket(string url)
    {
        IntPtr pUrl = Marshal.StringToHGlobalAnsi(url);
        _websocket = WebSocket_open(pUrl);
        Id = (IntPtr)_websocket;
        setUserPointer(Id);
        Marshal.FreeHGlobal(pUrl);
        if (instances == null)
            instances = new Dictionary<IntPtr, Channel>();

        instances[Id] = this;
    }
    
    public WebSocket(int id)
    {
        Id = (IntPtr)id;
        _websocket = id;
        setUserPointer(Id);
        if (instances == null)
            instances = new Dictionary<IntPtr, Channel>();

        instances[Id] = this;
    }

    ~WebSocket()
    {
        close();
    }

    public void close() => WebSocket_close(_websocket);

    public override bool send(byte[] data, int size) => WebSocket_send(_websocket, data, size);
    
    public override void onOpen(Action callback)
    {
        instances[Id].open_callback = callback;
        WebSocket_onOpen(_websocket, OnOpen);
    }

    public override void onError(Action<string> callback)
    {
        instances[Id].error_callback = callback;
        WebSocket_onError(_websocket, OnError);
    }

    public override void onMessage(Action<byte[], int> callback)
    {
        instances[Id].message_callback = callback;
        WebSocket_onMessage(_websocket, OnMessage);
    }
    
    public override void setUserPointer(IntPtr ptr) => WebSocket_setUserPointer(_websocket, ptr);

    [DllImport(DLL.DLL_NAME)]
    private static extern int WebSocket_open(IntPtr url);

    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_close(int websocket);

    [DllImport(DLL.DLL_NAME)]
    private static extern bool WebSocket_send(int websocket, byte[] data, int size);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onOpen(int websocket, OpenCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onError(int websocket, ErrorCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_onMessage(int websocket, MessageCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocket_setUserPointer(int websocket, IntPtr ptr);
}