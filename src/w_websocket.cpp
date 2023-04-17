#include "w_websocket.hpp"

namespace rtc {
extern "C" {
#if __EMSCRIPTEN__

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_open(const char *url)
{
    return wsCreateWebSocket(url);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_close(int id)
{
    wsDeleteWebSocket(id);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onOpen(int id, rtcOpenCallbackFunc callback)
{
    wsSetOpenCallback(id, callback);
}

UNITY_INTERFACE_EXPORT  void UNITY_INTERFACE_API WebSocket_onError(int id, rtcErrorCallbackFunc callback)
{
    wsSetErrorCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onMessage(int id, rtcMessageCallbackFunc callback)
{
    wsSetMessageCallback(id,callback);
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_send(int id, const char *data, int size)
{
    return wsSendMessage(id,data,size);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_setUserPointer(int id, void * ptr)
{
    return wsSetUserPointer(id,ptr);
}

#else
    
UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_open(const char *url)
{
    return rtcCreateWebSocket(url);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_close(int id)
{
    rtcDeleteWebSocket(id);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onOpen(int id, rtcOpenCallbackFunc callback)
{
    rtcSetOpenCallback(id, callback);
}

UNITY_INTERFACE_EXPORT  void UNITY_INTERFACE_API WebSocket_onError(int id, rtcErrorCallbackFunc callback)
{
    rtcSetErrorCallback(id, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onMessage(int id, rtcMessageCallbackFunc callback)
{
    rtcSetMessageCallback(id,callback);

}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_send(int id, const char *data, int size)
{
    return rtcSendMessage(id,data,size);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_setUserPointer(int id, void * ptr)
{
    rtcSetUserPointer(id,ptr);
}

#endif

} // extern "C"
}