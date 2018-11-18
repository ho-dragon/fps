
var server = require('./TcpServer.js');
var room = require('./Room.js');
var BSON = require('bson');
var color = require("colors");

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
    var buffMsg = new Buffer(msg);
    msg = buffMsg.slice(4, buffMsg.length);// remove header buffer
	console.log('[TcpReciver] receiveFromClient : length = %d /  data = %s', msg.length, msg.toString());
	
    var result = BSON.deserialize(msg);
    console.log("** method  = " + result.method);
    console.log("** id = " + result.id);
    console.log("** code = " + result.code);
    console.log("** msg = " + result.msg);
    console.log("** param = " + result.param);


    for (var key in result.param) {
		console.log("key : " + key +", value : type = " + get_type(result.param[key]));
		var x = result.param[key];
		console.log(x);
     }

	switch(result.method) {
		case 'init':
			init(socket, result);
			break;
		case 'enterRoom':
			enterRoom(socket, result);
			break;
		case 'movePlayer' :
			movePlayer(socket, msg);
			break;
		case 'attackPlayer' :
			attackPlayer(socket, result);
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
	var player = room.addPlayer(playerName);
	var model = new EnterRoomModel(player.teamCode, player.playerNum, player.playerName, player.currentHP, player.maxHP);
	var bytes = BSON.serialize(model);

	var response = new SocketRequestFormat('', 200, receivedData.id, "success", bytes);
	server.send(socket, response);

	var notiResult = new SocketRequestFormat('joinPlayer',200, 0, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket,  true);
}

function movePlayer(socket, msg) {
	server.broadcastExcludedMe(msg, socket, false);
}

function attackPlayer(socket, receivedData) {
	var attackPlayer = receivedData.param["attackPlayer"];
	var damagedPlayer = receivedData.param["damagedPlayer"];
	var attackPosition = receivedData.param["attackPosition"];

	var player = room.attackPlayer(attackPlayer, damagedPlayer, attackPosition);
	var model = new DamageModel(attackPlayer, damagedPlayer, 10, player.currentHP, player.maxHP);
	var bytes = BSON.serialize(model);

	var response = new SocketRequestFormat('', 200, receivedData.id, "success", bytes);
	server.send(socket, response);

	var notiResult = new SocketRequestFormat('damagedPlayer',200, 0, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket,  true);
}

function MovePlayerModel(playerNum, playerPosX, playerPosY, playerPosZ) {
	this.playerNum = playerNum;
	this.playerPosX = playerPosX;
	this.playerPosY = playerPosY;
	this.playerPosZ = playerPosZ;
}

function DamageModel(attackPlayer, damagedPlayer, damage, currentHP, maxHP) {
	this.attackPlayer = attackPlayer;
	this.damagedPlayer = damagedPlayer;
	this.damage = damage;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
}

function EnterRoomModel(teamCode, playerNum, playerName, currentHP, maxHP) {
	this.teamCdoe = teamCode;
	this.playerNum = playerNum;
	this.playerName = playerName;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
}

function SocketRequestFormat(method, code, id, msg, bytes) {
	this.method = method;
	this.code = code;
	this.id = id;
	this.msg = msg;
	this.bytes = bytes;
}