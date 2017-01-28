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
            let offset = 0;
            const list = [];
            splits.forEach(count => {
                const off = offset;
                offset += count + 1;
                list.push(Buffer.from(buffer, off, count));
            });
            if (offset < buffer.length) {
                list.push(Buffer.from(buffer, offset));
            }
            return list;
        };
        this.handleWrite = (data) => {
            if (this.expectedBytes === 0) {
                this.buffers = [];
                if (data.byteLength === 4) {
                    this.expectedBytes = data.readUInt32LE();
                } else {
                    const [head, rest] = this.splitBuffer(data, 4);
                    this.expectedBytes = head.readUInt32LE();
                    this.buffers.push(rest);
                }
                console.log('expectedBytes', this.expectedBytes);    
            } else {
                this.expectedBytes -= data.byteLength;
                this.buffers.push(data);
                if (this.expectedBytes === 0) {
                    console.log('emitting');
                }
            }
            if (this.expectedBytes < 0) {
                throw 'unexpected?';
            }
            
            // if (this.expectedBytes === 0) {
            //     const [head, rest] = this.splitBuffer(data, 4);
            //     this.expectedBytes = head.readUInt32LE();
            //     if (this.buffers.length > 0) {
            //         this.emit('message', Buffer.concat(this.buffers));
            //     }
            //     this.buffers = [];
            //     if (rest) {
            //         this.buffers.push(rest);
            //     }
            // } else if (this.expectedBytes - data.length < 0) {
            //     const [remainder, head, rest] = this.splitBuffer(data, this.expectedBytes, 4);
            //     this.expectedBytes -= data.length;
            //     this.buffers.push(data);
            //     if (this.expectedBytes === 0) {
            //         this.emit('message', Buffer.concat(this.buffers));
            //         this.buffers = [];
            //     } else if (this.expectedBytes < 0) {
            //         throw 'unhandled 2';
            //     }
            // } else {
            //     console.log(this.expectedBytes, data.byteLength);
            //     throw 'unhandled';
            // }

            // if (this.expectedBytes <= 0) {
            //     // new buffer incoming
                
            //     const [head, rest] = this.splitBuffer(data, 4);
            //     this.expectedBytes = head.readUInt32LE();
            //     if (rest) {
            //         this.buffers.push(rest);
            //     }
            // } else if (this.expectedBytes - data.length < 0) {
            //     // splitting buffer
            //     const [remainder, head, rest] = this.splitBuffer(data, this.expectedBytes, 4);
            //     this.buffers.push(remainder);
            //     const msg = Buffer.concat(this.buffers);
            //     this.expectedBytes = head.readUInt32LE();
            //     this.buffers = [];
            //     console.log(rest);
            //     console.log('split expectedBytes', this.expectedBytes);
            //     if (rest) {
            //         this.buffers.push(rest);
            //     }
            //     console.log(`split buffer`);
            //     this.emit('message', msg);
            // } else if (this.expectedBytes - data.length === 0) {
            //     this.buffers.push(data);
            //     const msg = Buffer.concat(this.buffers);
            //     this.expectedBytes = 0;
            //     this.buffers = [];
            //     console.log(`ending buffer`);
            //     this.emit('message', msg);
            // } else {
            //     // continuing buffer
            //     this.buffers.push(data);
            //     this.expectedBytes -= data.length;
            // }
        };
        net.createServer((stream) => this.createStream(stream))
            .listen(portNumber);
    }
}

module.exports = {
    listen: (portNumber) => new TcpSocketEmitter(portNumber)
};
