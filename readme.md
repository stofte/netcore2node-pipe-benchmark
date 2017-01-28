.NET Core to NodeJS transfer benchmark
--------------------------------------

This repo contains code that benchmarks (data) throughput from
.NET Core to NodeJS using some various tech:

- named pipes

Start the server:

    node server/index.js

Restore and run the client, passing the number of dummy DTOs to generate:

    dotnet restore
    dotnet run -p client 1000000

The programs outputs some basic stats on performance.
