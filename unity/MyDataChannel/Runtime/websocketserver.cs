using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

#if !UNITY_WEBGL
public delegate void WebSocketClientCallback(int wsserver, int ws, IntPtr ptr);

[StructLayout(LayoutKind.Sequential)]
public struct rtcWsServerConfiguration
{
    public ushort port;
    [MarshalAs(UnmanagedType.Bool)]
    public bool enableTls;
    [MarshalAs(UnmanagedType.LPStr)]
    public string certificatePemFile;
    [MarshalAs(UnmanagedType.LPStr)]
    public string keyPemFile;
    [MarshalAs(UnmanagedType.LPStr)]
    public string keyPemPass;
    [MarshalAs(UnmanagedType.LPStr)]
    public string bindAddress;
}

public class WebSocketServer
{
    private int _websocketserver;
    public Action<int,int> client_callback;
    private static WebSocketServer instance;
    
    [MonoPInvokeCallback(typeof(WebSocketClientCallback))]
    public static void OnClient(int wsserver, int ws, IntPtr ptr)
    {
        instance.client_callback(wsserver, ws);
    }
    
    public WebSocketServer(ref rtcWsServerConfiguration config, Action<int,int> callback)
    {
        instance = this;
        client_callback = callback;
        _websocketserver = WebSocketServer_new(ref config, OnClient);
    }

    ~WebSocketServer()
    {

    }
    
    public int delete() => WebSocketServer_delete(_websocketserver);

    public int port() => WebSocketServer_getPort(_websocketserver);
    
    private void setUserPointer(IntPtr ptr) => WebSocketServer_setUserPointer(_websocketserver, ptr);

    [DllImport(DLL.DLL_NAME)]
    private static extern int WebSocketServer_new(ref rtcWsServerConfiguration config, WebSocketClientCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern int WebSocketServer_delete(int wsserver);

    [DllImport(DLL.DLL_NAME)]
    private static extern int WebSocketServer_getPort(int wsserver);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void WebSocketServer_setUserPointer(int wsserver, IntPtr ptr);
}
#endif