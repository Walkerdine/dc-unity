#ifndef W_PEERCONNECTION_HPP
#define W_PEERCONNECTION_HPP

#include "IUnityInterface.h"
#include "w_rtc.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_new(const rtcConfiguration* config);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_delete(int pc);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_localDescription(int pc, char *buffer, int size);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_remoteDescription(int pc, char *buffer, int size);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_createDataChannel(int pc, const char* label, bool unordered, int maxRetransmits, int maxPacketLifeTime);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setRemoteDescription(int pc, const char *sdp, const char *type);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_addRemoteCandidate(int pc, const char *cand, const char *mid);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onDataChannel(int pc, rtcDataChannelCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalDescription(int pc, rtcDescriptionCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalCandidate(int pc, rtcCandidateCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onStateChange(int pc, rtcStateChangeCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onGatheringStateChange(int pc, rtcGatheringStateCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onSignalingStateChange(int pc, rtcSignalingStateCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setUserPointer(int pc, void * ptr);

} // extern "C"
}

#endif //W_PEERCONNECTION_HPP