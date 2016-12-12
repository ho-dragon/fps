
var server = require('./TcpServer.js');
var room = require('./Room.js');

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
	var msgs = msg.split('|');
	switch(msgs[0]) {
		case 'init':
		server.send(socket, "connected success");
		break;
		case 'enterRoom':
		enterRoom(socket,msgs);
		break;
	}
}

function enterRoom(socket, msg) {
	var playerName = msg[1];
	var playerNum = room.addPlayer(playerName);
	server.broadcastAll('joinPlayer' + '|' + playerNum + '|' + playerName);
}