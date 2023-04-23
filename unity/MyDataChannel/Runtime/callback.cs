using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using AOT;
using UnityEngine;

public static class DLL
{
#if !UNITY_WEBGL
    public const string DLL_NAME = "DataChannelUnity";
#else
    public const string DLL_NAME = "__Internal";
#endif 
}
