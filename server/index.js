const win32NamedPipes = require('./win32-named-pipes');
const tcpSocket = require('./tcp-socket');
const runner = require('./runner');

const testCase = process.argv[2];
const tcpPort = parseInt(process.argv[3], 10);

if (['pipe', 'tcp'].indexOf(testCase) === -1) {
    throw 'invalid type';
}

if (testCase === 'pipe') {
    runner.start(win32NamedPipes, 'd2n-json-pipe', 'json');
} else if (testCase === 'tcp') {
    runner.start(tcpSocket, tcpPort, 'json');
}
