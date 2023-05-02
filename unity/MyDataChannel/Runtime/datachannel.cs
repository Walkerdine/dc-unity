using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class DataChannel : Channel {
        
        private static Dictionary<int, DataChannel> instances = new Dictionary<int, DataChannel>();
        
        public DataChannel(int datachannel) {
            _id = datachannel;
            instances[_id] = this;
            SetUserId(_id);
        }
        
        ~DataChannel(){
            DataChannel_close(_id);
        }
        
        public string Label() {
            IntPtr strPtr = Marshal.AllocHGlobal(512);
            int size=512;
            DataChannel_label(_id, strPtr, size);
            string label = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return label;
        }
        
        public void SetUserId(int userId) {
            Id = userId;
            Channel_setUserPointer(_id, Id);
        }
        
        public void OnOpen(Action<int> callback) {
            openCallback = callback;
            Channel_onOpen(_id, OnOpenCallback);
        }
    
        public void OnError(Action<int, string> callback) {
            errorCallback = callback;
            Channel_onError(_id, OnErrorCallback);
        }
    
        public void OnMessage(Action<int, byte[], int> callback) {
            messageCallback = callback;
            Channel_onMessage(_id, OnMessageCallback);
        }
        
        public void OnBufferedAmountLow(Action<int> callback) {
            bufferedAmountLow = callback;
            Channel_onBufferedAmountLow(_id, OnBufferedAmountLowCallback);
        }
        
        [MonoPInvokeCallback(typeof(OpenCallback))]
        private static void OnOpenCallback(int id, int userId) {
            instances?[id].openCallback(userId);
        }
    
        [MonoPInvokeCallback(typeof(ErrorCallback))]
        private static void OnErrorCallback(int id, string error, int userId) {
            instances?[id].errorCallback(userId, error);
        }
    
        [MonoPInvokeCallback(typeof(MessageCallback))]
        private static void OnMessageCallback(int id, IntPtr message, int size, int userId) {
            byte[] managedArray = new byte[size];
            Marshal.Copy(message, managedArray, 0, size);
            instances?[id].messageCallback(userId, managedArray, size);
        }
        
        [MonoPInvokeCallback(typeof(BufferedAmountLowCallBack))]
        private static void OnBufferedAmountLowCallback(int id, int userId) {
            instances?[id].bufferedAmountLow(userId);
        }
        
        [DllImport(DLL.DLL_NAME)]
        private static extern int DataChannel_label(int id, IntPtr buffer, int size);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void DataChannel_close(int id);
    }
}