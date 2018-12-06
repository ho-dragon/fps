
const connection = require('./connection');
const models = require('./packetModels');
const room = require('./room');
const bson = require('bson');
const color = require("colors");
const debug = require('debug')('socektResponse');
const gameMain = require('./gameMain')

const _movePlayer = "movePlayer";
const _init = "init";
const _enterRoom = "enterRoom";
const _attackPlayer = "attackPlayer";
const _actionPlayer = "actionPlayer";

const successCode = 200;
const failCode_roomIsPlaying = 401;

module.exports.response = response;

function response(socket, msg) {
    let buffMsg = new Buffer(msg);
    //msg = buffMsg.slice(4, buffMsg.length);// remove header buffer// 불피요 : 기존 2.0.0에서 Bson 4.2.0 업그레이드 이후 Bson에서 알아서 앞에 버퍼 부분을 인식하고 디시리얼라이즈해줌
	//debug(' response : length = %d /  data = %s', msg.length, msg.toString());
    let result = bson.deserialize(msg);

    if (result.method != 'movePlayer') {//movePlayer는 너무 빈번함
		debug("** method  = " + result.method);
	    debug("** id = " + result.id);
    	debug("** code = " + result.code);
    	debug("** msg = " + result.msg);
    	debug("** param = " + result.param);

    	for (let key in result.param) {
			debug(" key : " + key +", value : type = " + getType(result.param[key]));
			let x = result.param[key];
			debug(x);
     	}
    }    

	switch(result.method) {
		case _init:
			init(socket, result);
			break;_attackPlayer
		case _enterRoom:
			enterRoom(socket, result);
			break;
		case _movePlayer:
			movePlayer(socket, result);
			break;
		case _attackPlayer:
			attackPlayer(socket, result);
			break;
		case _actionPlayer:
			actionPlayer(socket, result);
			break;
	}
}

function getType(thing) {
    if (thing == null) { 
    	return "[object Null]";
    }
    return Object.prototype.toString.call(thing);
}
 
function init(socket, result) {
	let snedData = new models.responseFormat(successCode, result.id, "success", null);
	connection.send(socket, snedData);
}

function enterRoom(socket, result) {	
	 if (gameMain.isRunningGame()) {
	 	let response = new models.responseFormat(failCode_roomIsPlaying, result.id, "success", null);
		connection.send(socket, response);
		return;
	 }

	let playerName = result.param["playerName"];
	let player = room.addPlayer(playerName); 
	if (player == null) {
		debug("[enterRoom] added player is null")
		return;
	}

	let model = new models.enterRoom(player, room.getOtherPlayers(player.number));
	if (model.player == null) {
		debug("[enterRoom] model.player is null")
		return;
	} else {
		debug("[enterRoom] model.player.name = " + model.player.name);
	}

	let bytes = bson.serialize(model);
   	debug("[enterRoom] bytes length = " + bytes.length);

	let response = new models.responseFormat(successCode, result.id, "success", bytes);
	connection.send(socket, response);

	let notiResult = new models.notificationFormat('joinPlayer', successCode, "success", bytes);
	connection.broadcastExcludedMe(notiResult, socket);
}

function movePlayer(socket, result) {
	let playerNum = result.param["playerNum"];
	let posX = result.param["x"];
	let posY = result.param["y"];
	let posZ = result.param["z"];
	let yaw = result.param["yaw"];
	let model = new models.playerMove(playerNum
		, posX
		, posY
		, posZ
		, yaw);

	let bytes = bson.serialize(model);
	let notiResult = new models.notificationFormat('movePlayer', successCode, "success", bytes);
	room.updateLastPosition(playerNum, [posX, posY, posZ], yaw);
	connection.broadcastExcludedMe(notiResult, socket);
}


function actionPlayer(socket, result) {
	let playerNum = result.param["playerNum"];
	let actionType = result.param["actionType"];
	let model = new models.playerAction(playerNum, actionType);
	let bytes = bson.serialize(model);
	let notiResult = new models.notificationFormat('actionPlayer', successCode, "success", bytes);
	debug("[actionPlayer] bytes length = " + bytes.length);
	connection.broadcastExcludedMe(notiResult, socket);	
}

function attackPlayer(socket, result) {
	let attackPlayerNumber = result.param["attackPlayer"];
	let damagedPlayerNumber = result.param["damagedPlayer"];
	let attackPosition = result.param["attackPosition"];

	let player = room.attackPlayer(damagedPlayerNumber, attackPosition);

	if (player.isDead) {
		gameMain.addTeamScore(player.teamCode);
	}
	
	debug("[attackPlayerNumber] damaged player name = " + player.name);
	let model = new models.playerDamage(attackPlayerNumber, damagedPlayerNumber, 10, player.currentHP, player.maxHP, player.isDead);
	let bytes = bson.serialize(model);
	let response = new models.responseFormat(successCode, result.id, "success", bytes);
	connection.send(socket, response);
	let notiResult = new models.notificationFormat('damagedPlayer', successCode, "success", bytes);
	connection.broadcastExcludedMe(notiResult, socket);
}

