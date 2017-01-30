const EventEmitter = require('events').EventEmitter;
const net = require('net');

class TcpSocketEmitter extends EventEmitter {
    constructor(portNumber) {
        super();
        this.buffers = [];
        this.expectedBytes = 0;
        this.createStream = (stream) => {
            return stream.on('data', this.handleWrite);
        };
        this.splitBuffer = (buffer, ...splits) => {
            // console.log(buffer.constructor.name);
            let offset = buffer.offset;
            let localOff = 0;
            const list = [];
            splits.forEach(count => {
                const off = offset;
                offset += count;
                localOff += count;
                // if (splits.length > 1) {
                //     console.log('splitBuffer.forEach', off, '=>', count);
                //     console.log('splitBuffer:', count, 'vs', Buffer.from(buffer.buffer, off, count).byteLength);
                // }
                list.push(Buffer.from(buffer.buffer, off, count));
            });
            if (localOff < buffer.length) {
                list.push(Buffer.from(buffer.buffer, offset));
            } else {
                // push an empty list for the while loop
                list.push(new Buffer([]));
            }
            return list;
        };
        this.handleWrite = (data) => {
            let buff = data;
            while (buff.byteLength > 0) {
                if (this.expectedBytes === 0) {
                    const [head, rest] = this.splitBuffer(buff, 4);
                    this.expectedBytes = head.readUInt32LE();
                    buff = rest;
                } else if (this.expectedBytes > 0) {
                    const readBytes = Math.min(this.expectedBytes, buff.byteLength);
                    const [remain, rest] = this.splitBuffer(buff, readBytes);
                    this.expectedBytes -= readBytes;
                    this.buffers.push(remain);
                    if (this.expectedBytes === 0) {
                        this.emit('message', Buffer.concat(this.buffers));
                        this.buffers = [];
                    }
                    buff = rest;
                } else {
                    throw 'unexpected';
                }
            }
        };
        net.createServer((stream) => this.createStream(stream))
            .listen(portNumber);
    }
}

module.exports = {
    listen: (portNumber) => new TcpSocketEmitter(portNumber)
};
