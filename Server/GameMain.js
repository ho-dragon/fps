'use strict';

const connection = require('./connection');
const models = require('./packetModels');
const room = require('./room');
const debug = require('debug')('gameMain');
const bson = require('bson');

const maxPlayerCount = 20;
const minPlayerCount = 2 
const maxWaitingTime = 5;
const maxPlayTime = 300;
const maxScoreGoal = 10;
const successCode = 200;

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
	debug("[intervalWaitingTimer] isWaiting = " + isWaiting);
	if (isWaiting == false) {
		debug("[waitingTimer] isWaiting is false");
		clearInterval(waitingTimer);
		return;
	}

	waitingTime++;
	debug("[waitingTimer] waitingTime = " + waitingTime);
	if (isRemainWaitingTime() <= 0) {//시간초과
		if (minPlayerCount <= joinedPlayerCount) {//최소 시작 인원 달성
			startGame();
			return;
		} else {
			waitingTime -= 5; //5초 더 추가
			if (waitingTime < 0) {
				waitingTime = 0;
			}
		}
	} else {
		if (joinedPlayerCount >= maxPlayerCount) {//바로 시작
			startGame();
			return;
		}
	}			
	broadcastWaitingStatus();
}

function intervalGameTimer() {
	playTime++;
	broadcastUpdateGameTime();
	debug("[gameTimer] playTime = " + playTime);
	if (isRemainTimeToEnd() <= 0) {
		endGame();
	}
}


function checkGameStart(playerCount) {
	joinedPlayerCount = playerCount;
	debug("[checkGameStart] joinedPlayerCount = " + joinedPlayerCount + "/  isWaiting = "+ isWaiting);
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

function startGame() {//Todo.GameStart
	if (isGameStarted) {
		debug("[startGame] game is already started.");
		return;
	}
	debug("[startGame]");
	clearInterval(waitingTimer);
	gameTimer = setInterval(intervalGameTimer, 1000);
	isWaiting = false;
 	isGameStarted = true;
 	isGameEnd = false;
 	room.assignTeam();
 	broadcastStartGame();
}

function endGame() {//Todo. send GameReuslt
	clearInterval(gameTimer);
	debug("[endGame]");
	isGameStarted = false;
	isGameEnd = true;
	waitingTimer();
	broadcastEndGame();
}

function addTeamScore(killerTeam) {
	if (killerTeam == 1) {
		scoreTeamRed++;
	} else {
		scoreTeamBlue++;
	}
	checkGameEnd(scoreRed, scoreBlue);
}

function checkGameEnd(scoreRed, scoreBlue) {
	if (maxScoreGoal <= scoreRed || maxScoreGoal <= scoreBlue) {
		endGame();
	}
}

function isRemainTimeToEnd() {
	return maxPlayTime - playTime;
}

function isRemainWaitingTime() {
	return maxWaitingTime - waitingTime;
}

function getRunningGameContext() {
	return new models.gameContext(playTime, maxPlayTime, room.getTeamNumbers(), scoreRed, scoreBlue, maxScoreGoal);
}

function broadcastStartGame() {
	debug("[broadcastStartGame]");
	let model = getRunningGameContext();
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('startGame', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastWaitingStatus() {
	debug("[broadcastWaitingStatus]");
	let model = new models.waitingStatus(joinedPlayerCount, maxPlayerCount, isRemainWaitingTime());
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('waitingPlayer', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastEndGame() {
	debug("[broadcastEndGame]");
	let model = new models.updateScore(scoreRed, scoreBlue);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('endGame', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastUpdateGameTime() {
	debug("[broadcastUpdateGameTime]");
	let model = new models.gameTime(playTime);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('updateGameTime', successCode, "success", bytes);
	connection.broadcastAll(noti);
}