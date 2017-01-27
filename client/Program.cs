namespace Client
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Win32.SafeHandles;
    using Newtonsoft.Json;

    public class Program
    {
        public static void Main(string[] args)
        {
            var itemCount = int.Parse(args[0]);
            var tasks = new [] { new Task(() => Json(itemCount)) };
            foreach(var t in tasks) {
                t.Start();
            }
            Task.WaitAll(tasks);
        }

        public static void Json(int itemCount)
        {
            var start = DateTime.Now;
            var pipe = GetPipeClient("d2n-json-pipe");
            var itemsSent = 0;

            var serializer = new JsonSerializer();
            var data = TestData.Generate(itemCount);
            uint bytesWritten = 0;
            byte[] buffer = new byte[100 * 1000 * 1000];
            
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
                var chunkBytesWritten = Write(pipe, buffer, bufferSize);
                bytesWritten += chunkBytesWritten;
                Console.WriteLine("JSON: {1} % done, wrote {0} mbytes", chunkBytesWritten / 1024 / 1024, (float)itemsSent / (float)itemCount);
            }

            var duration = DateTime.Now.Subtract(start).TotalSeconds;

            Console.WriteLine("Total {0} mbytes, duration {1} secs", bytesWritten / 1024 / 1024, duration);
            Win32.CloseHandle(pipe);
        }

        static uint Write(SafeFileHandle pipe, byte[] bytes, uint byteToWrite)
        {
            uint bytesWritten = 0;
            
            if (!Win32.WriteFile(pipe, bytes, byteToWrite, out bytesWritten, IntPtr.Zero))
            {
                var code = Marshal.GetLastWin32Error();
                var errMsg = new Win32Exception(code).Message;
                Console.WriteLine("WriteFile to pipe failed. GLE={0}/{0}", code, errMsg);
            }
            return bytesWritten;
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
