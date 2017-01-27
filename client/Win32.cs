namespace Client
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    public static class Win32 
    {
        public const uint PIPE_WAIT	= 0x00000000;
        public const uint PIPE_READMODE_MESSAGE = 0x00000002;
        public const Int64 INVALID_HANDLE_VALUE = -1;
        public const int ERROR_PIPE_BUSY = 231;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WaitNamedPipe(
            string lpNamedPipeName,
            uint nTimeOut);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(
            SafeFileHandle hFile,
            byte [] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(SafeFileHandle hObject);
    }
}