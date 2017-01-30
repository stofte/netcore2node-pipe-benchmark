namespace Client
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Win32.SafeHandles;
    using Newtonsoft.Json;

    public enum TestIpc
    {
        NamedPipe,
        TcpSocket
    }

    public static class GuidHelper
    {
        public static string ToIdentifierWithPrefix(this Guid guid, string prefix)
        {
            return string.Format("{0}{1}", prefix, guid.ToString().Replace("-", ""));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var itemCount = int.Parse(args[0]);
            TestIpc test = TestIpc.NamedPipe;
            int portNumber = 0;
            if (args[1] == "tcp")
            {
                test = TestIpc.TcpSocket;
                portNumber = int.Parse(args[2]);
            }

            if (Linux.IsLinux())
            {
                IntPtr[] fds = new IntPtr[2];
                var pipe = Linux.pipe(fds);
                Console.WriteLine("Linux pipe => {0}", pipe);
                Linux.close(fds[0]);
                Console.WriteLine("fds => [{0}, {1}]", fds[0], fds[1]);
                var str = "hej mor";
                var bytes = Encoding.UTF8.GetBytes(str);
                Linux.write(fds[0], bytes, (uint)bytes.Length);
            }
            else if (Win32.IsWin32())
            {
                var tasks = new [] { new Task(() => Json(itemCount, test, portNumber)) };
                foreach(var t in tasks) {
                    t.Start();
                }
                Task.WaitAll(tasks);
            }
        }

        public static void Json(int itemCount, TestIpc ipcType, int portNumber)
        {
            SafeFileHandle pipe = null;
            Socket socket = null;

            var start = DateTime.Now;
            if (ipcType == TestIpc.NamedPipe)
            {
                pipe = GetPipeClient("d2n-json-pipe");
            }
            else if (ipcType == TestIpc.TcpSocket)
            {
                socket = GetSocket(portNumber);
            }
            var itemsSent = 0;

            var serializer = new JsonSerializer();
            var data = TestData.Generate(itemCount);
            uint bytesWritten = 0;
            byte[] buffer = new byte[100 * 1024 * 1024];
            byte[] headBuffer = new byte[4];
            
            while (true)
            {
                var chunk = data.Take(50000);
                chunk = chunk.ToList();
                itemsSent += chunk.Count();
                data = data.Skip(chunk.Count());
                if (!chunk.Any()) break;
                var stream = new MemoryStream(buffer);
                var writer = new StreamWriter(stream);
                var jsonWriter = new JsonTextWriter(writer);
                serializer.Serialize(jsonWriter, chunk);
                jsonWriter.Flush();
                var bufferSize = (uint) stream.Position;
                int chunkBytesWritten = 0;
                if (ipcType == TestIpc.NamedPipe)
                {
                    chunkBytesWritten = (int) WriteToPipe(pipe, buffer, bufferSize);
                    bytesWritten += (uint) chunkBytesWritten;
                }
                else if (ipcType == TestIpc.TcpSocket)
                {
                    WriteToSocket(socket, BitConverter.GetBytes(bufferSize), 4);
                    chunkBytesWritten = WriteToSocket(socket, buffer, (int)bufferSize);
                    bytesWritten += (uint) chunkBytesWritten;
                }
                Console.WriteLine("JSON: {0} %, {1} bytes", ((float)itemsSent / (float)itemCount) * 100, bufferSize);
            }

            var duration = DateTime.Now.Subtract(start).TotalSeconds;

            Console.WriteLine("Total {0} mbytes, duration {1} secs", bytesWritten / 1024 / 1024, duration);
            if (ipcType == TestIpc.NamedPipe)
            {
                Win32.CloseHandle(pipe);
            }
            else if (ipcType == TestIpc.TcpSocket)
            {
                socket.Dispose();
            }
        }

        static int WriteToSocket(Socket socket, byte[] buffer, int bytesToWrite)
        {
            return socket.Send(buffer, bytesToWrite, SocketFlags.None);
        }

        static uint WriteToPipe(SafeFileHandle pipe, byte[] buffer, uint bytesToWrite)
        {
            uint bytesWritten = 0;
            
            if (!Win32.WriteFile(pipe, buffer, bytesToWrite, out bytesWritten, IntPtr.Zero))
            {
                var code = Marshal.GetLastWin32Error();
                var errMsg = new Win32Exception(code).Message;
                Console.WriteLine("WriteFile to pipe failed. GLE={0}/{0}", code, errMsg);
            }
            return bytesWritten;
        }

        static Socket GetSocket(int portNumber)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, portNumber);
            Socket socket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp );
            socket.Connect(endpoint);
            return socket;
        }

        static SafeFileHandle GetPipeClient(string name)
        {
            var pipeName = "\\\\.\\pipe\\" + name;
            SafeFileHandle pipe;
            while (true)
            {
                pipe = Win32.CreateFileW(pipeName, FileAccess.Write, FileShare.None, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
                if (!pipe.IsInvalid)
                {
                    break;
                }

                var errCode = Marshal.GetLastWin32Error();
                if (errCode != Win32.ERROR_PIPE_BUSY)
                {
                    Console.WriteLine("Could not open pipe. GLE={0}", new Win32Exception(errCode).Message);
                    return null;
                }

                if (!Win32.WaitNamedPipe(pipeName, 20000))
                {
                    Console.WriteLine("Could not open pipe: 20 second wait timed out.");
                    return null;
                }
            }
            return pipe;
        }
    }
}
