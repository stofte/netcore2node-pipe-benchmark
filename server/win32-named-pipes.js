const EventEmitter = require('events').EventEmitter;
const net = require('net');

class Win32PipeServerEmitter extends EventEmitter {
    constructor(pipeName) {
        super();
        this.pipeAddress = `\\\\.\\pipe\\${pipeName}`;
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
    listen: (pipeName) => new Win32PipeServerEmitter(pipeName)
};
