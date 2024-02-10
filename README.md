# MultiTargetingSample
In NET Framework, it is possible to build EXE for Any CPU and run it on x86 or x64 OS.

In NET Core, the host EXE has to target a specific platform, which means even though the app is using Any CPU DLLs and it is published as "Framework Dependent", the included EXE is actually platform specific and for Any CPU it is resolved to the same platform as the OS where the app is built. 

Officially, it is recommended to publish the app for each individual runtime, which significantly increases the build time for large applications with many dependencies. This sample shows how to build and publish the app only once for Any CPU (Portable, Framework Dependent, Any CPU) and include the EXE hosts for all Windows platforms in the output.

The solution is to include the following XML in the csproj:
```xml
<PropertyGroup>
	<UseAppHost>false</UseAppHost>
</PropertyGroup>
<Target Name="CustomAppHostBuild" AfterTargets="AfterBuild">
	<PropertyGroup>
		<CustomUseWindowsGraphicalUserInterface Condition="'$(OutputType)'=='WinExe'">true</CustomUseWindowsGraphicalUserInterface>
		<MyProgramFilesPath>$([System.Environment]::ExpandEnvironmentVariables("%ProgramW6432%"))</MyProgramFilesPath>
	</PropertyGroup>
	<PropertyGroup>
		<CustomTargetPathx86>$(MyProgramFilesPath)\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x86</CustomTargetPathx86>
		<CustomTargetDirectoriesLengthMinusOnex86>$([MSBuild]::Subtract($([System.IO.Directory]::GetDirectories("$(CustomTargetPathx86)").Length), 1))</CustomTargetDirectoriesLengthMinusOnex86>
		<CustomTargetNetSdkx86>$([System.IO.Directory]::GetDirectories("$(CustomTargetPathx86)")[$(CustomTargetDirectoriesLengthMinusOnex86)])</CustomTargetNetSdkx86>

		<CustomTargetPathx64>$(MyProgramFilesPath)\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x64</CustomTargetPathx64>
		<CustomTargetDirectoriesLengthMinusOnex64>$([MSBuild]::Subtract($([System.IO.Directory]::GetDirectories("$(CustomTargetPathx64)").Length), 1))</CustomTargetDirectoriesLengthMinusOnex64>
		<CustomTargetNetSdkx64>$([System.IO.Directory]::GetDirectories("$(CustomTargetPathx64)")[$(CustomTargetDirectoriesLengthMinusOnex64)])</CustomTargetNetSdkx64>

		<CustomTargetPathARM64>$(MyProgramFilesPath)\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-arm64</CustomTargetPathARM64>
		<CustomTargetDirectoriesLengthMinusOneARM64>$([MSBuild]::Subtract($([System.IO.Directory]::GetDirectories("$(CustomTargetPathARM64)").Length), 1))</CustomTargetDirectoriesLengthMinusOneARM64>
		<CustomTargetNetSdkARM64>$([System.IO.Directory]::GetDirectories("$(CustomTargetPathARM64)")[$(CustomTargetDirectoriesLengthMinusOneARM64)])</CustomTargetNetSdkARM64>
	</PropertyGroup>
	
	<CreateAppHost AppHostSourcePath="$(CustomTargetNetSdkx64)\runtimes\win-x64\native\apphost.exe" AppBinaryName="$(AssemblyName).dll" WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)" AppHostDestinationPath="$(TargetDir)\$(AssemblyName)_x64.exe" IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
	<CreateAppHost AppHostSourcePath="$(CustomTargetNetSdkx86)\runtimes\win-x86\native\apphost.exe" AppBinaryName="$(AssemblyName).dll" WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)" AppHostDestinationPath="$(TargetDir)\$(AssemblyName)_x86.exe" IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
	<CreateAppHost AppHostSourcePath="$(CustomTargetNetSdkARM64)\runtimes\win-arm64\native\apphost.exe" AppBinaryName="$(AssemblyName).dll" WindowsGraphicalUserInterface="$(CustomUseWindowsGraphicalUserInterface)" AppHostDestinationPath="$(TargetDir)\$(AssemblyName)_ARM64.exe" IntermediateAssembly="$(IntermediateOutputPath)\$(AssemblyName).dll" />
</Target>
<ItemGroup>
	<None Include="$(TargetDir)\$(AssemblyName)_x64.exe">
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

## AnyCPU Launcher
There is also a sample NET Framework AnyCPU application that can be used to launch the correct EXE based upon the OS platform. It will be included in the publish output as MultiTargetingSampleAppLauncher.exe.

## Support
Tested in Visual Studio 2022 Version 17.8 on Windows 11 ARM64 with NET8 Version 8.0.0.