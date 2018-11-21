
var server = require('./TcpServer.js');
var room = require('./Room.js');
var BSON = require('bson');
var color = require("colors");
const debug = require('debug')('TcpRecevier');

module.exports.receiveFromClient = receiveFromClient;

function receiveFromClient(socket, msg) {
    let buffMsg = new Buffer(msg);
    //msg = buffMsg.slice(4, buffMsg.length);// remove header buffer// 불피요 : 기존 2.0.0에서 Bson 4.2.0 업그레이드 이후 Bson에서 알아서 앞에 버퍼 부분을 인식하고 디시리얼라이즈해줌

	debug(' receiveFromClient : length = %d /  data = %s', msg.length, msg.toString());
    let result = BSON.deserialize(msg);

    debug("** method  = " + result.method);
    debug("** id = " + result.id);
    debug("** code = " + result.code);
    debug("** msg = " + result.msg);
    debug("** param = " + result.param);

    for (let key in result.param) {
		debug(" key : " + key +", value : type = " + get_type(result.param[key]));
		let x = result.param[key];
		debug(x);
     }

	switch(result.method) {
		case 'init':
			init(socket, result);
			break;
		case 'enterRoom':
			enterRoom(socket, result);
			break;
		case 'movePlayer' :
			movePlayer(socket, result);
			break;
		case 'attackPlayer' :
			attackPlayer(socket, result);
			break;
	}
}

function get_type(thing) {
    if (thing===null) { 
    	return "[object Null]";
    }
    return Object.prototype.toString.call(thing);
}
 
function init(socket, receivedData) {
	let snedData = new ResponseFormat(200, receivedData.id, "success", null);
	server.send(socket, snedData);
}

function enterRoom(socket, receivedData) {
	let playerName = receivedData.param["playerName"];
	let player = room.addPlayer(playerName);
 
	if (player == null) {
		debug("[enterRoom] added player is null")
		return;
	}

	let model = new EnterRoomModel(player, room.getOtherPlayers(player.number));

	if (model.player == null) {
		debug("[enterRoom] model.player is null")
		return;
	} else {
		debug("[enterRoom] model.player.name = " + model.player.name);
	}

	let bytes = BSON.serialize(model);

   debug("[enterRoom] bytes length = " + bytes.length);

	let response = new ResponseFormat(200, receivedData.id, "success", bytes);
	server.send(socket, response);

	let notiResult = new NotificationFormat('joinPlayer',200, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket);
}

function movePlayer(socket, receivedData) {
	let playerNum = receivedData.param["playerNum"];
	let posX = receivedData.param["playerPosX"];
	let posY = receivedData.param["playerPosY"];
	let posZ = receivedData.param["playerPosZ"];
	let model = new PlayerMoveModel(playerNum
		, posX
		, posY
		, posZ);

	let bytes = BSON.serialize(model);
	let notiResult = new NotificationFormat('movePlayer', 200, "success", bytes);
	room.updateLastPosition(playerNum, {posX, posY, posZ});
	server.broadcastExcludedMe(notiResult, socket);

}

function attackPlayer(socket, receivedData) {
	let attackPlayer = receivedData.param["attackPlayer"];
	let damagedPlayer = receivedData.param["damagedPlayer"];
	let attackPosition = receivedData.param["attackPosition"];

	let player = room.attackPlayer(attackPlayer, damagedPlayer, attackPosition);
	let model = new DamageModel(attackPlayer, damagedPlayer, 10, player.currentHP, player.maxHP);
	let bytes = BSON.serialize(model);

	let response = new ResponseFormat(200, receivedData.id, "success", bytes);
	server.send(socket, response);

	let notiResult = new NotificationFormat('damagedPlayer', 200, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket);
}

function PlayerMoveModel(playerNum, playerPosX, playerPosY, playerPosZ) {
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

function EnterRoomModel(player, otherPlayers) {
	this.player = player;
	this.otherPlayers = otherPlayers;
}

function ResponseFormat(code, id, msg, bytes) {
	this.method = "";
	this.code = code;
	this.id = id;
	this.msg = msg;
	this.bytes = bytes;
}

function NotificationFormat(method, code, msg, bytes) {
	this.method = method;
	this.code = code;
	this.id = 0;
	this.msg = msg;
	this.bytes = bytes;
}