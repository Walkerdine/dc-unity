using System;
using System.Runtime.InteropServices;

#if !UNITY_WEBGL
public delegate void LocalDescriptionCallback(int pc, string sdp, string type, IntPtr ptr);
public delegate void LocalCandidateCallback(int pc, string cand, string mid, IntPtr ptr);
public delegate void StateChangeCallback(int pc, State state, IntPtr ptr);
public delegate void GatheringStateChangeCallback(int pc, GatheringState gathering_state, IntPtr ptr);
public delegate void SignalingStateChangeCallback(int pc, SignalingState signaling_state, IntPtr ptr);
public delegate void DataChannelCallback(int pc, int dc, IntPtr ptr);
#else
public delegate void LocalDescriptionCallback(IntPtr pSdp, IntPtr pType, IntPtr ptr);
public delegate void LocalCandidateCallback(IntPtr pCandidate, IntPtr pSdpMid, IntPtr ptr);
public delegate void StateChangeCallback(State state, IntPtr ptr);
public delegate void GatheringStateChangeCallback(GatheringState gathering_state, IntPtr ptr);
public delegate void SignalingStateChangeCallback(SignalingState signaling_state, IntPtr ptr);
public delegate void DataChannelCallback(int dc, IntPtr ptr);
#endif

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
    private readonly int _peerConnection;
    public int Id { get; private set; }
    
    public Action<int> datachannel_callback;

    public Action<string,string> description_callback;

    public Action<string,string> candidate_callback;

    public Action<State> state_callback;

    public Action<GatheringState> gathering_state_callback;

    public Action<SignalingState> signaling_state_callback;

    public string sdp { get; set; }
    public string type { get; set; }
    public string cand { get; set; }
    public string mid { get; set; }
    public State state { get; set; }
    public GatheringState gathering_state { get; set; }
    public SignalingState signaling_state { get; set; }

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
        Id = _peerConnection;
        setUserPointer((IntPtr)Id);

        PeerConnectionCallbackBridge.SetInstance(this);

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
        //IntPtr strPtr;
        //int size;
        //PeerConnection_localDescription(_peerConnection,strPtr,size);
        return sdp;
    }

    public string remoteDescription()
    {
        //IntPtr strPtr;
        //int size;
        //PeerConnection_remoteDescription(_peerConnection,strPtr,size);
        return "";
    }

    public DataChannel createDataChannel(string label, bool unordered, int maxRetransmits, int maxPacketLifeTime)
    {
        return new DataChannel(PeerConnection_createDataChannel(_peerConnection, label, unordered, maxRetransmits, maxPacketLifeTime));
    }
    
    public void setUserPointer(IntPtr ptr) => PeerConnection_setUserPointer(_peerConnection,ptr);

    public void setRemoteDescription(string sdp, string type) => PeerConnection_setRemoteDescription(_peerConnection, sdp, type);

    public void addRemoteCandidate(string cand, string mid) => PeerConnection_addRemoteCandidate(_peerConnection, cand, mid);

    public void onDataChannelCallback(DataChannelCallback callback) => PeerConnection_onDataChannel(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));

    public void onLocalDescriptionCallback(LocalDescriptionCallback callback) => PeerConnection_onLocalDescription(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));

    public void onLocalCandidateCallback(LocalCandidateCallback callback) => PeerConnection_onLocalCandidate(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));

    public void onStateChangeCallback(StateChangeCallback callback) => PeerConnection_onStateChange(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));

    public void onGatheringStateChangeCallback(GatheringStateChangeCallback callback) => PeerConnection_onGatheringStateChange(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));

    public void onSignalingStateChangeCallback(SignalingStateChangeCallback callback) => PeerConnection_onSignalingStateChange(_peerConnection, Marshal.GetFunctionPointerForDelegate(callback));
    
    public void onDataChannel(Action<int> callback)
    {
        datachannel_callback = callback;
    }

    public void onLocalDescription(Action<string,string> callback)
    {
        description_callback = callback;
    }

    public void onLocalCandidate(Action<string,string> callback)
    {
        candidate_callback = callback;
    }

    public void onStateChange(Action<State> callback)
    {
        state_callback = callback;
    }

    public void onGatheringStateChange(Action<GatheringState> callback) 
    {
        gathering_state_callback = callback;
    }

    public void onSignalingStateChange(Action<SignalingState> callback) 
    {
        signaling_state_callback = callback;
    }
    
    [DllImport(DLL.DLL_NAME)]
    private static extern int PeerConnection_new(ref rtcConfiguration config);

    [DllImport(DLL.DLL_NAME)]
    private static extern IntPtr PeerConnection_localDescription(int peerconnection, IntPtr Buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern IntPtr PeerConnection_remoteDescription(int peerconnection, IntPtr Buffer, int size);

    [DllImport(DLL.DLL_NAME)]
    private static extern int PeerConnection_createDataChannel(int peerconnection, string label, bool unordered, int maxRetransmits, int maxPacketLifeTime);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_setRemoteDescription(int peerconnection, string sdp, string type);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_addRemoteCandidate(int peerconnection, string cand, string mid);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onDataChannel(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onLocalDescription(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onLocalCandidate(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onStateChange(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onGatheringStateChange(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);

    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_onSignalingStateChange(int peerconnection, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    
    [DllImport(DLL.DLL_NAME)]
    private static extern void PeerConnection_setUserPointer(int peerconnection, IntPtr ptr);

}