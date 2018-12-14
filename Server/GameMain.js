'use strict';

const connection = require('./connection');
const models = require('./packetModels');
const room = require('./room');
const debug = require('debug')('gameMain');
const bson = require('bson');

const maxPlayerCount = 20;
const minPlayerCount = 2 
const maxWaitingTime = 10;
const maxPlayTime = 360;
const maxScoreGoal = 10;
const codeSuccess = 200;

var waitingTime = 0;
var joinedPlayerCount = 0;
var playTime = 0;

var isGameStarted = false;
var isGameEnd = false;

var scoreRed = 0;
var scoreBlue = 0;
var isWaiting = false;

var waitingTimer;
var gameTimer;

module.exports.checkGameStart = checkGameStart;
module.exports.isWaitingGame = isWaitingGame;
module.exports.isRunningGame = isRunningGame;
module.exports.isFull = isFull;
module.exports.addTeamScore = addTeamScore;
module.exports.getScore = getScore;
module.exports.getRunningGameContext = getRunningGameContext;

function intervalWaitingTimer() {
	if (isWaiting == false) {
		clearInterval(waitingTimer);
		return;
	}

	waitingTime++;
	if (isRemainWaitingTime() == false) {
		if (minPlayerCount <= joinedPlayerCount) {
			startGame();
			return;
		} else {
			waitingTime -= 5;
			if (waitingTime < 0) {
				waitingTime = 0;
			}
		}
	} else {
		if (joinedPlayerCount >= maxPlayerCount) {
			startGame();
			return;
		}
	}			
	broadcastWaitingStatus();
}

function intervalGameTimer() {
	playTime++;
	broadcastUpdateGameTime();
	if (isRemainTimeToEnd() == false) {
		endGame();
	}
}

function checkGameStart(playerCount) {
	joinedPlayerCount = playerCount;
	if (isWaiting == false) {
		isWaiting = true;
		waitingTimer =  setInterval(intervalWaitingTimer, 1000);
	}
}

function isFull() {
	return joinedPlayerCount == maxPlayerCount;
}

function isWaitingGame() {
	return isWaiting;
}

function isRunningGame() {
	return isGameStarted && isGameEnd == false;
}

function getScore(teamCode) {
	if (teamCode == 1){
		return scoreRed;
	} 
	return scoreBlue;
}

function startGame() {
	if (isGameStarted) {
		debug("[startGame] game is already started.");
		return;
	}
	clearInterval(waitingTimer);
	gameTimer = setInterval(intervalGameTimer, 1000);
	scoreRed = 0;
	scoreBlue = 0;
	playTime = 0;
	isWaiting = false;
 	isGameStarted = true;
 	isGameEnd = false;
 	room.assignTeam();
 	broadcastStartGame();
}

function endGame() {
	clearInterval(gameTimer);
	isGameStarted = false;
	isGameEnd = true;
	isWaiting = false;
	waitingTime = 0;
	joinedPlayerCount = 0;	
	broadcastEndGame();
	room.clearRoom();
}

function addTeamScore(killerTeam) {
	if (killerTeam == 1) {
		scoreRed++;
	} else {
		scoreBlue++;
	}
	checkGameEnd(scoreRed, scoreBlue);
}

function checkGameEnd(scoreRed, scoreBlue) {
	if (maxScoreGoal <= scoreRed || maxScoreGoal <= scoreBlue) {
		endGame();
	}
}

function isRemainTimeToEnd() {
	return (maxPlayTime - playTime) > 0;
}

function isRemainWaitingTime() {
	return (maxWaitingTime - waitingTime) > 0;
}

function getRunningGameContext() {
	return new models.gameContext(maxPlayTime - playTime, room.getTeamNumbers(), scoreRed, scoreBlue, maxScoreGoal);
}

function broadcastStartGame() {
	let model = getRunningGameContext();
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('startGame', codeSuccess, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastWaitingStatus() {
	let model = new models.waitingStatus(joinedPlayerCount, maxPlayerCount, maxWaitingTime - waitingTime);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('waitingPlayer', codeSuccess, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastEndGame() {
	let model = getRunningGameContext();
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('endGame', codeSuccess, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastUpdateGameTime() {
	let model = new models.gameTime(maxPlayTime - playTime);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('updateGameTime', codeSuccess, "success", bytes);
	connection.broadcastAll(noti);
}