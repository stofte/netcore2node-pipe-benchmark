const StringDecoder = require('string_decoder').StringDecoder;
const decoder = new StringDecoder('utf8');

const result = {
    received: 0, letters: 0, duration: 0, rate: [], data: []
}

function handleMsg(type, buffer) {
    if (type === 'json') {
        var start = new Date().getTime();
        var data = JSON.parse(decoder.write(buffer));
        var end = new Date().getTime();
        var diff = end - start;
        data.forEach(elm => {
            result.letters += elm.LoremIpsum.length;
        })
        result.data = result.data.concat(data);
        result.received += buffer.length;
        result.duration += diff;
        result.rate.push({ time: end, size: buffer.length });
    }
}

const windowSec = 10;
function monitor() {
    let now = new Date().getTime();
    let bytes = 0;
    let rates = result.rate;
    let i = rates.length - 1;
    while (i > 0 && rates[i].time + windowSec * 1000 > now) {
        bytes += rates[i].size;
        i--;
    }
    let log = '---';
    if (bytes !== 0) {
        let mbs = (bytes/1024/1024/windowSec).toFixed(3);
        log = `${mbs} mbytes/sec, duration ${result.duration} ms, count ${result.data.length}`;
    }
    console.log(`[${new Date().toLocaleTimeString()}] json: ${log}`);
}

let timer = null;

module.exports = {
    start: (serverFactory, listenArg, msgType) => {
        const server = serverFactory.listen(listenArg);
        server.on('message', buffer => handleMsg(msgType, buffer));
        timer = setInterval(monitor, windowSec * 1000);
    },
    stop: () => {
        clearInterval(timer);
    }
};
