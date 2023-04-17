#ifndef W_RTC_HPP
#define W_RTC_HPP

#include "IUnityInterface.h"

#if __EMSCRIPTEN__

#include <stdbool.h>
#include <stdint.h>

#define RTC_API
extern "C" {
typedef void(RTC_API *rtcDescriptionCallbackFunc)(const char *sdp, const char *type, void *ptr);
typedef void(RTC_API *rtcCandidateCallbackFunc)(const char *cand, const char *mid, void *ptr);
typedef void(RTC_API *rtcStateChangeCallbackFunc)(int state, void *ptr);
typedef void(RTC_API *rtcGatheringStateCallbackFunc)(int state, void *ptr);
typedef void(RTC_API *rtcSignalingStateCallbackFunc)(int state, void *ptr);
typedef void(RTC_API *rtcDataChannelCallbackFunc)(int dc, void *ptr);

typedef void(RTC_API *rtcOpenCallbackFunc)(void *ptr);
typedef void(RTC_API *rtcClosedCallbackFunc)(void *ptr);
typedef void(RTC_API *rtcErrorCallbackFunc)(const char *error, void *ptr);
typedef void(RTC_API *rtcMessageCallbackFunc)(const char *message, int size, void *ptr);
typedef void(RTC_API *rtcBufferedAmountLowCallbackFunc)(void *ptr);

typedef enum {
    RTC_CERTIFICATE_DEFAULT = 0, // ECDSA
    RTC_CERTIFICATE_ECDSA = 1,
    RTC_CERTIFICATE_RSA = 2,
} rtcCertificateType;

typedef enum { RTC_TRANSPORT_POLICY_ALL = 0, RTC_TRANSPORT_POLICY_RELAY = 1 } rtcTransportPolicy;

// PeerConnection

typedef struct {
    const char **iceServers;
    int iceServersCount;
    const char *proxyServer; // libnice only
    const char *bindAddress; // libjuice only, NULL means any
    rtcCertificateType certificateType;
    rtcTransportPolicy iceTransportPolicy;
    bool enableIceTcp;    // libnice only
    bool enableIceUdpMux; // libjuice only
    bool disableAutoNegotiation;
    bool forceMediaTransport;
    uint16_t portRangeBegin; // 0 means automatic
    uint16_t portRangeEnd;   // 0 means automatic
    int mtu;                 // <= 0 means automatic
    int maxMessageSize;      // <= 0 means default
} rtcConfiguration;

extern int rtcCreatePeerConnection(const char **pUrls, const char **pUsernames,
              const char **pPasswords, int nIceServers);
extern void rtcDeletePeerConnection(int pc);
extern char *rtcGetLocalDescription(int pc);
extern char *rtcGetLocalDescriptionType(int pc);
extern char *rtcGetRemoteDescription(int pc);
extern char *rtcGetRemoteDescriptionType(int pc);
extern int rtcCreateDataChannel(int pc, const char *label, bool unordered, int maxRetransmits,
           int maxPacketLifeTime);
extern void rtcSetDataChannelCallback(int pc, void (*dataChannelCallback)(int, void *));
extern void rtcSetLocalDescriptionCallback(int pc,
                      void (*descriptionCallback)(const char *, const char *,
                                                  void *));
extern void rtcSetLocalCandidateCallback(int pc, void (*candidateCallback)(const char *,
                                                      const char *, void *));
extern void rtcSetStateChangeCallback(int pc, void (*stateChangeCallback)(int, void *));
extern void rtcSetGatheringStateChangeCallback(int pc,
                          void (*gatheringStateChangeCallback)(int, void *));
extern void rtcSetSignalingStateChangeCallback(int pc,
                          void (*signalingStateChangeCallback)(int, void *));
extern void rtcSetRemoteDescription(int pc, const char *sdp, const char *type);
extern void rtcAddRemoteCandidate(int pc, const char *candidate, const char *mid);
extern void rtcSetUserPointer(int i, void *ptr);
extern void rtcDeleteDataChannel(int dc);
extern int rtcGetDataChannelLabel(int dc, char *buffer, int size);

extern void rtcSetOpenCallback(int dc, void (*openCallback)(void *));
extern void rtcSetErrorCallback(int dc, void (*errorCallback)(const char *, void *));
extern void rtcSetMessageCallback(int dc, void (*messageCallback)(const char *, int, void *));
extern void rtcSetBufferedAmountLowCallback(int dc, void (*bufferedAmountLowCallback)(void *));
extern int rtcGetBufferedAmount(int dc);
extern void rtcSetBufferedAmountLowThreshold(int dc, int threshold);
extern int rtcSendMessage(int dc, const char *buffer, int size);
extern void rtcSetUserPointer(int i, void *ptr);

extern int wsCreateWebSocket(const char *url);
extern void wsDeleteWebSocket(int ws);
extern void wsSetOpenCallback(int ws, void (*openCallback)(void *));
extern void wsSetErrorCallback(int ws, void (*errorCallback)(const char *, void *));
extern void wsSetMessageCallback(int ws, void (*messageCallback)(const char *, int, void *));
extern int wsSendMessage(int ws, const char *buffer, int size);
extern void wsSetUserPointer(int ws, void *ptr);

}
#else
#include "rtc/rtc.h"
#endif

#endif //W_RTC_HPP
