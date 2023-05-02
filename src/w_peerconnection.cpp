#include "w_peerconnection.hpp"
#include <cstring>
#include <cstdlib>
#include <cctype>
#include <string>
#include <regex>
#include <vector>
#include <optional>
#include <cassert>

namespace rtc {

#if __EMSCRIPTEN__
bool parse_url(const std::string &url, std::vector<std::optional<std::string>> &result);
std::string url_decode(const std::string &str);
#endif

extern "C" {

UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_new(const char ** ice_servers, int ice_servers_count)
{
#if __EMSCRIPTEN__
    char** passwords = new char*[ice_servers_count];
    char** usernames = new char*[ice_servers_count];
    char** urls = new char*[ice_servers_count];
    int pc = -1;
    
    for(int i = 0; i<ice_servers_count; i++){
        std::vector<std::optional<std::string>> opt;

        parse_url(ice_servers[i], opt);
        
        std::optional<std::string> scheme, hostname, service, query;
        std::string url;
        
        if((scheme = opt[2])) 
            url += scheme.value() + ":";
        
        hostname = opt[10];
        url += hostname.value();
        
        if((service = opt[12])) 
            url += ":" + service.value();
        
        if((query = opt[15])) 
            url += "?" + query.value();
        
        std::string username = url_decode(opt[6].value_or(""));
        std::string password = url_decode(opt[8].value_or(""));
        
        usernames[i] = new char[username.size()+1];
        passwords[i] = new char[password.size()+1];
        urls[i] = new char[url.size()+1];
        
        std::strcpy(usernames[i], username.c_str());
        std::strcpy(passwords[i], password.c_str());
        std::strcpy(urls[i], url.c_str());
    }
    
    pc = rtcCreatePeerConnection((const char**)urls,(const char**)usernames,(const char**)passwords, ice_servers_count);
    
    for(int i = 0; i<ice_servers_count; i++){
        delete[] usernames[i];
        delete[] passwords[i];
        delete[] urls[i];
    }
    
    delete[] usernames;
    delete[] passwords;
    delete[] urls;
    
    return pc;
#else
    rtcConfiguration config;
    config.iceServers = ice_servers;
    config.iceServersCount = ice_servers_count;
    config.proxyServer = NULL;
    config.bindAddress = NULL;
    config.certificateType = RTC_CERTIFICATE_DEFAULT;
    config.iceTransportPolicy = RTC_TRANSPORT_POLICY_ALL;
    config.enableIceTcp = false;
    config.enableIceUdpMux = false;
    config.disableAutoNegotiation = false;
    config.forceMediaTransport = false;
    config.portRangeBegin = 0;
    config.portRangeEnd = 0;
    config.mtu = 0;
    config.maxMessageSize = 0;

    return rtcCreatePeerConnection(&config);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_delete(int pc)
{
    rtcDeletePeerConnection(pc);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_localDescription(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetLocalDescription(pc);
    strcpy(buffer,str);
    (void)size;
    free(str);
#else
    rtcGetLocalDescription(pc, buffer, size);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_remoteDescription(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetRemoteDescription(pc);
    strcpy(buffer,str);
    (void)size;
    free(str);
#else
    rtcGetRemoteDescription(pc, buffer, size);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_localDescriptionType(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetLocalDescriptionType(pc);
    strcpy(buffer,str);
    (void)size;
    free(str);
#else
    rtcGetLocalDescriptionType(pc, buffer, size);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_remoteDescriptionType(int pc, char *buffer, int size)
{
#if __EMSCRIPTEN__
    char* str = rtcGetRemoteDescriptionType(pc);
    strcpy(buffer,str);
    (void)size;
    free(str);
#else
    rtcGetRemoteDescriptionType(pc, buffer, size);
#endif
}


UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API PeerConnection_createDataChannel(int pc, const char* label, bool unordered, int maxRetransmits, int maxPacketLifeTime)
{
#if __EMSCRIPTEN__
    return rtcCreateDataChannel(pc, label, unordered, maxRetransmits, maxPacketLifeTime);
#else
    rtcDataChannelInit init;
    init.protocol = NULL;
    init.negotiated = false;
    init.manualStream = false;
    init.stream = 0;
    init.reliability.unordered = unordered;
    init.reliability.maxPacketLifeTime = maxPacketLifeTime;
    init.reliability.maxRetransmits = maxRetransmits;
    return rtcCreateDataChannelEx(pc, label, &init);
#endif
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setRemoteDescription(int pc, const char *sdp, const char *type)
{
    rtcSetRemoteDescription(pc,sdp,type);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_addRemoteCandidate(int pc, const char *cand, const char *mid)
{
    rtcAddRemoteCandidate(pc, cand, mid);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onDataChannel(int pc, rtcDataChannelCallbackFunc callback) 
{
    rtcSetDataChannelCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalDescription(int pc, rtcDescriptionCallbackFunc callback) 
{
    rtcSetLocalDescriptionCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onLocalCandidate(int pc, rtcCandidateCallbackFunc callback)
{
    rtcSetLocalCandidateCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onStateChange(int pc, rtcStateChangeCallbackFunc callback)
{
    rtcSetStateChangeCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onGatheringStateChange(int pc, rtcGatheringStateCallbackFunc callback)
{
    rtcSetGatheringStateChangeCallback(pc, callback);   
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_onSignalingStateChange(int pc, rtcSignalingStateCallbackFunc callback)
{
    rtcSetSignalingStateChangeCallback(pc, callback);
}

UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API PeerConnection_setUserPointer(int id, void * ptr)
{
    rtcSetUserPointer(id, ptr);
}

} // extern "C"

#if __EMSCRIPTEN__

bool parse_url(const std::string &url, std::vector<std::optional<std::string>> &result) {
    // Modified regex from RFC 3986, see https://www.rfc-editor.org/rfc/rfc3986.html#appendix-B
    static const char *rs =
        R"(^(([^:.@/?#]+):)?(/{0,2}((([^:@]*)(:([^@]*))?)@)?(([^:/?#]*)(:([^/?#]*))?))?([^?#]*)(\?([^#]*))?(#(.*))?)";
    static const std::regex r(rs, std::regex::extended);

    std::smatch m;
    if (!std::regex_match(url, m, r) || m[10].length() == 0)
        return false;

    result.resize(m.size());
    std::transform(m.begin(), m.end(), result.begin(), [](const auto &sm) {
        return sm.length() > 0 ? std::make_optional(std::string(sm)) : std::nullopt;
    });

    assert(result.size() == 18);
    return true;
}

std::string url_decode(const std::string &str) {
    std::string result;
    size_t i = 0;
    while (i < str.size()) {
        char c = str[i++];
        if (c == '%') {
            auto value = str.substr(i, 2);
            try {
                if (value.size() != 2 || !std::isxdigit(value[0]) || !std::isxdigit(value[1]))
                    throw std::exception();

                c = static_cast<char>(std::stoi(value, nullptr, 16));
                i += 2;

            } catch (...) {
                //PLOG_WARNING << "Invalid percent-encoded character in URL: \"%" + value + "\"";
            }
        }

        result.push_back(c);
    }

    return result;
}

#endif

}