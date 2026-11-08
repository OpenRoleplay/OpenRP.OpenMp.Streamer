cmake_minimum_required(VERSION 3.19)
project(OpenRP.OpenMp.Streamer.Native LANGUAGES CXX VERSION 1.0.0)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# open.mp SDK source can be provided via -DOMP_SDK_DIR=<path>. Default: sibling
# SampSharp submodule, which sits either next to this repo (standalone and
# Docker checkouts) or one level further up (inside VSRP: src/SampSharp with
# this repo under src/submodules/). Falls back to a nested copy.
if(NOT DEFINED OMP_SDK_DIR OR OMP_SDK_DIR STREQUAL "")
    if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/../omp-streamer-component/lib/open.mp-sdk/CMakeLists.txt")
        # OpenRP layout: the streamer component fork checked out next to this repo, same SDK revision.
        set(OMP_SDK_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../omp-streamer-component/lib/open.mp-sdk")
    elseif(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/../SampSharp/external/sdk/CMakeLists.txt")
        set(OMP_SDK_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../SampSharp/external/sdk")
    elseif(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/../../SampSharp/external/sdk/CMakeLists.txt")
        set(OMP_SDK_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../../SampSharp/external/sdk")
    elseif(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/external/open.mp-sdk/CMakeLists.txt")
        set(OMP_SDK_DIR "${CMAKE_CURRENT_SOURCE_DIR}/external/open.mp-sdk")
    else()
        message(FATAL_ERROR "open.mp SDK not found. Set -DOMP_SDK_DIR=<path> or make sure the SampSharp submodule is checked out at ../SampSharp or ../../SampSharp.")
    endif()
endif()
message(STATUS "OpenRP.OpenMp.Streamer.Native: using open.mp SDK at ${OMP_SDK_DIR}")

# Output path: use -DCOMPONENTS_DIR=<path/to/env/components> for direct deploy,
# otherwise build/artifacts/.
if(DEFINED COMPONENTS_DIR AND NOT COMPONENTS_DIR STREQUAL "")
    set(OUTDIR "${COMPONENTS_DIR}")
else()
    set(OUTDIR "${CMAKE_BINARY_DIR}/artifacts")
endif()

foreach(cfg "" "_DEBUG" "_RELEASE" "_RELWITHDEBINFO" "_MINSIZEREL")
    set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY${cfg} "${OUTDIR}")
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY${cfg} "${OUTDIR}")
    set(CMAKE_RUNTIME_OUTPUT_DIRECTORY${cfg} "${OUTDIR}")
endforeach()

add_subdirectory("${OMP_SDK_DIR}" "${CMAKE_BINARY_DIR}/external/sdk" EXCLUDE_FROM_ALL)

if(MSVC)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /std:c++latest /Zc:preprocessor")
endif()

add_library(OpenRP.OpenMp.Streamer.Native SHARED
    native/component.cpp
)

set_target_properties(OpenRP.OpenMp.Streamer.Native PROPERTIES OUTPUT_NAME "OpenRP.OpenMp.Streamer.Native" PREFIX "")
set_property(TARGET OpenRP.OpenMp.Streamer.Native PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

target_compile_definitions(OpenRP.OpenMp.Streamer.Native PRIVATE
    WIN32_LEAN_AND_MEAN
    VC_EXTRALEAN
    NOGDI
)

if(WIN32)
    target_compile_definitions(OpenRP.OpenMp.Streamer.Native PRIVATE NOMINMAX)
endif()

target_link_libraries(OpenRP.OpenMp.Streamer.Native PRIVATE OMP-SDK)
