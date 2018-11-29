'use strict';//hoisting을 막기위해 

const debug = require('debug')('TcpBufferHandler');

function TcpBufferHandler(socket, handler) {
  debug(' _onData =');

  this.socket = socket;
  this._packet = {};
  
  this._process = false;
  this._state = 'HEADER';
  this._payloadLength = 0;
  this._bufferedBytes = 0;
  this.queue = [];

  this.handler = handler;
}

TcpBufferHandler.prototype.init = function () {
debug('init');	
  this.socket.on('data', (data) => {
    this._bufferedBytes += data.length;
    this.queue.push(data);

    this._process = true;
    this._onData();
  });

  this.socket.on('served', this.handler);
};

NetwoTcpBufferHandlerrker.prototype._hasEnough = function (size) {
  if (this._bufferedBytes >= size) {
    return true;
  }
  this._process = false;
  return false;
}

TcpBufferHandler.prototype._readBytes = function (size) {
  let result;
  this._bufferedBytes -= size;

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

TcpBufferHandler.prototype._getHeader = function () {
  //if (this._hasEnough(2)) {
  if (this._hasEnough(4)) {
    //this._payloadLength = this._readBytes(2).readUInt16BE(0, true);
    this._payloadLength = this._readBytes(4).readUInt32LE(0, true);
    debug(' _getHeader...|| this._payloadLength =', this._payloadLength);
    this._state = 'PAYLOAD';
  }
}

TcpBufferHandler.prototype.getBody = function () {
  if (this._hasEnough(this._payloadLength)) {
    let received = this._readBytes(this._payloadLength);
    this.socket.emit('served', received);
    this._state = 'BODY';
  }
}

TcpBufferHandler.prototype._onData = function (data) {
  debug(' _onData...|| this._process =', this._process);

  while (this._process) {
    switch (this._state) {
      case 'HEADER':
        this._getHeader();
        break;
      case 'BODY':
        this.getBody();
        break;
    }
  }
}

TcpBufferHandler.prototype.send = function (message) {
  let buffer = Buffer.from(message);
  this._header(buffer.length);
  this._packet.message = buffer;
  this._send();
}

TcpBufferHandler.prototype._header = function (messageLength) {
  this._packet.header = { length: messageLength };
};

TcpBufferHandler.prototype._send = function () {
  //let contentLength = Buffer.allocUnsafe(2);
  //contentLength.writeUInt16BE(this._packet.header.length);
  let contentLength = Buffer.allocUnsafe(4);
  contentLength.writeUInt32LE(this._packet.header.length);
  this.socket.write(contentLength);
  this.socket.write(this._packet.message);
  this._packet = {};
};

module.exports = Networker;