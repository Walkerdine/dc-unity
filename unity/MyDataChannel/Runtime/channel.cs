using System;
using System.Runtime.InteropServices;


#if !UNITY_WEBGL
public delegate void OpenCallback(int id, IntPtr ptr);
public delegate void ErrorCallback(int id, string error, IntPtr ptr);
public delegate void MessageCallback(int id, string message, int size, IntPtr ptr);
#else
public delegate void OpenCallback(IntPtr ptr);
public delegate void ErrorCallback(IntPtr pError, IntPtr ptr);
public delegate void MessageCallback(IntPtr pMessage, int size, IntPtr ptr);
#endif

public class Channel
{
    public int Id { get; protected set; }
    public string error { get; set; }
    public string message { get;  set; }
    public int size { get; set; }
    
    public Action open_callback;
    public Action<string> error_callback;
    public Action<string,int> message_callback;

    public virtual bool send(byte[] data, int size) => Channel_send(Id, data, size);
    
    public virtual void onOpenCallback(OpenCallback callback) => Channel_onOpen(Id, Marshal.GetFunctionPointerForDelegate(callback));
    
    public virtual void onErrorCallback(ErrorCallback callback) => Channel_onError(Id, Marshal.GetFunctionPointerForDelegate(callback));
    
    public virtual void onMessageCallback(MessageCallback callback) => Channel_onMessage(Id, Marshal.GetFunctionPointerForDelegate(callback));
    
    public virtual void setUserPointer(IntPtr ptr) => Channel_setUserPointer(Id, ptr);

    public void onOpen(Action callback)
    {
        open_callback = callback;
    }

    public void onError(Action<string> callback)
    {
        error_callback = callback;
    }

    public void onMessage(Action<string, int> callback)
    {
        message_callback = callback;
    }

    [DllImport(DLL.DLL_NAME)]
    private static extern bool Channel_send(int channel, byte[] data, int size);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onOpen(int channel, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onError(int channel, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onMessage(int channel, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_setUserPointer(int channel, IntPtr ptr);
}

