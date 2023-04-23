#include "w_websocket.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_open(const char *url)
{
#if __EMSCRIPTEN__
    return wsCreateWebSocket(url);
#else
    return rtcCreateWebSocket(url);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_close(int id)
{
#if __EMSCRIPTEN__
    wsDeleteWebSocket(id);
#else
    rtcDeleteWebSocket(id);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onOpen(int id, rtcOpenCallbackFunc callback)
{
#if __EMSCRIPTEN__
    wsSetOpenCallback(id, callback);
#else
    rtcSetOpenCallback(id, callback);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onError(int id, rtcErrorCallbackFunc callback)
{
#if __EMSCRIPTEN__
    wsSetErrorCallback(id, callback);
#else
    rtcSetErrorCallback(id, callback);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onMessage(int id, rtcMessageCallbackFunc callback)
{
#if __EMSCRIPTEN__
    wsSetMessageCallback(id,callback);
#else
    rtcSetMessageCallback(id,callback);
#endif
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_send(int id, const char *data, int size)
{
#if __EMSCRIPTEN__
    return wsSendMessage(id,data,size);
#else
    return rtcSendMessage(id,data,size);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_setUserPointer(int id, void * ptr)
{
#if __EMSCRIPTEN__
    wsSetUserPointer(id,ptr);
#else
    rtcSetUserPointer(id,ptr);
#endif
}

} // extern "C"
}