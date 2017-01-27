const EventEmitter = require('events').EventEmitter;
const net = require('net');

class PipeServerEmitter extends EventEmitter {
    constructor(pipeName, win32 = true) {
        super();
        this.pipeAddress = win32 ? `\\\\.\\pipe\\${pipeName}` : `/tmp/${pipeName}.pipe`;
        this.createStream = (stream) => {
            return stream.on('data', this.handleWrite);
        }
        this.handleWrite = (data) => {
            this.emit('message', data);
        }
        net.createServer((stream) => this.createStream(stream))
            .listen(this.pipeAddress);
    }
}

module.exports = {
    listen: (pipeName) => new PipeServerEmitter(pipeName)
};
