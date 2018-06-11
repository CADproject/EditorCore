cmake_minimum_required(VERSION 3.11.1)
set(CMAKE_CXX_STANDARD 17)

project(EditorCore)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/includes/)

file(GLOB_RECURSE HEADERS "*.h")
file(GLOB SOURCE "*.cpp")

add_executable(${PROJECT_NAME} ${SOURCE} ${HEADERS})
