#ifndef W_CHANNEL_HPP
#define W_CHANNEL_HPP

#include "IUnityInterface.h"
#include "w_rtc.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API Channel_send(int id, const char *data, int size);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API Channel_bufferedAmount(int id);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onOpen(int id, rtcOpenCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onClosed(int id, rtcClosedCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onError(int id, rtcErrorCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onMessage(int id, rtcMessageCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onBufferedAmountLow(int id, rtcBufferedAmountLowCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_setBufferedAmountLowThreshold(int id, int amount);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_setUserPointer(int id, void * ptr);

} // extern "C"
}

#endif //W_CHANNEL_HPP