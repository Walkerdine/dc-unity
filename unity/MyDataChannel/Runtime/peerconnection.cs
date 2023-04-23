using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

[StructLayout(LayoutKind.Sequential)]
public struct rtcConfiguration
{
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

public enum State
{
    New = 0,
    Connecting = 1,
    Connected = 2,
    Disconnected = 3,
    Failed = 4,
    Closed = 5
}

public enum GatheringState
{
    New = 0,
    InProgress = 1,
    Complete = 2
}

public enum SignalingState
{
    Stable = 0,
    HaveLocalOffer = 1,
    HaveRemoteOffer = 2,
    HaveLocalPranswer = 3,
    HaveRemotePranswer = 4
}

public class PeerConnection
{
#if !UNITY_WEBGL
    public delegate void LocalDescriptionCallback(int pc, string sdp, string type, IntPtr ptr);
    public delegate void LocalCandidateCallback(int pc, string cand, string mid, IntPtr ptr);
    public delegate void StateChangeCallback(int pc, State state, IntPtr ptr);
    public delegate void GatheringStateChangeCallback(int pc, GatheringState gathering_state, IntPtr ptr);
    public delegate void SignalingStateChangeCallback(int pc, SignalingState signaling_state, IntPtr ptr);
    public delegate void DataChannelCallback(int pc, int dc, IntPtr ptr);
#else
    public delegate void LocalDescriptionCallback(string sdp, string type, IntPtr ptr);
    public delegate void LocalCandidateCallback(string cand, string mid, IntPtr ptr);
    public delegate void StateChangeCallback(State state, IntPtr ptr);
    public delegate void GatheringStateChangeCallback(GatheringState gathering_state, IntPtr ptr);
    public delegate void SignalingStateChangeCallback(SignalingState signaling_state, IntPtr ptr);
    public delegate void DataChannelCallback(int dc, IntPtr ptr);
#endif
    
    private static Dictionary<IntPtr, PeerConnection> instances;
    private readonly int _peerConnection;
    public IntPtr Id { get; private set; }
    
    public Action<int> datachannel_callback;
    public Action<string,string> description_callback;
    public Action<string,string> candidate_callback;
    public Action<State> state_callback;
    public Action<GatheringState> gathering_state_callback;
    public Action<SignalingState> signaling_state_callback;

    public PeerConnection(string[] iceServers) 
    { 
        rtcConfiguration config = new rtcConfiguration();
        int ice_servers_count = iceServers.Length;
        
        // Allocate memory for the array of pointers
        IntPtr ice_ptr = Marshal.AllocHGlobal(ice_servers_count * IntPtr.Size);

        // Write the pointers to the individual strings into the array
        for (int i = 0; i < ice_servers_count; i++)
        {
            IntPtr string_ptr = Marshal.StringToHGlobalAnsi(iceServers[i]);
            Marshal.WriteIntPtr(ice_ptr, i * IntPtr.Size, string_ptr);
        }

        config.iceServers = ice_ptr;
        config.iceServersCount = ice_servers_count;

        _peerConnection = PeerConnection_new(ref config);
        Id = (IntPtr)_peerConnection;
        setUserPointer(Id);

        if (instances == null)
            instances = new Dictionary<IntPtr, PeerConnection>();
        
        instances[Id] = this;

        // Free the memory for the individual strings
        for (int i = 0; i < ice_servers_count; i++)
        {
            IntPtr string_ptr = Marshal.ReadIntPtr(ice_ptr, i * IntPtr.Size);
            Marshal.FreeHGlobal(string_ptr);
        }

        // Free the memory for the array of pointers
        Marshal.FreeHGlobal(ice_ptr);
    }

    public string localDescription()
    {
        IntPtr strPtr = Marshal.AllocHGlobal(4096);
        int size=4096;
        PeerConnection_localDescription(_peerConnection,strPtr,size);
        string sdp = Marshal.PtrToStringAnsi(strPtr);
        Marshal.FreeHGlobal(strPtr);
        return sdp;
    }

    public string remoteDescription()
    {
        IntPtr strPtr = Marshal.AllocHGlobal(4096);
        int size=4096;
        PeerConnection_remoteDescription(_peerConnection,strPtr,size);
        string sdp = Marshal.PtrToStringAnsi(strPtr);
        Marshal.FreeHGlobal(strPtr);
        return sdp;
    }
    
    public string localDescriptionType()
    {
        IntPtr strPtr = Marshal.AllocHGlobal(128);
        int size=128;
        PeerConnection_localDescriptionType(_peerConnection,strPtr,size);
        string type = Marshal.PtrToStringAnsi(strPtr);
        Marshal.FreeHGlobal(strPtr);
        return type;
    }

    public string remoteDescriptionType()
    {
        IntPtr strPtr = Marshal.AllocHGlobal(128);
        int size=128;
        PeerConnection_remoteDescriptionType(_peerConnection,strPtr,size);
        string type = Marshal.PtrToStringAnsi(strPtr);
        Marshal.FreeHGlobal(strPtr);
        return type;
    }

    public DataChannel createDataChannel(string label, bool unordered, int maxRetransmits, int maxPacketLifeTime)
    {
        return new DataChannel(PeerConnection_createDataChannel(_peerConnection, label, unordered, maxRetransmits, maxPacketLifeTime));
    }
    
    public void setUserPointer(IntPtr ptr) => PeerConnection_setUserPointer(_peerConnection,ptr);

    public void setRemoteDescription(string sdp, string type) => PeerConnection_setRemoteDescription(_peerConnection, sdp, type);

    public void addRemoteCandidate(string cand, string mid) => PeerConnection_addRemoteCandidate(_peerConnection, cand, mid);

    public void onDataChannel(Action<int> callback)
    {
        instances[Id].datachannel_callback = callback;
        PeerConnection_onDataChannel(_peerConnection, OnDataChannel);
    }

    public void onLocalDescription(Action<string,string> callback)
    {
        instances[Id].description_callback = callback;
        PeerConnection_onLocalDescription(_peerConnection, OnLocalDescription);
    }

    public void onLocalCandidate(Action<string,string> callback)
    {
        instances[Id].candidate_callback = callback;
        PeerConnection_onLocalCandidate(_peerConnection, OnLocalCandidate);
    }

    public void onStateChange(Action<State> callback)
    {
        instances[Id].state_callback = callback;
        PeerConnection_onStateChange(_peerConnection, OnStateChange);
    }

    public void onGatheringStateChange(Action<GatheringState> callback) 
    {
        instances[Id].gathering_state_callback = callback;
        PeerConnection_onGatheringStateChange(_peerConnection, OnGatheringStateChange);
    }

    public void onSignalingStateChange(Action<SignalingState> callback) 
    {
        instances[Id].signaling_state_callback = callback;
        PeerConnection_onSignalingStateChange(_peerConnection, OnSignalingStateChange);
    }
    
    [DllImport(DLL.DLL_NAME)]
    private static extern int PeerConnection_new(ref rtcConfiguration config);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_localDescription(int peerconnection, IntPtr Buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_remoteDescription(int peerconnection, IntPtr Buffer, int size);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_localDescriptionType(int peerconnection, IntPtr Buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_remoteDescriptionType(int peerconnection, IntPtr Buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern int PeerConnection_createDataChannel(int peerconnection, string label, bool unordered, int maxRetransmits, int maxPacketLifeTime);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_setRemoteDescription(int peerconnection, string sdp, string type);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_addRemoteCandidate(int peerconnection, string cand, string mid);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onDataChannel(int peerconnection, DataChannelCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onLocalDescription(int peerconnection, LocalDescriptionCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onLocalCandidate(int peerconnection, LocalCandidateCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onStateChange(int peerconnection, StateChangeCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onGatheringStateChange(int peerconnection, GatheringStateChangeCallback callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onSignalingStateChange(int peerconnection, SignalingStateChangeCallback callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_setUserPointer(int peerconnection, IntPtr ptr);
    
#if !UNITY_WEBGL
    [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
    public static void OnLocalDescription(int pc, string sdp, string type, IntPtr ptr)
    {
        instances?[ptr].description_callback(sdp, type);
    }

    [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
    public static void OnLocalCandidate(int pc, string cand, string mid, IntPtr ptr)
    {
        instances?[ptr].candidate_callback(cand, mid);
    }
    
    [MonoPInvokeCallback(typeof(StateChangeCallback))]
    public static void OnStateChange(int pc, State state, IntPtr ptr)
    {
        instances?[ptr].state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
    public static void OnGatheringStateChange(int pc, GatheringState state, IntPtr ptr)
    {
        instances?[ptr].gathering_state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
    public static void OnSignalingStateChange(int pc, SignalingState state, IntPtr ptr)
    {
        instances?[ptr].signaling_state_callback(state);
    }

    [MonoPInvokeCallback(typeof(DataChannelCallback))]
    public static void OnDataChannel(int pc, int dc, IntPtr ptr)
    {
        instances?[ptr].datachannel_callback(dc);
    }

#else
    
    [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
    public static void OnLocalDescription(string sdp, string type, IntPtr ptr)
    {
        instances?[ptr].description_callback(sdp, type);
    }

    [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
    public static void OnLocalCandidate(string cand, string mid, IntPtr ptr)
    {
        instances?[ptr].candidate_callback(cand, mid);
    }

    [MonoPInvokeCallback(typeof(StateChangeCallback))]
    public static void OnStateChange(State state, IntPtr ptr)
    {
        instances?[ptr].state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
    public static void OnGatheringStateChange(GatheringState state, IntPtr ptr)
    {
        instances?[ptr].gathering_state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
    public static void OnSignalingStateChange(SignalingState state, IntPtr ptr)
    {
        instances?[ptr].signaling_state_callback(state);
    }

    [MonoPInvokeCallback(typeof(DataChannelCallback))]
    public static void OnDataChannel(int dc, IntPtr ptr)
    {
        instances?[ptr].datachannel_callback(dc);
    }
#endif
}