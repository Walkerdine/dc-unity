#include "w_datachannel.hpp"

namespace rtc {
extern "C" {

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API DataChannel_close(int dc)
{
    rtcDeleteDataChannel(dc);
}

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API DataChannel_label(int dc, char * buffer, int size)
{
    return rtcGetDataChannelLabel(dc,buffer,size);
}

} // extern "C"
}