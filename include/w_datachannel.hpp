#ifndef W_DATACHANNEL_HPP
#define W_DATACHANNEL_HPP

#include "IUnityInterface.h"
#include "w_rtc.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API DataChannel_delete(int dc);

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API DataChannel_label(int dc, char * buffer, int size);

} // extern "C"
}

#endif //W_DATACHANNEL_HPP