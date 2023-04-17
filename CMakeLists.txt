cmake_minimum_required(VERSION 3.7)
project(dc-unity
    DESCRIPTION "Datachannel-unity"
    VERSION 0.1.0
    LANGUAGES CXX)
    
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)

include_directories(include)
file(GLOB SOURCES "src/*.cpp")

if(CMAKE_SYSTEM_NAME MATCHES "Emscripten")
    add_library(DataChannelUnity STATIC ${SOURCES})
    file(COPY "${CMAKE_CURRENT_SOURCE_DIR}/deps/datachannel-wasm/wasm/js/webrtc.js" DESTINATION "${CMAKE_BINARY_DIR}/lib")
    file(COPY "${CMAKE_CURRENT_SOURCE_DIR}/deps/datachannel-wasm/wasm/js/websocket.js" DESTINATION "${CMAKE_BINARY_DIR}/lib")
    file(RENAME "${CMAKE_BINARY_DIR}/lib/webrtc.js" "${CMAKE_BINARY_DIR}/lib/webrtc.jslib")
    file(RENAME "${CMAKE_BINARY_DIR}/lib/websocket.js" "${CMAKE_BINARY_DIR}/lib/websocket.jslib")
else()
    add_library(DataChannelUnity SHARED ${SOURCES})
    option(NO_MEDIA "Disable media support in libdatachannel" ON)
    add_subdirectory(deps/libdatachannel EXCLUDE_FROM_ALL)
    target_link_libraries(DataChannelUnity datachannel)
endif()

if(MSVC)
  target_compile_options(DataChannelUnity PRIVATE /MP /W4 /WX)
else()
  target_compile_options(DataChannelUnity PRIVATE -Wall -Wextra -Wpedantic)
endif()

