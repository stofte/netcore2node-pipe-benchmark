const namedPipes = require('./named-pipes');
const StringDecoder = require('string_decoder').StringDecoder;

const decoder = new StringDecoder('utf8');
const jsonServer = namedPipes.listen('d2n-json-pipe');

const result = {
    json: { received: 0, letters: 0, duration: 0, rate: [], data: [] }
}

jsonServer.on('message', buffer => handleMsg('json', buffer));

function handleMsg(type, buffer) {
    if (type === 'json') {
        var start = new Date().getTime();
        var data = JSON.parse(decoder.write(buffer));
        var end = new Date().getTime();
        var diff = end - start;
        data.forEach(elm => {
            result.json.letters += elm.LoremIpsum.length;
        })
        result.json.data = result.json.data.concat(data);
        result.json.received += buffer.length;
        result.json.duration += diff;
        result.json.rate.push({ time: end, size: buffer.length });
    }
}


const windowSec = 10;
setInterval(() => {
    let now = new Date().getTime();
    let bytes = 0;
    let rates = result.json.rate;
    let i = rates.length - 1;
    while (i > 0 && rates[i].time + windowSec * 1000 > now) {
        bytes += rates[i].size;
        i--;
    }
    let log = '---';
    if (bytes !== 0) {
        let mbs = (bytes/1024/1024/windowSec).toFixed(3);
        log = `${mbs} mbytes/sec, duration ${result.json.duration} ms, count ${result.json.data.length}`;
    }
    console.log(`[${new Date().toLocaleTimeString()}] json: ${log}`);
}, windowSec * 1000);
