
'use strict';

const connection = require('./connection');
const models = require('./packetModels');
const room = require('./room');
const bson = require('bson');
const debug = require('debug')('socektResponse');
const game = require('./gameMain')

const _movePlayer = "movePlayer";
const _init = "init";
const _enterRoom = "enterRoom";
const _attackPlayer = "attackPlayer";
const _actionPlayer = "actionPlayer";
const _joinRuuningGame = "joinRunningGame";

const codeSuccess = 200;
const codeRoomisPlaying = 9001;
const codeRoomisFull = 9002;
const codeNoPlayingGame = 9003;
const codeIsAlreadyDead = 9004;

module.exports.response = response;

function response(socket, msg) { 
    let result = bson.deserialize(msg);
	switch(result.method) {
		case _init:
			init(socket, result);
			break;
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
		case _joinRuuningGame:
			joinRunningGame(socket, result);
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
	let snedData = new models.responseFormat(codeSuccess, result.id, "success", null);
	connection.send(socket, snedData);
}

function enterRoom(socket, result) {
	 if (game.isRunningGame()) {
	 	let response = new models.responseFormat(codeRoomisPlaying, result.id, "success", null);
		connection.send(socket, response);
		return;
	 }

	let playerName = result.param["playerName"];
	let player = room.addPlayer(false, playerName);
	let model = new models.enterRoom(player, room.getOtherPlayers(player.number), null);
	let bytes = bson.serialize(model);
	let response = new models.responseFormat(codeSuccess, result.id, "success", bytes);
	connection.send(socket, response);

	let notiResult = new models.notificationFormat('joinPlayer', codeSuccess, "success", bytes);
	connection.broadcastExcludedMe(notiResult, socket);
	game.checkGameStart(room.getPlayerCount());
}

function joinRunningGame(socket, result) {
	if (game.isRunningGame() == false) {
		let response = new models.responseFormat(codeRoomisPlaying, result.id, "success", null);
		connection.send(socket, response);
		return;
	}

	if (game.isFull()) {
		let response = new models.responseFormat(codeRoomisFull, result.id, "success", null);
		connection.send(socket, response);
		return;
	}

	let playerName = result.param["playerName"];
	let player = room.addPlayer(true, playerName);
	let runningGameContext = game.getRunningGameContext();
	let model = new models.enterRoom(player, room.getOtherPlayers(player.number), runningGameContext);	
	let bytes = bson.serialize(model);
	let response = new models.responseFormat(codeSuccess, result.id, "success", bytes);
	connection.send(socket, response);

	let notiResult = new models.notificationFormat('joinPlayer', codeSuccess, "success", bytes);
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
	let notiResult = new models.notificationFormat('movePlayer', codeSuccess, "success", bytes);
	room.updateLastPosition(playerNum, [posX, posY, posZ], yaw);
	connection.broadcastExcludedMe(notiResult, socket);
}

function actionPlayer(socket, result) {
	let playerNum = result.param["playerNum"];
	let actionType = result.param["actionType"];
	let model = new models.playerAction(playerNum, actionType);
	let bytes = bson.serialize(model);
	let notiResult = new models.notificationFormat('actionPlayer', codeSuccess, "success", bytes);
	connection.broadcastExcludedMe(notiResult, socket);	
}

function attackPlayer(socket, result) {
	if (game.isRunningGame() == false) {
		debug("[broadcastRespawnPlayer] game end.");
		return;
	}

	let attackPlayerNumber = result.param["attackPlayer"];
	let damagedPlayerNumber = result.param["damagedPlayer"];
	let attackPosition = result.param["attackPosition"];
	let attakPlayer = room.getPlayerByNumber(attackPlayerNumber);
	let targetPlayer = room.getPlayerByNumber(damagedPlayerNumber);

	if (targetPlayer.isDead) {
		let response = new models.responseFormat(codeIsAlreadyDead, result.id, "success", null);
		connection.send(socket, response);
		return;
	}

	let damage = 0;
	if (attackPlayer.teamCode != targetPlayer.teamCode) {
		damage = 20;
	}

	let damagedPlayer = room.applyDamage(attakPlayer, targetPlayer, damage);
	let damageModel = new models.playerDamage(attackPlayerNumber, damagedPlayerNumber, damage, damagedPlayer.currentHP, damagedPlayer.maxHP, damagedPlayer.isDead);
	let bytes = bson.serialize(damageModel);
	let response = new models.responseFormat(codeSuccess, result.id, "success", bytes);
	connection.send(socket, response);
	
	if (damagedPlayer.isDead) {		
		game.addTeamScore(attakPlayer.teamCode);
		let model = new models.deadPlayer(damageModel, attackPlayer.killCount, damagedPlayer.deadCount, game.getScore(1), game.getScore(2));
		let bytes = bson.serialize(model);
		let notiResult = new models.notificationFormat('deadPlayer', codeSuccess, "success", bytes);
		connection.broadcastAll(notiResult, socket);
		setTimeout(broadcastRespawnPlayer, 5000, damagedPlayer.number);
	} else {
		let notiResult = new models.notificationFormat('damagedPlayer', codeSuccess, "success", bytes);
		connection.broadcastExcludedMe(notiResult, socket);
	}
}

function broadcastRespawnPlayer(playerNumber) {
	if (game.isRunningGame() == false) {
		debug("[broadcastRespawnPlayer] game end.");
		return;
	}
	let respawnPlayer = room.respawn(playerNumber);
	let model = new models.respawnModel(playerNumber, respawnPlayer.currentHP, respawnPlayer.maxHP);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('respawn', codeSuccess, "success", bytes);
	connection.broadcastAll(noti);
}