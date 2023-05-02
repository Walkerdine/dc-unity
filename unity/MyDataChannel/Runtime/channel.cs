using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class Channel {
        
        protected delegate void OpenCallback(int id, int userId);
        protected delegate void ErrorCallback(int id, string error, int userId);
        protected delegate void MessageCallback(int id, IntPtr message, int size, int userId);
        protected delegate void BufferedAmountLowCallBack(int id, int userId);
    
        protected int _id;

        protected Action<int> openCallback;
        protected Action<int, string> errorCallback;
        protected Action<int, byte[], int> messageCallback;
        protected Action<int> bufferedAmountLow;
        
        public int Id {get; protected set;}
        
        public void SetBufferedAmountLowThreshold(int amount) => Channel_setBufferedAmountLowThreshold(_id, amount);
        
        public int BufferedAmount() => Channel_bufferedAmount(_id);
    
        public virtual bool Send(byte[] data, int size) => Channel_send(_id, data, size);
    
        [DllImport(DLL.DLL_NAME)]
        protected static extern bool Channel_send(int id, byte[] data, int size);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_onOpen(int id, OpenCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_onError(int id, ErrorCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_onMessage(int id, MessageCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_onBufferedAmountLow(int id, BufferedAmountLowCallBack callback);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_setBufferedAmountLowThreshold(int id, int amount);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern int Channel_bufferedAmount(int id);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_setUserPointer(int id, int userId);
    }
}