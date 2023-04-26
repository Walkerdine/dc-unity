using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {

    [StructLayout(LayoutKind.Sequential)]
    public struct rtcConfiguration {
        public IntPtr iceServers;
        public int iceServersCount;
        [MarshalAs(UnmanagedType.LPStr)]
        public string proxyServer;
        [MarshalAs(UnmanagedType.LPStr)]
        public string bindAddress;
        public int certificateType;
        public int iceTransportPolicy;
        [MarshalAs(UnmanagedType.Bool)]
        public bool enableIceTcp;
        [MarshalAs(UnmanagedType.Bool)]
        public bool enableIceUdpMux;
        [MarshalAs(UnmanagedType.Bool)]
        public bool disableAutoNegotiation;
        [MarshalAs(UnmanagedType.Bool)]
        public bool forceMediaTransport;
        public ushort portRangeBegin;
        public ushort portRangeEnd;
        public int mtu;
        public int maxMessageSize;
    }
    
    public enum State {
        New = 0,
        Connecting = 1,
        Connected = 2,
        Disconnected = 3,
        Failed = 4,
        Closed = 5
    }
    
    public enum GatheringState {
        New = 0,
        InProgress = 1,
        Complete = 2
    }
    
    public enum SignalingState {
        Stable = 0,
        HaveLocalOffer = 1,
        HaveRemoteOffer = 2,
        HaveLocalPranswer = 3,
        HaveRemotePranswer = 4
    }
    
    public class PeerConnection {
        
    #if !UNITY_WEBGL
        private delegate void LocalDescriptionCallback(int pc, string sdp, string type, IntPtr ptr);
        private delegate void LocalCandidateCallback(int pc, string cand, string mid, IntPtr ptr);
        private delegate void StateChangeCallback(int pc, State state, IntPtr ptr);
        private delegate void GatheringStateChangeCallback(int pc, GatheringState gathering_state, IntPtr ptr);
        private delegate void SignalingStateChangeCallback(int pc, SignalingState signaling_state, IntPtr ptr);
        private delegate void DataChannelCallback(int pc, int dc, IntPtr ptr);
    #else
        private delegate void LocalDescriptionCallback(string sdp, string type, IntPtr ptr);
        private delegate void LocalCandidateCallback(string cand, string mid, IntPtr ptr);
        private delegate void StateChangeCallback(State state, IntPtr ptr);
        private delegate void GatheringStateChangeCallback(GatheringState gathering_state, IntPtr ptr);
        private delegate void SignalingStateChangeCallback(SignalingState signaling_state, IntPtr ptr);
        private delegate void DataChannelCallback(int dc, IntPtr ptr);
    #endif
        
        private static Dictionary<IntPtr, PeerConnection> instances;
        public int id {get; private set;}
        
        private Action<int,int> datachannelCallback;
        private Action<int,string,string> descriptionCallback;
        private Action<int,string,string> candidateCallback;
        private Action<int,State> stateCallback;
        private Action<int,GatheringState> gatheringStateCallback;
        private Action<int,SignalingState> signalingStateCallback;
    
        public PeerConnection(string[] iceServers) { 
            rtcConfiguration config = new rtcConfiguration();
            int ice_servers_count = iceServers.Length;
            
            // Allocate memory for the array of pointers
            IntPtr ice_ptr = Marshal.AllocHGlobal(ice_servers_count * IntPtr.Size);
    
            // Write the pointers to the individual strings into the array
            for (int i = 0; i < ice_servers_count; i++) {
                IntPtr string_ptr = Marshal.StringToHGlobalAnsi(iceServers[i]);
                Marshal.WriteIntPtr(ice_ptr, i * IntPtr.Size, string_ptr);
            }
    
            config.iceServers = ice_ptr;
            config.iceServersCount = ice_servers_count;
    
            id = PeerConnection_new(ref config);
            SetUserPointer();
    
            if (instances == null)
                instances = new Dictionary<IntPtr, PeerConnection>();
            
            instances[(IntPtr)id] = this;
    
            // Free the memory for the individual strings
            for (int i = 0; i < ice_servers_count; i++) {
                IntPtr string_ptr = Marshal.ReadIntPtr(ice_ptr, i * IntPtr.Size);
                Marshal.FreeHGlobal(string_ptr);
            }
    
            // Free the memory for the array of pointers
            Marshal.FreeHGlobal(ice_ptr);
        }
    
        public string LocalDescription() {
            IntPtr strPtr = Marshal.AllocHGlobal(4096);
            int size = 4096;
            PeerConnection_localDescription(id, strPtr, size);
            string sdp = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return sdp;
        }
    
        public string RemoteDescription() {
            IntPtr strPtr = Marshal.AllocHGlobal(4096);
            int size = 4096;
            PeerConnection_remoteDescription(id, strPtr, size);
            string sdp = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return sdp;
        }
        
        public string LocalDescriptionType() {
            IntPtr strPtr = Marshal.AllocHGlobal(128);
            int size = 128;
            PeerConnection_localDescriptionType(id, strPtr, size);
            string type = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return type;
        }
    
        public string RemoteDescriptionType() {
            IntPtr strPtr = Marshal.AllocHGlobal(128);
            int size = 128;
            PeerConnection_remoteDescriptionType(id, strPtr, size);
            string type = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return type;
        }
    
        public DataChannel CreateDataChannel(string label, bool unordered, int maxRetransmits, int maxPacketLifeTime) {
            return new DataChannel(PeerConnection_createDataChannel(id, label, unordered, maxRetransmits, maxPacketLifeTime));
        }
    
        public void SetRemoteDescription(string sdp, string type) => PeerConnection_setRemoteDescription(id, sdp, type);
    
        public void AddRemoteCandidate(string cand, string mid) => PeerConnection_addRemoteCandidate(id, cand, mid);
    
        public void OnDataChannel(Action<int,int> callback) {
            datachannelCallback = callback;
            PeerConnection_onDataChannel(id, OnDataChannelCallback);
        }
    
        public void OnLocalDescription(Action<int,string,string> callback) {
            descriptionCallback = callback;
            PeerConnection_onLocalDescription(id, OnLocalDescriptionCallback);
        }
    
        public void OnLocalCandidate(Action<int,string,string> callback) {
            candidateCallback = callback;
            PeerConnection_onLocalCandidate(id, OnLocalCandidateCallback);
        }
    
        public void OnStateChange(Action<int,State> callback) {
            stateCallback = callback;
            PeerConnection_onStateChange(id, OnStateChangeCallback);
        }
    
        public void OnGatheringStateChange(Action<int,GatheringState> callback) {
            gatheringStateCallback = callback;
            PeerConnection_onGatheringStateChange(id, OnGatheringStateChangeCallback);
        }
    
        public void OnSignalingStateChange(Action<int,SignalingState> callback) {
            signalingStateCallback= callback;
            PeerConnection_onSignalingStateChange(id, OnSignalingStateChangeCallback);
        }
        
        private void SetUserPointer() => PeerConnection_setUserPointer(id, (IntPtr)id);
          
    #if !UNITY_WEBGL
        [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
        private static void OnLocalDescriptionCallback(int pc, string sdp, string type, IntPtr ptr) {
            instances?[ptr].descriptionCallback((int)ptr, sdp, type);
        }
    
        [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
        private static void OnLocalCandidateCallback(int pc, string cand, string mid, IntPtr ptr) {
            instances?[ptr].candidateCallback((int)ptr, cand, mid);
        }
        
        [MonoPInvokeCallback(typeof(StateChangeCallback))]
        private static void OnStateChangeCallback(int pc, State state, IntPtr ptr) {
            instances?[ptr].stateCallback((int)ptr, state);
        }
        
        [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
        private static void OnGatheringStateChangeCallback(int pc, GatheringState state, IntPtr ptr) {
            instances?[ptr].gatheringStateCallback((int)ptr, state);
        }
        
        [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
        private static void OnSignalingStateChangeCallback(int pc, SignalingState state, IntPtr ptr) {
            instances?[ptr].signalingStateCallback((int)ptr, state);
        }
    
        [MonoPInvokeCallback(typeof(DataChannelCallback))]
        private static void OnDataChannelCallback(int pc, int dc, IntPtr ptr) {
            instances?[ptr].datachannelCallback((int)ptr, dc);
        }
    #else    
        [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
        private static void OnLocalDescriptionCallback(string sdp, string type, IntPtr ptr) {
            instances?[ptr].descriptionCallback((int)ptr, sdp, type);
        }
    
        [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
        private static void OnLocalCandidateCallback(string cand, string mid, IntPtr ptr) {
            instances?[ptr].candidateCallback((int)ptr, cand, mid);
        }
    
        [MonoPInvokeCallback(typeof(StateChangeCallback))]
        private static void OnStateChangeCallback(State state, IntPtr ptr) {
            instances?[ptr].stateCallback((int)ptr, state);
        }
        
        [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
        private static void OnGatheringStateChangeCallback(GatheringState state, IntPtr ptr) {
            instances?[ptr].gatheringStateCallback((int)ptr, state);
        }
        
        [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
        private static void OnSignalingStateChangeCallback(SignalingState state, IntPtr ptr) {
            instances?[ptr].signalingStateCallback((int)ptr, state);
        }
    
        [MonoPInvokeCallback(typeof(DataChannelCallback))]
        private static void OnDataChannelCallback(int dc, IntPtr ptr) {
            instances?[ptr].datachannelCallback((int)ptr, dc);
        }
    #endif

        [DllImport(DLL.DLL_NAME)]
        private static extern int PeerConnection_new(ref rtcConfiguration config);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_localDescription(int id, IntPtr Buffer, int size);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_remoteDescription(int id, IntPtr Buffer, int size);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_localDescriptionType(int id, IntPtr Buffer, int size);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_remoteDescriptionType(int id, IntPtr Buffer, int size);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern int PeerConnection_createDataChannel(int id, string label, bool unordered, int maxRetransmits, int maxPacketLifeTime);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_setRemoteDescription(int id, string sdp, string type);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_addRemoteCandidate(int id, string cand, string mid);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onDataChannel(int id, DataChannelCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onLocalDescription(int id, LocalDescriptionCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onLocalCandidate(int id, LocalCandidateCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onStateChange(int id, StateChangeCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onGatheringStateChange(int id, GatheringStateChangeCallback callback);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_onSignalingStateChange(int id, SignalingStateChangeCallback callback);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern void PeerConnection_setUserPointer(int id, IntPtr ptr);
    }
}