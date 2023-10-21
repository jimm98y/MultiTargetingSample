# MultiTargetingSample
In NET Framework, it is possible to build EXE for Any CPU and run it on x86 or x64 OS.

In NET Core, the host EXE has to target a specific platform, which means even though the app is using Any CPU DLLs and it is published as "Framework Dependent", the included EXE is actually platform specific and for Any CPU it is resolved to the same platform as the OS where the app is built. 

Officially, it is recommended to publish the app for each individual runtime, which significantly increases the build time for large applications with many dependencies. This sample shows how to build and publish the app only once for Any CPU (Portable, Framework Dependent, Any CPU) and include the EXE hosts for all Windows platforms in the output (*.EXE for the default x64, *_x86.EXE, *_ARM64.EXE).