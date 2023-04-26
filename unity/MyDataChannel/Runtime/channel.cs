using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class Channel {
        
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
        public int id {get; protected set;}
        
        public Action<int> openCallback {get; protected set;}
        public Action<int,string> errorCallback {get; protected set;}
        public Action<int,byte[],int> messageCallback {get; protected set;}
        public Action<int> bufferedAmountLow {get; protected set;}
        
        public void SetBufferedAmountLowThreshold(int amount) => Channel_setBufferedAmountLowThreshold(id, amount);
        
        public int BufferedAmount() => Channel_bufferedAmount(id);
    
        public virtual bool Send(byte[] data, int size) => Channel_send(id, data, size);
        
        protected virtual void SetUserPointer() => Channel_setUserPointer(id, (IntPtr)id);
    
        public virtual void OnOpen(Action<int> callback) {
            openCallback = callback;
            Channel_onOpen(id, OnOpenCallback);
        }
    
        public virtual void OnError(Action<int,string> callback) {
            errorCallback = callback;
            Channel_onError(id, OnErrorCallback);
        }
    
        public virtual void OnMessage(Action<int,byte[], int> callback) {
            messageCallback = callback;
            Channel_onMessage(id, OnMessageCallback);
        }
        
        public void OnBufferedAmountLow(Action<int> callback) {
            bufferedAmountLow = callback;
            Channel_onBufferedAmountLow(id, OnBufferedAmountLowCallback);
        }
        
    #if !UNITY_WEBGL
        [MonoPInvokeCallback(typeof(OpenCallback))]
        protected static void OnOpenCallback(int id, IntPtr ptr) {
            instances?[ptr].openCallback((int)ptr);
        }
    
        [MonoPInvokeCallback(typeof(ErrorCallback))]
        protected static void OnErrorCallback(int id, string error, IntPtr ptr) {
            instances?[ptr].errorCallback((int)ptr, error);
        }
    
        [MonoPInvokeCallback(typeof(MessageCallback))]
        protected static void OnMessageCallback(int id, IntPtr message, int size, IntPtr ptr) {
            byte[] managedArray = new byte[size];
            Marshal.Copy(message, managedArray, 0, size);
            instances?[ptr].messageCallback((int)ptr, managedArray, size);
        }
        
        [MonoPInvokeCallback(typeof(BufferedAmountLowCallBack))]
        protected static void OnBufferedAmountLowCallback(int id, IntPtr ptr) {
            instances?[ptr].bufferedAmountLow((int)ptr);
        }
    #else
        [MonoPInvokeCallback(typeof(OpenCallback))]
        protected static void OnOpenCallback(IntPtr ptr) {
            instances?[ptr].openCallback((int)ptr);
        }
    
        [MonoPInvokeCallback(typeof(ErrorCallback))]
        protected static void OnErrorCallback(string error, IntPtr ptr) {
            instances?[ptr].errorCallback((int)ptr,error);
        }
    
        [MonoPInvokeCallback(typeof(MessageCallback))]
        protected static void OnMessageCallback(IntPtr pMessage, int size, IntPtr ptr) {
            byte[] managedArray = new byte[size];
            Marshal.Copy(pMessage, managedArray, 0, size);
            instances?[ptr].messageCallback((int)ptr,managedArray, size);
        }
        
        [MonoPInvokeCallback(typeof(BufferedAmountLowCallBack))]
        protected static void OnBufferedAmountLowCallback(IntPtr ptr) {
            instances?[ptr].bufferedAmountLow((int)ptr);
        }
    #endif

        [DllImport(DLL.DLL_NAME)]
        private static extern bool Channel_send(int id, byte[] data, int size);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void Channel_onOpen(int id, OpenCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void Channel_onError(int id, ErrorCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void Channel_onMessage(int id, MessageCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void Channel_onBufferedAmountLow(int id, BufferedAmountLowCallBack callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void Channel_setBufferedAmountLowThreshold(int id, int amount);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern int Channel_bufferedAmount(int id);
        
        [DllImport(DLL.DLL_NAME)]
        protected static extern void Channel_setUserPointer(int id, IntPtr ptr);
    }
}