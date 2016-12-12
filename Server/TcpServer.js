/***************************************************************************************
* refernece link :  https://mylko72.gitbooks.io/node-js/content/chapter8/chapter8_3.html
****************************************************************************************/
var connectPort = 8107;
var timeOutDuration = 5000;
var net = require('net');
var color = require("colors");
var receiver = require('./TcpReceiver.js');
var clients = [];

module.exports.send = send;
module.exports.broadcastAll = broadcastAll;
module.exports.broadcastExcludedMe = broadcastExcludedMe;

var server = net.createServer(function(client) {
  clients.push(client);

  console.log('Client connection: ');
  console.log('   local = %s:%s', client.localAddress, client.localPort);
  console.log('   remote = %s:%s', client.remoteAddress, client.remotePort);
  client.setTimeout(timeOutDuration);
  client.setEncoding('utf8');

  client.on('data', function(data) {
    console.log('Received test data from client on port %d: %s', client.remotePort, data.toString());
    console.log('Bytes received: ' + client.bytesRead);
    receive(client, data);
    console.log('  Bytes sent: ' + client.bytesWritten);
  });

  client.on('end', function() {
    console.log('Client disconnected');
    server.getConnections(function(err, count){
      console.log('Remaining Connections: ' + count);
    });
  });

  client.on('error', function(err) {
    console.log('Socket Error: ', JSON.stringify(err));
  });

  client.on('timeout', function() {
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
console.log(color.yellow('[send]') + '%s',data.toString());	
  var success = !socket.write(data);
  if (!success){ (function(socket, data) {
      	socket.once('drain', function() {
        send(socket, data);
      });
    })(socket, data);
  } 
}

function broadcastAll(message) {
    clients.forEach(function (client) {
      client.write(message);
    });
    process.stdout.write(message)
}

function broadcastExcludedMe(message, sender) {
    clients.forEach(function (client) {
      if (client === sender) {
           return;
      }
      client.write(message);
    });
    process.stdout.write(message)
}