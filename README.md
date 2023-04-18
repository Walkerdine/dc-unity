# dc-unity
WebRTC datachannels plugin for unity with support for WebGL, Windows and Linux

## Installation

You just need to add libdatachannel and datachannel-wasm as a submodule in your Emscripten project:

```bash
$ git submodule add https://github.com/paullouisageneau/datachannel-wasm.git deps/datachannel-wasm
$ git submodule add https://github.com/paullouisageneau/libdatachannel.git deps/libdatachannel
$ git submodule update --init --recursive --depth 1
```

## Building
Build for Linux requires gcc
```bash
$ cmake -S. -B./build-linux/ -DCAPI_STDCALL=ON
$ cd build-linux
$ make -j2
```

Build for windows requires visual studio to be installed
```bash
$ cmake -S. -B./build-win/ -A x64 -DCAPI_STDCALL=ON
$ cd build-win
$ make -j2
```

Building the WebGL Plugin requires that you have [emsdk](https://github.com/emscripten-core/emsdk) installed and activated in your environment:
```bash
$ cmake -B./build-webgl/ -DCMAKE_TOOLCHAIN_FILE=$EMSDK/upstream/emscripten/cmake/Modules/Platform/Emscripten.cmake
$ cd build-webgl
$ make -j2
```
