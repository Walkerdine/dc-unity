#ifndef W_WEBSOCKET_HPP
#define W_WEBSOCKET_HPP

#include "IUnityInterface.h"
#include "w_rtc.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_open(const char *url);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_delete(int id);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onOpen(int id, rtcOpenCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onError(int id, rtcErrorCallbackFunc callback);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_onMessage(int id, rtcMessageCallbackFunc callback);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API WebSocket_send(int id, const char *data, int size);

UNITY_INTERFACE_EXPORT bool UNITY_INTERFACE_API WebSocket_isOpen(int id); 

UNITY_INTERFACE_EXPORT bool UNITY_INTERFACE_API WebSocket_isClosed(int id);

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API WebSocket_setUserPointer(int id, void * ptr);

} // extern "C"
}
#endif //W_WEBSOCKET_HPP