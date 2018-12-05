// 방입장시 / 누가들어오면 ? 현재 커넥션 개수 클라에게 보냄

// 2명이상 참가시 5초 기다리고 방에 들어오지 않으면 게임 시작
// 팀번호 지정
// 팀킬 스코어 지정

const server = require('./TcpServer');
const packetModel = require('./PacketModel');
const room = require('./Room');
const debug = require('debug')('GameMain');

const maxPlayerCount = 20;
const minPlayerCount = 2 
const maxWaitingTime = 20;
const maxPlayTime = 300;
const maxScoreGoal = 10;
const countDownTime = 5;
const successCode = 200;

var currentWaitingTime = 0;
var currentPlayerCount = 0;
var currentPlayTime = 0;

var isGameStarted = false;
var isGameEnd = false;

var scoreTeamRed = 0;
var scoreTeamBlue = 0;
var isWaitingPlayer = false;

module.exports.checkGameStart = checkGameStart;
module.exports.isWaitingGame = isWaitingGame;
module.exports.isRunningGame = isRunningGame;

const waitingTimer = setInterval(() => {
			if (this.isWaitingPlayer == false) {
				debug("[waitingTimer] isWaitingPlayer is false");
				return;
			}
			this currentWaitingTime++;
			debug("[waitingTimer] currentWaitingTime = " + this currentWaitingTime);
			if (isRemainWaitingTime() <= 0) {//시간초과
				if (this.minPlayerCount <= playerCount) {//최소 시작 인원 달성
					StartGame();
	   	 			return;
				} else {
					this currentWaitingTime -= 15; //5초 더 추가
					if (this currentWaitingTime < 0) {
						this.currentPlayTime = 0;
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
	this.currentPlayTime++;
	debug("[gameTimer] currentPlayTime = " + this.currentPlayTime);
	if (isRemainPlayTime() <= 0) {
		clearInterval(this.gameTimer);
		endGame();
		return;
	}
	broadcastUpdateGameTime();
}, 1000);


function checkGameStart(playerCount) {
	this.currentPlayerCount = playerCount;
	if (this.isWaitingPlayer == false) {
		this.isWaitingPlayer = true;
		waitingTimer();
	}
}

function isWaitingGame() {
	return this.isWaitingPlayer;
}

function isRunningGame() {
	return this.isGameStarted && this.isGameEnd == false;
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
 	room.assignTemaNumber();
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

function isRemainPlayTime() {
	return this.maxPlayTime - this.currentPlayTime;
}

function isRemainWaitingTime() {
	return this.maxWaitingTime - this.currentPlayTime;
}

function broadcastStartGame() {
	let gameContext = packetModel.GameContext(this.isGameStarted, this.scoreRed, this.scoreBlue);
	let model = new packetModel.StartGame(this.currentPlayTime, this.maxPlayTime,  room.getTeamNumbers(), gameContext)
	let bytes = BSON.serialize(model);
	let noti = new packetModel.notificationFormat('startGame', successCode, "success", bytes);
	server.broadcastAll(noti);
}

function broadcastWaitingStatus() {
	let model = new packetModel.waitingStatus(this.currentPlayerCount, this.maxPlayerCount, isRemainWaitingTime());
	let bytes = BSON.serialize(model);
	let noti = new packetModel.notificationFormat('waitingPlayer', successCode, "success", bytes);
	server.broadcastAll(noti);
}

function broadcastEndGame() {
	let model = new packetModel.GameResult(this.scoreRed, this.scoreBlue);
	let bytes = BSON.serialize(model);
	let noti = new packetModel.notificationFormat('endGame', successCode, "success", bytes);
	server.broadcastAll(noti);
}

function broadcastUpdateGameTime() {
	let model = new packetModel.GameTime(this.currentPlayTime);
	let bytes = BSON.serialize(model);
	let noti = new packetModel.notificationFormat('updateGameTime', successCode, "success", bytes);
	server.broadcastAll(noti);
}