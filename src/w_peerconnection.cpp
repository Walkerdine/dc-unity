#include "w_peerconnection.hpp"
#include <cstring>

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_new(const rtcConfiguration* config)
{
#if __EMSCRIPTEN__
    return rtcCreatePeerConnection(config->iceServers,NULL,NULL,config->iceServersCount);
#else
    return rtcCreatePeerConnection(config);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_delete(int pc)
{
    rtcDeletePeerConnection(pc);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_localDescription(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetLocalDescription(pc);
    strcpy(buffer,str);
    (void)size;
#else
    rtcGetLocalDescription(pc, buffer, size);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_remoteDescription(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetRemoteDescription(pc);
    strcpy(buffer,str);
    (void)size;
#else
    rtcGetRemoteDescription(pc, buffer, size);
#endif
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_createDataChannel(int pc, const char* label, bool unordered, int maxRetransmits, int maxPacketLifeTime)
{
#if __EMSCRIPTEN__
    return rtcCreateDataChannel(pc, label, unordered, maxRetransmits, maxPacketLifeTime);
#else
    rtcDataChannelInit init;
    init.protocol = NULL;
    init.negotiated = false;
    init.manualStream = false;
    init.stream = 0;
    init.reliability.unordered = unordered;
    init.reliability.maxPacketLifeTime = maxPacketLifeTime;
    init.reliability.maxRetransmits = maxRetransmits;
    return rtcCreateDataChannelEx(pc, label, &init);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setRemoteDescription(int pc, const char *sdp, const char *type)
{
    rtcSetRemoteDescription(pc,sdp,type);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_addRemoteCandidate(int pc, const char *cand, const char *mid)
{
    rtcAddRemoteCandidate(pc, cand, mid);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onDataChannel(int pc, rtcDataChannelCallbackFunc callback) 
{
    rtcSetDataChannelCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalDescription(int pc, rtcDescriptionCallbackFunc callback) 
{
    rtcSetLocalDescriptionCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalCandidate(int pc, rtcCandidateCallbackFunc callback)
{
    rtcSetLocalCandidateCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onStateChange(int pc, rtcStateChangeCallbackFunc callback)
{
    rtcSetStateChangeCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onGatheringStateChange(int pc, rtcGatheringStateCallbackFunc callback)
{
    rtcSetGatheringStateChangeCallback(pc, callback);   
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onSignalingStateChange(int pc, rtcSignalingStateCallbackFunc callback)
{
    rtcSetSignalingStateChangeCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setUserPointer(int id, void * ptr)
{
    rtcSetUserPointer(id, ptr);
}

} // extern "C"
}