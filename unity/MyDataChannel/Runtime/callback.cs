using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using AOT;
using UnityEngine;

public static class DLL
{
#if !UNITY_WEBGL
    public const string DLL_NAME = "DataChannelUnity";
#else
    public const string DLL_NAME = "__Internal";
#endif 
}

public static class CallbackBridge
{
    public static void Cleanup()
    {
        PeerConnectionCallbackBridge.Cleanup();
        ChannelCallbackBridge.Cleanup();
    }
}

// A static class to detour Unity not supporting providing instance methods as callbacks to native code.
// NotSupportedException: IL2CPP does not support marshaling delegates that point to instance methods to native code.
public static class PeerConnectionCallbackBridge
{
    private static Dictionary<IntPtr, PeerConnection> instances;

    public static void SetInstance(PeerConnection instance)
    {
        if (instances == null)
            instances = new Dictionary<IntPtr, PeerConnection>();

        instances[(IntPtr)instance.Id] = instance;
        instances[(IntPtr)instance.Id].onLocalDescriptionCallback(OnLocalDescription);
        instances[(IntPtr)instance.Id].onLocalCandidateCallback(OnLocalCandidate);
        instances[(IntPtr)instance.Id].onStateChangeCallback(OnStateChange);
        instances[(IntPtr)instance.Id].onGatheringStateChangeCallback(OnGatheringStateChange);
        instances[(IntPtr)instance.Id].onSignalingStateChangeCallback(OnSignalingStateChange);
        instances[(IntPtr)instance.Id].onDataChannelCallback(OnDataChannel);
    }

    public static void Cleanup()
    {
        instances = null;
    }

#if !UNITY_WEBGL
    [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
    public static void OnLocalDescription(int pc, string sdp, string type, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.description_callback(sdp, type);
    }

    [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
    public static void OnLocalCandidate(int pc, string cand, string mid, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.candidate_callback(cand, mid);
    }
    
    [MonoPInvokeCallback(typeof(StateChangeCallback))]
    public static void OnStateChange(int pc, State state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
    public static void OnGatheringStateChange(int pc, GatheringState state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.gathering_state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
    public static void OnSignalingStateChange(int pc, SignalingState state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.signaling_state_callback(state);
    }

    [MonoPInvokeCallback(typeof(DataChannelCallback))]
    public static void OnDataChannel(int pc, int dc, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.datachannel_callback(dc);
    }


#else
    [MonoPInvokeCallback(typeof(LocalDescriptionCallback))]
    public static void OnLocalDescription(IntPtr pSdp, IntPtr pType, IntPtr ptr)
    {
        string sdp = Marshal.PtrToStringAnsi(pSdp);
        string type = Marshal.PtrToStringAnsi(pType);
        PeerConnection instance = instances?[ptr];
        instance.description_callback(sdp, type);
    }

    [MonoPInvokeCallback(typeof(LocalCandidateCallback))]
    public static void OnLocalCandidate(IntPtr pCandidate, IntPtr pSdpMid, IntPtr ptr)
    {
        string cand = Marshal.PtrToStringAnsi(pCandidate);
        string mid = Marshal.PtrToStringAnsi(pSdpMid);
        PeerConnection instance = instances?[ptr];
        instance.candidate_callback(cand, mid);
    }


    [MonoPInvokeCallback(typeof(StateChangeCallback))]
    public static void OnStateChange(State state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(GatheringStateChangeCallback))]
    public static void OnGatheringStateChange(GatheringState state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.gathering_state_callback(state);
    }
    
    [MonoPInvokeCallback(typeof(SignalingStateChangeCallback))]
    public static void OnSignalingStateChange(SignalingState state, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.signaling_state_callback(state);
    }

    [MonoPInvokeCallback(typeof(DataChannelCallback))]
    public static void OnDataChannel(int dc, IntPtr ptr)
    {
        PeerConnection instance = instances?[ptr];
        instance.datachannel_callback(dc);
    }
#endif
}

// A static class to detour Unity not supporting providing instance methods as callbacks to native code.
// NotSupportedException: IL2CPP does not support marshaling delegates that point to instance methods to native code.
public static class ChannelCallbackBridge
{
    private static Dictionary<IntPtr, Channel> instances;

    public static void SetInstance(Channel instance)
    {
        if (instances == null)
            instances = new Dictionary<IntPtr, Channel>();

        instances[(IntPtr)instance.Id] = instance;
        instances[(IntPtr)instance.Id].onOpenCallback(OnOpen);
        instances[(IntPtr)instance.Id].onErrorCallback(OnError);
        instances[(IntPtr)instance.Id].onMessageCallback(OnMessage);
        //instances[instance.Id].onBufferedAmountLowCallback(OnBufferedAmountLow);
    }

    public static void Cleanup()
    {
        instances = null;
    }
#if !UNITY_WEBGL
    [MonoPInvokeCallback(typeof(OpenCallback))]
    public static void OnOpen(int id, IntPtr ptr)
    {
        Channel instance = instances?[ptr];
        instance.open_callback();
    }

    [MonoPInvokeCallback(typeof(ErrorCallback))]
    public static void OnError(int id, string error, IntPtr ptr)
    {
        Channel instance = instances?[ptr];
        instance.error_callback(error);
    }

    [MonoPInvokeCallback(typeof(MessageCallback))]
    public static void OnMessage(int id, string message, int size, IntPtr ptr)
    {
        Channel instance = instances?[ptr];
        instance.message_callback(message,size);
    }
#else
    [MonoPInvokeCallback(typeof(OpenCallback))]
    public static void OnOpen(IntPtr ptr)
    {
        Channel instance = instances?[ptr];
        instance.open_callback();
    }

    [MonoPInvokeCallback(typeof(ErrorCallback))]
    public static void OnError(IntPtr pError, IntPtr ptr)
    {
        string error = Marshal.PtrToStringAnsi(pError);
        Channel instance = instances?[ptr];
        instance.error_callback(error);
    }

    [MonoPInvokeCallback(typeof(MessageCallback))]
    public static void OnMessage(IntPtr pMessage, int size, IntPtr ptr)
    {
        string message = Marshal.PtrToStringAnsi(pMessage);
        Channel instance = instances?[ptr];
        instance.message_callback(message,size);
    }
#endif
    //[MonoPInvokeCallback(typeof(BufferedAmountLowCallback))]
    //public static void OnBufferedAmountLow(int id, IntPtr ptr)
    //{
    //    Channel instance = instances?[id];
    //    instance.buffer_low_callback(id);
    //}
}

