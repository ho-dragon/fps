
var server = require('./TcpServer.js');
var room = require('./Room.js');
var BSON = require('bson');

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
    var buffMsg = new Buffer(msg);
    msg = buffMsg.slice(4, buffMsg.length);// remove header buffer
	console.log('receiveFromClient : data = %s', msg.toString());
	
	

	var msgs = msg.toString().split('|');
	switch(msgs[0]) {
		case 'init':
			//server.send(socket, "connected success");
			break;
		case 'enterRoom':
			enterRoom(socket, msgs);
			break;
		case 'move' :
			movePlayer(msg)
			break;
	}
}

function enterRoom(enterRoom) {
	var playerName = enterRoom.playerName;
	var playerNum = room.addPlayer(playerName);

	var result = new EnterRoomModel() {
		self.playerNum = playerNum;
		self.playerName = playerName;
	};
	
	server.send(socket, result);
	//server.broadcastExcludedMe('joinPlayer' + '|' + playerNum + '|' + playerName, socket,  true);
}

/*
function enterRoom(socket, msg) {
	var playerName = msg[1];
	var playerNum = room.addPlayer(playerName);
	server.send(socket, 'enterRoomResult' + '|' + playerNum + '|' + playerName);
	server.broadcastExcludedMe('joinPlayer' + '|' + playerNum + '|' + playerName, socket,  true);
}*/

function movePlayer(msg) {
	server.broadcastAll(msg, false);
}

var initResult = {
	msg : '',
}

var enterRoom = {
	playerName : '',
}

var EnterRoomModel = {
	playerNum : 0,
	playerName : '',
}

var SocketRequestFormat = {
	method : '',
	id : 0,
	time : 0,
	param : 
}

var doc = { long: Long.fromNumber(100) }