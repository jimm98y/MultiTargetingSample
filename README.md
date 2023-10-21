# MultiTargetingSample
In NET Framework, it is possible to build EXE for Any CPU and run it on x86 or x64 OS.

In NET Core, the host EXE has to target a specific platform, which means even though the app is using Any CPU DLLs and it is published as "Framework Dependent", the included EXE is actually platform specific and for Any CPU it is resolved to the same platform as the OS where the app is built. 

Officially, it is recommended to publish the app for each individual runtime, which significantly increases the build time for large applications with many dependencies. This sample shows how to build and publish the app only once for Any CPU (Portable, Framework Dependent, Any CPU) and include the EXE hosts for all Windows platforms in the output (*.EXE for the default x64, *_x86.EXE, *_ARM64.EXE).

The solution is to include the following XML in the csproj:
```xml
<PropertyGroup>
    <!-- Do not generate the default app host which for Any CPU depends upon the OS where we build - x64 for x64 OS, ARM64 for ARM64 OS -->
    <UseAppHost>false</UseAppHost>
</PropertyGroup>
<Target Name="CustomAppHostBuild" AfterTargets="AfterCompile">
    <PropertyGroup>
        <!-- Console app or Windows UI app? -->
        <CustomUseWindowsGraphicalUserInterface Condition="'$(OutputType)'=='WinExe'">true</CustomUseWindowsGraphicalUserInterface>
    </PropertyGroup>
    <PropertyGroup Condition="'$(TargetFrameworkVersion)'=='v5.0'">
        <CustomTargetNetSdk>$(_RuntimePackInWorkloadVersion5)</CustomTargetNetSdk>
    </PropertyGroup>
    <PropertyGroup Condition="'$(TargetFrameworkVersion)'=='v6.0'">
        <CustomTargetNetSdk>$(_RuntimePackInWorkloadVersion6)</CustomTargetNetSdk>
    </PropertyGroup>
    <PropertyGroup Condition="'$(TargetFrameworkVersion)'=='v7.0'">
        <CustomTargetNetSdk>$(_RuntimePackInWorkloadVersion7)</CustomTargetNetSdk>
    </PropertyGroup>
    <PropertyGroup Condition="'$(TargetFrameworkVersion)'=='v8.0'">
        <CustomTargetNetSdk>$(_RuntimePackInWorkloadVersion8)</CustomTargetNetSdk>
    </PropertyGroup>
    <PropertyGroup Condition="'$(TargetFrameworkVersion)'=='v9.0'">
        <CustomTargetNetSdk>$(_RuntimePackInWorkloadVersion9)</CustomTargetNetSdk>
    </PropertyGroup>
    <!-- Generate hosts per-platform: x64 will be the default app host no matter the OS where we build -->
    <CreateAppHost 
        AppHostSourcePath="$(NetCoreTargetingPackRoot)\Microsoft.NETCore.App.Host.win-x64\$(CustomTargetNetSdk)\runtimes\win-x64\native\apphost.exe" 
        AppBinaryName="$(AssemblyName).dll"
        WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)"
        AppHostDestinationPath="$(TargetDir)\$(AssemblyName).exe" 
        IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
    <CreateAppHost 
        AppHostSourcePath="$(NetCoreTargetingPackRoot)\Microsoft.NETCore.App.Host.win-x86\$(CustomTargetNetSdk)\runtimes\win-x86\native\apphost.exe" 
        AppBinaryName="$(AssemblyName).dll" 
        WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)" 
        AppHostDestinationPath="$(TargetDir)\$(AssemblyName)_x86.exe" 
        IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
    <CreateAppHost 
        AppHostSourcePath="$(NetCoreTargetingPackRoot)\Microsoft.NETCore.App.Host.win-arm64\$(CustomTargetNetSdk)\runtimes\win-arm64\native\apphost.exe" 
        AppBinaryName="$(AssemblyName).dll" 
        WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)" 
        AppHostDestinationPath="$(TargetDir)\$(AssemblyName)_ARM64.exe" 
        IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
</Target>
<ItemGroup>
    <!-- Include the generated hosts in the publish output -->
    <None Include="$(TargetDir)\$(AssemblyName).exe">
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
        <Visible>False</Visible>
        <Link>%(Filename)%(Extension)</Link>
    </None>
    <None Include="$(TargetDir)\$(AssemblyName)_x86.exe">
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
        <Visible>False</Visible>
        <Link>%(Filename)%(Extension)</Link>
    </None>
    <None Include="$(TargetDir)\$(AssemblyName)_ARM64.exe">
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
        <Visible>False</Visible>
        <Link>%(Filename)%(Extension)</Link>
    </None>
</ItemGroup>
```