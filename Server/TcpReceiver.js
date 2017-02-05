
var server = require('./TcpServer.js');
var room = require('./Room.js');
var BSON = require('bson');
var bson = new BSON();

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
    var buffMsg = new Buffer(msg);
    msg = buffMsg.slice(4, buffMsg.length);// remove header buffer
	console.log('receiveFromClient : data = %s', msg.toString());
	
	var Int32 = BSON.Int32;

    var result = bson.deserialize(msg);

    console.log("** method  = " + result.method);
    console.log("** id = " + result.id);
    //console.log("** time = " + result.time);
    console.log("** code = " + result.code);
    console.log("** msg = " + result.msg);
    console.log("** param = " + result.param);

    for (var key in result.param) {
     	console.log("key : " + key +", value : " + new Int32(result.param[key]));
     }

	switch(result.method) {
		case 'init':
			//server.send(socket, "connected success");
			break;
		case 'enterRoom':
			//enterRoom(socket, msgs);
			break;
		case 'move' :
			movePlayer(msg)
			break;
	}
}

function enterRoom(enterRoom) {
	var playerName = enterRoom.playerName;
	var playerNum = room.addPlayer(playerName);

	//var result = new EnterRoomModel() {
	//	self.playerNum = playerNum;
	//	self.playerName = playerName;
	//};
	
	//server.send(socket, result);
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
