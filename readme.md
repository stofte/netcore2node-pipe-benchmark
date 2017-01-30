.NET Core to NodeJS transfer benchmark
--------------------------------------

This repo contains code that benchmarks (data) throughput from
.NET Core to NodeJS using some various approaches.

Running
-------

Start the server, use [--max-old-space-size](https://github.com/nodejs/node/issues/7937) to increase heap size.

`node --max-old-space-size=4096 server/index.js` *`method`*

The *`method`* argument determines which underlying approach to use.

 - `tcp n` for TCP on port n 
 - `pipe` for Named Pipes (Win32 only)

Restore and run the client, passing the number of dummy DTOs to generate
as well as the method the server was started with.

`dotnet restore`
`dotnet run -p client 1000000` *`method`*

The programs outputs some basic stats on performance.
