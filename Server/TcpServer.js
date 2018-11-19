/***************************************************************************************
* refernece link :  https://mylko72.gitbooks.io/node-js/content/chapter8/chapter8_3.html
****************************************************************************************/
var connectPort = 8107;
var timeOutDuration = 5000;
var net = require('net');
var util = require('util');
var color = require("colors");
var receiver = require('./TcpReceiver.js');
const Networker = require('./networker');
var BSON = require('bson');
var sockets = [];

module.exports.send = send;
module.exports.broadcastAll = broadcastAll;
module.exports.broadcastExcludedMe = broadcastExcludedMe;

var server = net.createServer(function(socket) {

  let networker = new Networker(socket, (data) => {
  	console.log('[TcpServer] received:', data.toString());
  	console.log('[TcpServer] received from socket / bytesRead = ' + socket.bytesRead + " / data length = " + data.length);
    receive(socket, data);
  });

  networker.init();
  sockets.push(socket);
  
  console.log('Client connection: ');
  console.log('   local = %s:%s', socket.localAddress, socket.localPort);
  console.log('   remote = %s:%s', socket.remoteAddress, socket.remotePort);
  socket.setTimeout(timeOutDuration);
  //socket.setEncoding('utf8');
/*
  socket.on('data', function(data) {
    //console.log('Received test data from socket on port %d: %s', socket.remotePort, data.toString());
    console.log('received from socket / bytesRead = : ' + socket.bytesRead);
    console.log('received from socket / data length = : ' + data.length);
    
    receive(socket, data);
    console.log('  Bytes sent: ' + socket.bytesWritten);
  });
*/
  socket.on('end', function() {
    console.log('Client disconnected');
    server.getConnections(function(err, count){
      console.log('Remaining Connections: ' + count);
    });
  });

  socket.on('error', function(err) {
    console.log('Socket Error: ', JSON.stringify(err));
  });

  socket.on('timeout', function() {
    console.log('Socket Timed out');
  });
});

server.listen(connectPort, function() {
  console.log('Server listening: ' + JSON.stringify(server.address()));
  server.on('close', function(){
    console.log('Server Terminated');
  });
  server.on('error', function(err){
    console.log('Server Error: ', JSON.stringify(err));
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

function broadcastAll(message, isShowLog) {
    message = makeSendBuffer(BSON.serialize(message));
    sockets.forEach(function (socket) {
      socket.write(message);
    });
    
    if(isShowLog) {
          process.stdout.write(message)
    }
}

function broadcastExcludedMe(message, sender, isShowLog) {
    message = makeSendBuffer(BSON.serialize(message));
    sockets.forEach(function (socket) {
      if (socket === sender) {
           return;
      }
      socket.write(message);
    });
    process.stdout.write(message)

    if(isShowLog) {
          process.stdout.write(message)
    }
}


function makeSendBuffer(msg) {
    var buffMsg = new Buffer(msg);
    var msgLen = buffMsg.length ;
    var bufPacketLenInfo = new Buffer(4);
    bufPacketLenInfo.fill();
    var headerLen= bufPacketLenInfo.length;
    bufPacketLenInfo.writeUInt32LE(msgLen, 0); 
    
    var bufTotal = new Buffer(  headerLen + msgLen ); //packet length info + msg
    bufTotal.fill();
    bufPacketLenInfo.copy(bufTotal, 0, 0, headerLen);
    buffMsg.copy(bufTotal, headerLen, 0, msgLen );
    //console.log(color.yellow('[send] header length = '+headerLen+' / msg length = '+ msgLen+ '/ total length = '+(headerLen + msgLen) +' /  msg = ' + buffMsg.toString())); 
    return bufTotal;
}
