.NET Core to NodeJS Pipe Bench
----------------------------

This repo contains some basic code that benchmarks pipe throughput from
.NET Core to NodeJS. Win32 and JSON only for now.

Start the server:

    node server/index.js

Restore and run the client, passing the number of dummy DTOs to generate:

    dotnet restore
    dotnet run -p client 1000000

The programs outputs some basic stats on performance.
