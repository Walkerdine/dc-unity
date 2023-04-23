#include "w_channel.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API Channel_send(int id, const char *data, int size)
{
    return rtcSendMessage(id, data, size);
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API Channel_bufferedAmount(int id)
{
    return rtcGetBufferedAmount(id);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onOpen(int id, rtcOpenCallbackFunc callback)
{
    rtcSetOpenCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onError(int id, rtcErrorCallbackFunc callback) 
{
    rtcSetErrorCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onMessage(int id, rtcMessageCallbackFunc callback) 
{
    rtcSetMessageCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_onBufferedAmountLow(int id, rtcBufferedAmountLowCallbackFunc callback) 
{
    rtcSetBufferedAmountLowCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_setBufferedAmountLowThreshold(int id, int amount)
{
    rtcSetBufferedAmountLowThreshold(id, amount);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Channel_setUserPointer(int id, void * ptr)
{
    rtcSetUserPointer(id, ptr);
}

} // extern "C"
}