#ifndef W_WEBSOCKETSERVER_HPP
#define W_WEBSOCKETSERVER_HPP

#include "IUnityInterface.h"
#include "w_rtc.hpp"

namespace rtc {
extern "C" {
#if !__EMSCRIPTEN__
UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_new(const rtcWsServerConfiguration *config, rtcWebSocketClientCallbackFunc cb);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_delete(int wsserver);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocketServer_getPort(int wsserver);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocketServer_setUserPointer(int wsserver, void * ptr);
#endif
} // extern "C"
}
#endif //W_WEBSOCKETSERVER_HPP