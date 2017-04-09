
var server = require('./TcpServer.js');
var room = require('./Room.js');
var BSON = require('bson');
var color = require("colors");
var bson = new BSON();

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
    var buffMsg = new Buffer(msg);
    msg = buffMsg.slice(4, buffMsg.length);// remove header buffer
	console.log('receiveFromClient : index = %d /  data = %s', msg.length, msg.toString());
    var result = bson.deserialize(msg);
    console.log("** method  = " + result.method);
    console.log("** id = " + result.id);
    console.log("** code = " + result.code);
    console.log("** msg = " + result.msg);
    console.log("** param = " + result.param);

/*
    for (var key in result.param) {
		console.log("key : " + key +", value : type = " + get_type(result.param[key]));
		var x = result.param[key];
		console.log(x);
     }*/

	switch(result.method) {
		case 'init':
			init(socket, result);
			break;
		case 'enterRoom':
			enterRoom(socket, result);
			break;
		case 'move' :
			//movePlayer(msg)
			break;
	}
}

function get_type(thing){
    if(thing===null)return "[object Null]"; // special case
    return Object.prototype.toString.call(thing);
}
function init(socket, receivedData){
	var snedData = new SocketRequestFormat('',200, receivedData.id, "success", null);
	server.send(socket, snedData);
}

function enterRoom(socket, receivedData) {
	var playerName = receivedData.param["playerName"];
	var playerNum = room.addPlayer(playerName);

	var model = new EnterRoomModel(playerNum, playerName);
	var response = new SocketRequestFormat('', 200, receivedData.id, "success", bson.serialize(model));
	server.send(socket, response);

	//var notiResult = new SocketRequestFormat('joinPlayer',200, 0, "success",null);
	//server.broadcastExcludedMe(notiResult, socket,  true);
}

function movePlayer(msg) {
	server.broadcastAll(msg, false);
}

function EnterRoomModel(playerNum, playerName){
	this.playerNum = playerNum;
	this.playerName = playerName;
}

function SocketRequestFormat(method, code, id, msg, bytes) {
	this.method = method;
	this.code = code;
	this.id = id;
	this.msg = msg;
	this.bytes = bytes;
}