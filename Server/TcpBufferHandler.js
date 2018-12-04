'use strict';//hoisting을 막기위해 

const debug = require('debug')('TcpBufferHandler');
const HEADER = "HEADER";
const BODY = "BODY";

module.exports = TcpBufferHandler;

function TcpBufferHandler(socket, handler) {
  debug('new TcpBufferHandler');

  this.socket = socket;
  this.packet = {};
  
  this.process = false;
  this.state = HEADER;
  this.bodySzie = 0;
  this.bufferedBytes = 0;
  this.queue = [];
  this.handler = handler;
}

TcpBufferHandler.prototype.init = function () {
debug('init');	
  this.socket.on('data', (data) => {
    this.bufferedBytes += data.length;
    this.queue.push(data);

    this.process = true;
    this.onData();
  });

  this.socket.on('served', this.handler);
};

TcpBufferHandler.prototype.hasEnough = function (size) {
  if (this.bufferedBytes >= size) {
    return true;
  }
  this.process = false;
  return false;
}

TcpBufferHandler.prototype.readBytes = function (size) {
  let result;
  this.bufferedBytes -= size;

  if (size === this.queue[0].length) {
    return this.queue.shift();
  }

  if (size < this.queue[0].length) {
    result = this.queue[0].slice(0, size);
    this.queue[0] = this.queue[0].slice(size);
    return result;
  }
  
  result = Buffer.allocUnsafe(size);
  let offset = 0;
  let length;
  
  while (size > 0) {
    length = this.queue[0].length;

    if (size >= length) {
      this.queue[0].copy(result, offset);
      offset += length;
      this.queue.shift();
    } else {
      this.queue[0].copy(result, offset, 0, size);
      this.queue[0] = this.queue[0].slice(size);
    }

    size -= length;
  }

  return result;
}

TcpBufferHandler.prototype.getHeader = function () {
  if (this.hasEnough(4)) {
    this.bodySzie = this.readBytes(4).readUInt32LE(0, true);
    debug('[getHeader] this.bodySzie =', this.bodySzie);
    this.state = BODY;
  }
}

TcpBufferHandler.prototype.getBody = function () {
  if (this.hasEnough(this.bodySzie)) {
    let received = this.readBytes(this.bodySzie);
    this.socket.emit('served', received);
    this.state = HEADER;
  }
}

TcpBufferHandler.prototype.onData = function (data) {
  debug('[onData] this.process =', this.process);
  while (this.process) {
    switch (this.state) {
      case HEADER:
        this.getHeader();
        break;
      case BODY:
        this.getBody();
        break;
    }
  }
}

TcpBufferHandler.prototype.send = function (message) {
  let buffer = Buffer.from(message);
  this.header(buffer.length);
  this.packet.message = buffer;
  this._send();
}

TcpBufferHandler.prototype.header = function (messageLength) {
  this.packet.header = { length: messageLength };
};

TcpBufferHandler.prototype._send = function () {
  //let contentLength = Buffer.allocUnsafe(2);
  //contentLength.writeUInt16BE(this._packet.header.length);
  let contentLength = Buffer.allocUnsafe(4);
  contentLength.writeUInt32LE(this.packet.header.length);
  this.socket.write(contentLength);
  this.socket.write(this.packet.message);
  this.packet = {};
};

