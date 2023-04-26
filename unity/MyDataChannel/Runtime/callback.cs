
namespace WebRTC {
    
    public static class DLL {
    #if !UNITY_WEBGL
        public const string DLL_NAME = "DataChannelUnity";
    #else
        public const string DLL_NAME = "__Internal";
    #endif 
    }
}