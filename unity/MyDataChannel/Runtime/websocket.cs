using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class WebSocket : Channel {

        private static Dictionary<int, WebSocket> instances = new Dictionary<int, WebSocket>();
    
        public WebSocket(string url) {
            _id = WebSocket_open(url);
            instances[_id] = this;
            SetUserId(_id);
        }
        
        public WebSocket(int ws) {
            _id = ws;
            instances[_id] = this;
            SetUserId(_id);
        }
    
        ~WebSocket() {
            WebSocket_close(_id);
        }
    
        public override bool Send(byte[] data, int size) => WebSocket_send(_id, data, size);
        
        public void OnOpen(Action<int> callback) {
            openCallback = callback;
            WebSocket_onOpen(_id, OnOpenCallback);
        }
    
        public void OnError(Action<int, string> callback) {
            errorCallback = callback;
            WebSocket_onError(_id, OnErrorCallback);
        }
    
        public void OnMessage(Action<int, byte[], int> callback) {
            messageCallback = callback;
            WebSocket_onMessage(_id, OnMessageCallback);
        }
        
        public void SetUserId(int userPtr) {
            Id = userPtr;
            WebSocket_setUserPointer(_id, Id);
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
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocket_open(string url);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocket_close(int id);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern bool WebSocket_send(int id, byte[] data, int size);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocket_onOpen(int id, OpenCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocket_onError(int id, ErrorCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocket_onMessage(int id, MessageCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocket_setUserPointer(int id, int userId);
    }
}