using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

public class Channel
{
#if !UNITY_WEBGL
    public delegate void OpenCallback(int id, IntPtr ptr);
    public delegate void ErrorCallback(int id, string error, IntPtr ptr);
    public delegate void MessageCallback(int id, IntPtr message, int size, IntPtr ptr);
    public delegate void BufferedAmountLowCallBack(int id, IntPtr ptr);
#else
    public delegate void OpenCallback(IntPtr ptr);
    public delegate void ErrorCallback(string error, IntPtr ptr);
    public delegate void MessageCallback(IntPtr pMessage, int size, IntPtr ptr);
    public delegate void BufferedAmountLowCallBack(IntPtr ptr);
#endif

    protected static Dictionary<IntPtr, Channel> instances;
    public int _channel;
    public IntPtr Id { get; protected set; }
    
    public Action open_callback;
    public Action<string> error_callback;
    public Action<byte[],int> message_callback;
    public Action buffered_amount_low;

    public virtual bool send(byte[] data, int size) => Channel_send(_channel, data, size);
    
    public virtual void setUserPointer(IntPtr ptr) => Channel_setUserPointer(_channel, ptr);

    public virtual void onOpen(Action callback)
    {
        instances[Id].open_callback = callback;
        Channel_onOpen(_channel, OnOpen);
    }

    public virtual void onError(Action<string> callback)
    {
        instances[Id].error_callback = callback;
        Channel_onError(_channel, OnError);
    }

    public virtual void onMessage(Action<byte[], int> callback)
    {
        instances[Id].message_callback = callback;
        Channel_onMessage(_channel, OnMessage);
    }
    
    public int bufferedAmount() => Channel_bufferedAmount(_channel);
    
    void setBufferedAmountLowThreshold(int amount) => Channel_setBufferedAmountLowThreshold(_channel, amount);
    
    public void onBufferedAmountLow(Action callback)
    {
        instances[Id].buffered_amount_low = callback;
        Channel_onBufferedAmountLow(_channel, OnBufferedAmountLow);
    }

    [DllImport(DLL.DLL_NAME)]
    private static extern bool Channel_send(int channel, byte[] data, int size);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onOpen(int channel, OpenCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onError(int channel, ErrorCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onMessage(int channel, MessageCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_onBufferedAmountLow(int channel, BufferedAmountLowCallBack callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_setBufferedAmountLowThreshold(int id, int amount);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern int Channel_bufferedAmount(int id);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void Channel_setUserPointer(int channel, IntPtr ptr);
    
#if !UNITY_WEBGL
    [MonoPInvokeCallback(typeof(OpenCallback))]
    public static void OnOpen(int id, IntPtr ptr)
    {
        instances?[ptr].open_callback();
    }

    [MonoPInvokeCallback(typeof(ErrorCallback))]
    public static void OnError(int id, string error, IntPtr ptr)
    {
        instances?[ptr].error_callback(error);
    }

    [MonoPInvokeCallback(typeof(MessageCallback))]
    public static void OnMessage(int id, IntPtr message, int size, IntPtr ptr)
    {
        byte[] managedArray = new byte[size];
        Marshal.Copy(message, managedArray, 0, size);
        instances?[ptr].message_callback(managedArray,size);
    }
    
    [MonoPInvokeCallback(typeof(BufferedAmountLowCallBack))]
    public static void OnBufferedAmountLow(int id, IntPtr ptr)
    {
        instances?[ptr].buffered_amount_low();
    }
#else
    [MonoPInvokeCallback(typeof(OpenCallback))]
    public static void OnOpen(IntPtr ptr)
    {
        instances?[ptr].open_callback();
    }

    [MonoPInvokeCallback(typeof(ErrorCallback))]
    public static void OnError(string error, IntPtr ptr)
    {
        instances?[ptr].error_callback(error);
    }

    [MonoPInvokeCallback(typeof(MessageCallback))]
    public static void OnMessage(IntPtr pMessage, int size, IntPtr ptr)
    {
        byte[] managedArray = new byte[size];
        Marshal.Copy(pMessage, managedArray, 0, size);
        instances?[ptr].message_callback(managedArray, size);
    }
    
    [MonoPInvokeCallback(typeof(BufferedAmountLowCallBack))]
    public static void OnBufferedAmountLow(IntPtr ptr)
    {
        instances?[ptr].buffered_amount_low();
    }
#endif
}

