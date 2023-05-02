#ifndef W_RTC_HPP
#define W_RTC_HPP

#include "IUnityInterface.h"

#if __EMSCRIPTEN__

#include <stdbool.h>
#include <stdint.h>
#include <string>

#define RTC_API
extern "C" {
typedef void(RTC_API *rtcDescriptionCallbackFunc)(int pc, const char *sdp, const char *type, void *ptr);
typedef void(RTC_API *rtcCandidateCallbackFunc)(int pc, const char *cand, const char *mid, void *ptr);
typedef void(RTC_API *rtcStateChangeCallbackFunc)(int pc, int state, void *ptr);
typedef void(RTC_API *rtcGatheringStateCallbackFunc)(int pc, int state, void *ptr);
typedef void(RTC_API *rtcSignalingStateCallbackFunc)(int pc, int state, void *ptr);
typedef void(RTC_API *rtcDataChannelCallbackFunc)(int pc, int dc, void *ptr);

typedef void(RTC_API *rtcOpenCallbackFunc)(int id, void *ptr);
typedef void(RTC_API *rtcClosedCallbackFunc)(int id, void *ptr);
typedef void(RTC_API *rtcErrorCallbackFunc)(int id, const char *error, void *ptr);
typedef void(RTC_API *rtcMessageCallbackFunc)(int id, const char *message, int size, void *ptr);
typedef void(RTC_API *rtcBufferedAmountLowCallbackFunc)(int id, void *ptr);

typedef enum {
    RTC_CERTIFICATE_DEFAULT = 0, // ECDSA
    RTC_CERTIFICATE_ECDSA = 1,
    RTC_CERTIFICATE_RSA = 2,
} rtcCertificateType;

typedef enum { RTC_TRANSPORT_POLICY_ALL = 0, RTC_TRANSPORT_POLICY_RELAY = 1 } rtcTransportPolicy;

extern int rtcCreatePeerConnection(const char **pUrls, const char **pUsernames,
              const char **pPasswords, int nIceServers);
extern void rtcDeletePeerConnection(int pc);
extern char *rtcGetLocalDescription(int pc);
extern char *rtcGetLocalDescriptionType(int pc);
extern char *rtcGetRemoteDescription(int pc);
extern char *rtcGetRemoteDescriptionType(int pc);
extern int rtcCreateDataChannel(int pc, const char *label, bool unordered, int maxRetransmits,
           int maxPacketLifeTime);
extern void rtcSetDataChannelCallback(int pc, void (*dataChannelCallback)(int, int, void *));
extern void rtcSetLocalDescriptionCallback(int pc,
                      void (*descriptionCallback)(int, const char *, const char *,
                                                  void *));
extern void rtcSetLocalCandidateCallback(int pc, void (*candidateCallback)(int, const char *,
                                                      const char *, void *));
extern void rtcSetStateChangeCallback(int pc, void (*stateChangeCallback)(int, int, void *));
extern void rtcSetGatheringStateChangeCallback(int pc,
                          void (*gatheringStateChangeCallback)(int, int, void *));
extern void rtcSetSignalingStateChangeCallback(int pc,
                          void (*signalingStateChangeCallback)(int, int, void *));
extern void rtcSetRemoteDescription(int pc, const char *sdp, const char *type);
extern void rtcAddRemoteCandidate(int pc, const char *candidate, const char *mid);
extern void rtcSetUserPointer(int i, void *ptr);
extern void rtcDeleteDataChannel(int dc);
extern int rtcGetDataChannelLabel(int dc, char *buffer, int size);

extern void rtcSetOpenCallback(int dc, void (*openCallback)(int, void *));
extern void rtcSetErrorCallback(int dc, void (*errorCallback)(int, const char *, void *));
extern void rtcSetMessageCallback(int dc, void (*messageCallback)(int, const char *, int, void *));
extern void rtcSetBufferedAmountLowCallback(int dc, void (*bufferedAmountLowCallback)(int, void *));
extern int rtcGetBufferedAmount(int dc);
extern void rtcSetBufferedAmountLowThreshold(int dc, int threshold);
extern int rtcSendMessage(int dc, const char *buffer, int size);
extern void rtcSetUserPointer(int i, void *ptr);

extern int wsCreateWebSocket(const char *url);
extern void wsDeleteWebSocket(int ws);
extern void wsSetOpenCallback(int ws, void (*openCallback)(int, void *));
extern void wsSetErrorCallback(int ws, void (*errorCallback)(int, const char *, void *));
extern void wsSetMessageCallback(int ws, void (*messageCallback)(int, const char *, int, void *));
extern int wsSendMessage(int ws, const char *buffer, int size);
extern void wsSetUserPointer(int ws, void *ptr);

}
#else
#include "rtc/rtc.h"
#endif

#endif //W_RTC_HPP
