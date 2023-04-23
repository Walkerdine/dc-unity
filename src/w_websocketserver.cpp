#include "w_websocketserver.hpp"

namespace rtc {
extern "C" {
#if !__EMSCRIPTEN__

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_new(const rtcWsServerConfiguration *config, rtcWebSocketClientCallbackFunc cb)
{
    return rtcCreateWebSocketServer(config, cb);
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_delete(int wsserver)
{
    return rtcDeleteWebSocketServer(wsserver);
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_getPort(int wsserver)
{
    return rtcGetWebSocketServerPort(wsserver);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocketServer_setUserPointer(int wsserver, void * ptr)
{
    rtcSetUserPointer(wsserver, ptr);
}
#endif

} // extern "C"
}