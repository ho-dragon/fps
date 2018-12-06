const connection = require('./connection');
const models = require('./packetModels');
const room = require('./Room');
const debug = require('debug')('gameMain');
const bson = require('bson');

const maxPlayerCount = 20;
const minPlayerCount = 2 
const maxWaitingTime = 20;
const maxPlayTime = 300;
const maxScoreGoal = 10;
const countDownTime = 5;
const successCode = 200;

var currentWaitingTime = 0;
var joinedPlayerCount = 0;
var playTime = 0;

var isGameStarted = false;
var isGameEnd = false;

var scoreTeamRed = 0;
var scoreTeamBlue = 0;
var isWaitingPlayer = false;

module.exports.checkGameStart = checkGameStart;
module.exports.isWaitingGame = isWaitingGame;
module.exports.isRunningGame = isRunningGame;
module.exports.isFull = isFull;
module.exports.addTeamScore = addTeamScore;
module.exports.getScore = getScore;

const waitingTimer = setInterval(() => {
			if (this.isWaitingPlayer == false) {
				debug("[waitingTimer] isWaitingPlayer is false");
				return;
			}
			this.currentWaitingTime++;
			debug("[waitingTimer] currentWaitingTime = " + this.currentWaitingTime);
			if (isRemainWaitingTime() <= 0) {//시간초과
				if (this.minPlayerCount <= playerCount) {//최소 시작 인원 달성
					StartGame();
	   	 			return;
				} else {
					this.currentWaitingTime -= 15; //5초 더 추가
					if (this.currentWaitingTime < 0) {
						this.playTime = 0;
					}
				}
			} else {
				if (playerCount >= this.maxPlayerCount) {//바로 시작
					startGame();
					return;
				}
			}			
			broadcastWaitingStatus();
}, 1000)


const gameTimer = setInterval(() => {
	this.playTime++;
	debug("[gameTimer] playTime = " + this.playTime);
	if (isRemainTimeToEnd() <= 0) {
		clearInterval(this.gameTimer);
		endGame();
		return;
	}
	broadcastUpdateGameTime();
}, 1000);


function checkGameStart(playerCount) {
	this.joinedPlayerCount = playerCount;
	if (this.isWaitingPlayer == false) {
		this.isWaitingPlayer = true;
		waitingTimer();
	}
}

function isFull() {
	return this.joinedPlayerCount == this.maxPlayerCount;
}

function isWaitingGame() {
	return this.isWaitingPlayer;
}

function isRunningGame() {
	return this.isGameStarted && this.isGameEnd == false;
}

function getScore(teamCode) {
	if (teamCode == 1){
		return this.scoreRed;
	} 
	return this.scoreBlue;
}

function startGame() {//Todo.GameStart
	debug("[startGame]");
	if (this.isGameStarted) {
		debug("[startGame] game is already started.");
		return;
	}

	clearInterval(this.waitingTimer);
	this.isWaitingPlayer = false;
 	this.isGameStarted = true;
 	this.isGameEnd = false;
 	room.assignTeam();
 	gameTimer();
 	broadcastStartGame();
}

function endGame() {//Todo. send GameReuslt
	debug("[endGame]");
	this.isGameStarted = false;
	this.isGameEnd = true;
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
	if (this.maxScoreGoal <= scoreRed || this.maxScoreGoal <= scoreBlue) {
		endGame();
	}
}

function isRemainTimeToEnd() {
	return this.maxPlayTime - this.playTime;
}

function isRemainWaitingTime() {
	return this.maxWaitingTime - this.playTime;
}

function broadcastStartGame() {
	let model = new models.gameContext(this.playTime, this.maxPlayTime,  room.getTeamNumbers(), this.scoreRed, this.scoreBlue)
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('startGame', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastWaitingStatus() {
	let model = new models.waitingStatus(this.joinedPlayerCount, this.maxPlayerCount, isRemainWaitingTime());
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('waitingPlayer', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastEndGame() {
	let model = new models.updateScore(this.scoreRed, this.scoreBlue);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('endGame', successCode, "success", bytes);
	connection.broadcastAll(noti);
}

function broadcastUpdateGameTime() {
	let model = new models.gameTime(this.playTime);
	let bytes = bson.serialize(model);
	let noti = new models.notificationFormat('updateGameTime', successCode, "success", bytes);
	connection.broadcastAll(noti);
}