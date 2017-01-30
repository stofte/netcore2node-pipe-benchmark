.NET Core to NodeJS transfer benchmark
--------------------------------------

This repo contains code that benchmarks (data) throughput from
.NET Core to NodeJS using some various approaches.

Running
-------

Start the server (optionally use [--max-old-space-size](https://github.com/nodejs/node/issues/7937) to increase the heap size).
Pass `tcp 1234` to test using a TCP socket on port 1234, otherwise pass `pipes` to 

    node --max-old-space-size=4096 server/index.js [arg]

`[arg]` being one of 

 - `tcp 1234` for TCP on port 1234
 - `pipe` for Named Pipes (Win32 only)

Restore and run the client, passing the number of dummy DTOs to generate as well as the same args the server was started with.

    dotnet restore
    dotnet run -p client 1000000 [arg]

The programs outputs some basic stats on performance.
