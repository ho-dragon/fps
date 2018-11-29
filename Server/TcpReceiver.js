
var server = require('./TcpServer.js');
var model = require('./PacketModel.js');
var room = require('./Room.js');
var BSON = require('bson');
var color = require("colors");
const debug = require('debug')('TcpRecevier');

module.exports.receiveFromClient = receiveFromClient;


const methodMovePlayer = "movePlayer";
const methodInit = "init";
const methodEnterRoom = "enterRoom";
const methodAttackPlayer = "attackPlayer";
const methodactionPlayer = "actionPlayer";

function receiveFromClient(socket, msg) {
    let buffMsg = new Buffer(msg);
    //msg = buffMsg.slice(4, buffMsg.length);// remove header buffer// 불피요 : 기존 2.0.0에서 Bson 4.2.0 업그레이드 이후 Bson에서 알아서 앞에 버퍼 부분을 인식하고 디시리얼라이즈해줌
	//debug(' receiveFromClient : length = %d /  data = %s', msg.length, msg.toString());
    let result = BSON.deserialize(msg);

    if (result.method != 'movePlayer') {//movePlayer는 너무 빈번함
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
    }    

	switch(result.method) {
		case methodInit:
			init(socket, result);
			break;
		case methodEnterRoom:
			enterRoom(socket, result);
			break;
		case methodMovePlayer:
			movePlayer(socket, result);
			break;
		case methodAttackPlayer:
			attackPlayer(socket, result);
			break;
		case methodactionPlayer:

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
	let snedData = new model.responseFormat(200, receivedData.id, "success", null);
	server.send(socket, snedData);
}

function enterRoom(socket, receivedData) {	
	let playerName = receivedData.param["playerName"];
	let player = room.addPlayer(playerName);
 
	if (player == null) {
		debug("[enterRoom] added player is null")
		return;
	}

	let model = new model.enterRoom(player, room.getOtherPlayers(player.number));

	if (model.player == null) {
		debug("[enterRoom] model.player is null")
		return;
	} else {
		debug("[enterRoom] model.player.name = " + model.player.name);
	}

	let bytes = BSON.serialize(model);

   debug("[enterRoom] bytes length = " + bytes.length);

	let response = new model.responseFormat(200, receivedData.id, "success", bytes);
	server.send(socket, response);

	let notiResult = new model.notificationFormat('joinPlayer', 200, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket);
}

function movePlayer(socket, receivedData) {
	let playerNum = receivedData.param["playerNum"];
	let posX = receivedData.param["x"];
	let posY = receivedData.param["y"];
	let posZ = receivedData.param["z"];
	let yaw = receivedData.param["yaw"];
	let model = new model.playerMove(playerNum
		, posX
		, posY
		, posZ
		, yaw);

	let bytes = BSON.serialize(model);
	let notiResult = new model.notificationFormat('movePlayer', 200, "success", bytes);
	room.updateLastPosition(playerNum, [posX, posY, posZ, yaw]);
	server.broadcastExcludedMe(notiResult, socket);
}


function actionPlayer(socket, receivedData) {
	let playerNum = receivedData.param["playerNum"];
	let actionType = receivedData.param["actionType"];
	let model = new model.playerAction(playerNum, actionType);
	let bytes = BSON.serialize(model);
	let notiResult = new model.notificationFormat('actionPlayer', 200, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket);
}

function attackPlayer(socket, receivedData) {
	let attackPlayer = receivedData.param["attackPlayer"];
	let damagedPlayer = receivedData.param["damagedPlayer"];
	let attackPosition = receivedData.param["attackPosition"];

	let player = room.attackPlayer(attackPlayer, damagedPlayer, attackPosition);
	debug("[attackPlayer] damaged player name = " + player.number);

	let model = new model.playerDamage(attackPlayer, damagedPlayer, 10, player.currentHP, player.maxHP);
	let bytes = BSON.serialize(model);

	let response = new model.responseFormat(200, receivedData.id, "success", bytes);
	server.send(socket, response);

	let notiResult = new model.notificationFormat('damagedPlayer', 200, "success", bytes);
	server.broadcastExcludedMe(notiResult, socket);
}

