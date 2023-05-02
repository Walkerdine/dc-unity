using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
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
        
        private delegate void LocalDescriptionCallback(int pc, string sdp, string type, int userId);
        private delegate void LocalCandidateCallback(int pc, string cand, string mid, int userId);
        private delegate void StateChangeCallback(int pc, State state, int userId);
        private delegate void GatheringStateChangeCallback(int pc, GatheringState state, int userId);
        private delegate void SignalingStateChangeCallback(int pc, SignalingState state, int userId);
        private delegate void DataChannelCallback(int pc, int dc, int userId);

        private static Dictionary<int, PeerConnection> instances = new Dictionary<int, PeerConnection>();
        private int _id;
        
        private Action<int,int> datachannelCallback;
        private Action<int,string,string> descriptionCallback;
        private Action<int,string,string> candidateCallback;
        private Action<int,State> stateCallback;
        private Action<int,GatheringState> gatheringStateCallback;
        private Action<int,SignalingState> signalingStateCallback;
        
        public int Id {get; private set;} = 0;
    
        public PeerConnection(string[] iceServers) { 
            _id = PeerConnection_new(iceServers, iceServers.Length);
            instances[_id] = this;
            SetUserId(_id);
        }
        
        ~PeerConnection() {
            PeerConnection_delete(_id);
        }
    
        public string LocalDescription() {
            IntPtr strPtr = Marshal.AllocHGlobal(4096);
            int size = 4096;
            PeerConnection_localDescription(_id, strPtr, size);
            string sdp = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return sdp;
        }
    
        public string RemoteDescription() {
            IntPtr strPtr = Marshal.AllocHGlobal(4096);
            int size = 4096;
            PeerConnection_remoteDescription(_id, strPtr, size);
            string sdp = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return sdp;
        }
        
        public string LocalDescriptionType() {
            IntPtr strPtr = Marshal.AllocHGlobal(128);
            int size = 128;
            PeerConnection_localDescriptionType(_id, strPtr, size);
            string type = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return type;
        }
    
        public string RemoteDescriptionType() {
            IntPtr strPtr = Marshal.AllocHGlobal(128);
            int size = 128;
            PeerConnection_remoteDescriptionType(_id, strPtr, size);
            string type = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return type;
        }
    
        public DataChannel CreateDataChannel(string label, bool unordered, int maxRetransmits, int maxPacketLifeTime) {
            return new DataChannel(PeerConnection_createDataChannel(_id, label, unordered, maxRetransmits, maxPacketLifeTime));
        }
    
        public void SetRemoteDescription(string sdp, string type) => PeerConnection_setRemoteDescription(_id, sdp, type);
    
        public void AddRemoteCandidate(string cand, string mid) => PeerConnection_addRemoteCandidate(_id, cand, mid);
    
        public void OnDataChannel(Action<int,int> callback) {
            datachannelCallback = callback;
            PeerConnection_onDataChannel(_id, OnDataChannelCallback);
        }
    
        public void OnLocalDescription(Action<int,string,string> callback) {
            descriptionCallback = callback;
            PeerConnection_onLocalDescription(_id, OnLocalDescriptionCallback);
        }
    
        public void OnLocalCandidate(Action<int,string,string> callback) {
            candidateCallback = callback;
            PeerConnection_onLocalCandidate(_id, OnLocalCandidateCallback);
        }
    
        public void OnStateChange(Action<int,State> callback) {
            stateCallback = callback;
            PeerConnection_onStateChange(_id, OnStateChangeCallback);
        }
    
        public void OnGatheringStateChange(Action<int,GatheringState> callback) {
            gatheringStateCallback = callback;
            PeerConnection_onGatheringStateChange(_id, OnGatheringStateChangeCallback);
        }
    
        public void OnSignalingStateChange(Action<int,SignalingState> callback) {
            signalingStateCallback= callback;
            PeerConnection_onSignalingStateChange(_id, OnSignalingStateChangeCallback);
        }
        
        public void SetUserId(int userId){
            Id = userId;
            PeerConnection_setUserPointer(_id, Id);
        }
          
        [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
        private static void OnLocalDescriptionCallback(int pc, string sdp, string type, int userId) {
            instances?[pc].descriptionCallback(userId, sdp, type);
        }
    
        [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
        private static void OnLocalCandidateCallback(int pc, string cand, string mid, int userId) {
            instances?[pc].candidateCallback(userId, cand, mid);
        }
        
        [MonoPInvokeCallback(typeof(StateChangeCallback))]
        private static void OnStateChangeCallback(int pc, State state, int userId) {
            instances?[pc].stateCallback(userId, state);
        }
        
        [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
        private static void OnGatheringStateChangeCallback(int pc, GatheringState state, int userId) {
            instances?[pc].gatheringStateCallback(userId, state);
        }
        
        [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
        private static void OnSignalingStateChangeCallback(int pc, SignalingState state, int userId) {
            instances?[pc].signalingStateCallback(userId, state);
        }
    
        [MonoPInvokeCallback(typeof(DataChannelCallback))]
        private static void OnDataChannelCallback(int pc, int dc, int userId) {
            instances?[pc].datachannelCallback(userId, dc);
        }

        [DllImport(DLL.DLL_NAME)]
        private static extern int PeerConnection_new(string[] ice_servers, int ice_servers_count);
        
        [DllImport(DLL.DLL_NAME)]
        private static extern int PeerConnection_delete(int id);
    
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
        private static extern void PeerConnection_setUserPointer(int id, int userId);
    }
}