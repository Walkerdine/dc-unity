using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class WebSocket : Channel {   
    
        public WebSocket(string url) {
            id = WebSocket_open(url);
            SetUserPointer();
            if (instances == null)
                instances = new Dictionary<IntPtr, Channel>();
    
            instances[(IntPtr)id] = this;
        }
        
        public WebSocket(int ws) {           
            id = ws;
            SetUserPointer();
            if (instances == null)
                instances = new Dictionary<IntPtr, Channel>();
    
            instances[(IntPtr)id] = this;
        }
    
        ~WebSocket() {
            Close();
        }
    
        public void Close() => WebSocket_close(id);
    
        public override bool Send(byte[] data, int size) => WebSocket_send(id, data, size);
        
        public override void OnOpen(Action<int> callback) {
            openCallback = callback;
            WebSocket_onOpen(id, OnOpenCallback);
        }
    
        public override void OnError(Action<int, string> callback) {
            errorCallback = callback;
            WebSocket_onError(id, OnErrorCallback);
        }
    
        public override void OnMessage(Action<int, byte[], int> callback) {
            messageCallback = callback;
            WebSocket_onMessage(id, OnMessageCallback);
        }
        
        protected override void SetUserPointer() => WebSocket_setUserPointer(id, (IntPtr)id);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocket_open(string url);
    
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
}