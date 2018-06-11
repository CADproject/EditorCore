## Project preparation

1) Clone repository: `git clone https://github.com/CADproject/EditorCore.git`
 
2) Go to folder "builds": `cd ./builds`
 
3) Run cmake:
 
    for MAC and Linux:
 `cmake -DCMAKE_CXX_FLAGS={-m32/-m64} -DCMAKE_BUILD_TYPE={Debug/Release} ../src/`
 
    for Windows:
`cmake {-DCMAKE_GENERATOR_PLATFORM=x64} ..\src\`
