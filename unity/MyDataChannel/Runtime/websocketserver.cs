using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

#if !UNITY_WEBGL

namespace WebRTC { 
  
    [StructLayout(LayoutKind.Sequential)]
    public struct rtcWsServerConfiguration {
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
    
    public class WebSocketServer {
        
        private delegate void WebSocketClientCallback(int webSocketServerId, int webSocketId, int userId);
        
        private Action<int,int> clientCallback;
        private static Dictionary<int, WebSocketServer> instances = new Dictionary<int, WebSocketServer>();
        private int _id;
        
        public int Id {get; private set;}
        
        public WebSocketServer(ref rtcWsServerConfiguration config, Action<int,int> callback) {
            clientCallback = callback;
            _id = WebSocketServer_new(ref config, OnClient);
            instances[_id] = this;
            SetUserId(_id);
        }
    
        ~WebSocketServer() {
            WebSocketServer_delete(_id);
        }
    
        public int Port() => WebSocketServer_getPort(_id);
        
        public void SetUserId(int userId) {
            Id = userId;
            WebSocketServer_setUserPointer(_id, Id);
        }
        
        [MonoPInvokeCallback(typeof(WebSocketClientCallback))]
        private static void OnClient(int webSocketServerId, int webSocketId, int userId) {
            instances?[webSocketServerId].clientCallback(userId, webSocketId);
        }
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_new(ref rtcWsServerConfiguration config, WebSocketClientCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_delete(int id);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_getPort(int id);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocketServer_setUserPointer(int id, int userId);
    }
}

#endif