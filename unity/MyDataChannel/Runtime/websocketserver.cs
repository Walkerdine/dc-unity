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
        
        private delegate void WebSocketClientCallback(int webSocketServerID, int webSocketID, IntPtr userPtr);
        
        private Action<int,int> clientCallback;
        private static Dictionary<IntPtr, WebSocketServer> instances;
        public int id {get; private set;}
        
        public WebSocketServer(ref rtcWsServerConfiguration config, Action<int,int> callback) {
            clientCallback = callback;
            id = WebSocketServer_new(ref config, OnClient);
            SetUserPointer();
            if (instances == null)
                instances = new Dictionary<IntPtr, WebSocketServer>();
            instances[(IntPtr)id] = this;
        }
    
        ~WebSocketServer() {
            Delete();
        }
        
        public int Delete() => WebSocketServer_delete(id);
    
        public int Port() => WebSocketServer_getPort(id);
        
        private void SetUserPointer() => WebSocketServer_setUserPointer(id, (IntPtr)id);
        
        [MonoPInvokeCallback(typeof(WebSocketClientCallback))]
        private static void OnClient(int wsserver, int ws, IntPtr ptr) {
            instances?[ptr].clientCallback(wsserver, ws);
        }
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_new(ref rtcWsServerConfiguration config, WebSocketClientCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_delete(int wsserver);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int WebSocketServer_getPort(int wsserver);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void WebSocketServer_setUserPointer(int wsserver, IntPtr ptr);
    }
}

#endif