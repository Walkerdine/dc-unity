using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace WebRTC {
    
    public class DataChannel : Channel {
        
        public DataChannel(int datachannel) {
            id = datachannel;
            SetUserPointer();
            if (instances == null)
                instances = new Dictionary<IntPtr, Channel>();
    
            instances[(IntPtr)id] = this;
        }
        
        public string Label() {
            IntPtr strPtr = Marshal.AllocHGlobal(512);
            int size=512;
            DataChannel_label(id,strPtr,size);
            string label = Marshal.PtrToStringAnsi(strPtr);
            Marshal.FreeHGlobal(strPtr);
            return label;
        }
        
        [DllImport(DLL.DLL_NAME)]
        private static extern int DataChannel_label(int id, IntPtr buffer, int size);
    
        [DllImport(DLL.DLL_NAME)]
        private static extern void DataChannel_close(int id);
    }
}