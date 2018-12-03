/***************************************************************************************
* refernece link :  https://mylko72.gitbooks.io/node-js/content/chapter8/chapter8_3.html
****************************************************************************************/
var connectPort = 8107;
var timeOutDuration = 5000;
var net = require('net');
var util = require('util');
var color = require("colors");
const debug = require('debug')('TcpServer');
var receiver = require('./TcpReceiver.js');
const TcpBufferHandler = require('./TcpBufferHandler.js');
var BSON = require('bson');
var sockets = [];

module.exports.send = send;
module.exports.broadcastAll = broadcastAll;
module.exports.broadcastExcludedMe = broadcastExcludedMe;

var server = net.createServer(function(socket) {
    server.maxConnections = 1000;
    server.getConnections(function(err, count) {
       console.log("Connections: " + count +" /  maxConnections = " + server.maxConnections);
    });

  let bufferHandler = new TcpBufferHandler(socket, (data) => {
  	debug(' received:', data.toString());
  	debug(' received from socket / bytesRead = ' + socket.bytesRead + " / data length = " + data.length);
    receive(socket, data);
  });

  bufferHandler.init();
  sockets.push(socket);
  
  debug('Client connection: ');
  debug('   local = %s:%s', socket.localAddress, socket.localPort);
  debug('   remote = %s:%s', socket.remoteAddress, socket.remotePort);
  socket.setTimeout(timeOutDuration);
  //socket.setEncoding('utf8');
/*
  socket.on('data', function(data) {
    //debug('Received test data from socket on port %d: %s', socket.remotePort, data.toString());
    debug('received from socket / bytesRead = : ' + socket.bytesRead);
    debug('received from socket / data length = : ' + data.length);
    
    receive(socket, data);
    debug('  Bytes sent: ' + socket.bytesWritten);
  });
*/
  socket.on('end', function() {
    debug('Client disconnected');
    server.getConnections(function(err, count){
      debug('Remaining Connections: ' + count);
    });
  });

  socket.on('error', function(err) {
    debug('Socket Error: ', JSON.stringify(err));
  });

  socket.on('timeout', function() {
    debug('Socket Timed out');
  });
});

server.listen(connectPort, function() {
  debug('Server listening: ' + JSON.stringify(server.address()));
  server.on('close', function(){
    debug('Server Terminated');
  });
  server.on('error', function(err){
    debug('Server Error: ', JSON.stringify(err));
  });
});

function receive(socket, data){
  receiver.receiveFromClient(socket, data);
}

function send(socket, data) {
data = makeSendBuffer(BSON.serialize(data));
  var success = !socket.write(data);
  if (!success){ (function(socket, data) {
      	socket.once('drain', function() {
        send(socket, data);
      });
    })(socket, data);
  } 
}

function broadcastAll(message) {
  debug('broadcastAll / msg = ', message);
    message = makeSendBuffer(BSON.serialize(message));
    sockets.forEach(function (socket) {
      socket.write(message);      
    });
}

function broadcastExcludedMe(message, sender) {
  debug('broadcastExcludedMe / msg = ', message);
    message = makeSendBuffer(BSON.serialize(message));
    sockets.forEach(function (socket) {
      if (socket != sender) {
          socket.write(message);          
      }      
    });
}


function makeSendBuffer(msg) {
    var buffMsg = Buffer.from(msg);
    var msgLen = buffMsg.length ;
    var bufPacketLenInfo = Buffer.allocUnsafe(4);
    var headerLen= bufPacketLenInfo.length;
    bufPacketLenInfo.writeUInt32LE(msgLen, 0); 
    
    var bufTotal = Buffer.allocUnsafe(headerLen + msgLen); //packet length info + msg
    bufPacketLenInfo.copy(bufTotal, 0, 0, headerLen);
    buffMsg.copy(bufTotal, headerLen, 0, msgLen );
    //debug(color.yellow('[send] header length = '+headerLen+' / msg length = '+ msgLen+ '/ total length = '+(headerLen + msgLen) +' /  msg = ' + buffMsg.toString())); 
    return bufTotal;
}
