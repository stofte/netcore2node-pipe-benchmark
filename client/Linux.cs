namespace Client
{
    using System;
    using System.Runtime.InteropServices;
    using System.Linq;

    public static class Linux
    {
        public static bool IsLinux()
        {
            return RuntimeInformation.OSDescription.StartsWith("Linux");
        }

        [DllImport("libc.so.6")]
        public static extern int getpid();

        [DllImport("libc.so.6")]
        public static extern int pipe(IntPtr[] fds);

        [DllImport("libc.so.6")]
        public static extern int close(IntPtr fd);

        [DllImport("libc.so.6")]
        public static extern int write(IntPtr fd, byte[] buf, uint count);
        
    }
}