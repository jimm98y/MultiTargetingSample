using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MultiTargetingSampleApp
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process2(
            IntPtr process,
            out ushort processMachine,
            out ushort nativeMachine
        );

        public static bool IsARM64()
        {
            IntPtr handle = Process.GetCurrentProcess().Handle;
            IsWow64Process2(handle, out _, out ushort nativeMachine);
            return nativeMachine == 0xaa64;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;

            // using dotnet.exe directly means we'd have to provide our own window and then embed hWnd of the NET core app... 
            //processInfo.FileName = "dotnet.exe";
            //processInfo.Arguments = "./MultiTargetingSampleApp.dll";

            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace("Launcher", "");
            switch (System.Runtime.InteropServices.RuntimeInformation.OSArchitecture)
            {
                case System.Runtime.InteropServices.Architecture.X86:
                    {
                        processInfo.FileName = $"{processName}_x86.exe";
                    }
                    break;

                // OS is lying us on ARM64, so this will probably never be true
                case System.Runtime.InteropServices.Architecture.Arm64:
                    {
                        processInfo.FileName = $"{processName}_ARM64.exe";
                    }
                    break;

                default:
                    {
                        if (IsARM64())
                        {
                            processInfo.FileName = $"{processName}_ARM64.exe";
                        }
                        else
                        {
                            processInfo.FileName = $"{processName}_x64.exe";
                        }
                    }
                    break;
            }

            // pass our arguments to the process being launched
            processInfo.Arguments = string.Join(" ", args);

            var process = Process.Start(processInfo);
            process.WaitForInputIdle();

            // optional: we can either wait, or exit immediately...
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
