// 방입장시 / 누가들어오면 ? 현재 커넥션 개수 클라에게 보냄

// 2명이상 참가시 5초 기다리고 방에 들어오지 않으면 게임 시작
// 팀번호 지정
// 팀킬 스코어 지정

const server = require('./TcpServer');
const packetModel = require('./PacketModel');
const room = require('./Room');
const debug = require('debug')('GameMain');
const startPlayerMinmum = 2;
const waitingPlayerTime = 5f;
const maxGameTime = 180f;
var currentWaitingTime = 0f;
var currentPlayerCount = 0;
var currentGameTime = 0f;
var isChangedPlayerCount = false;
var isGameStarted = false;
var isGameEnd = false;

var scoreTeam1 = 0;
var scoreTeam2 = 0;

module.exports.checkGameStart = checkGameStart;
const waitingPlayerInterver = setInterval(() => {
			this.currentWaitingTime++;
			debug("[waitingPlayerInterver] currentWaitingTime = " + this.currentWaitingTime);
			if (this.currentWaitingTime >= this.waitingPlayerTime) {
				clearInterval(this.waitingPlayerInterver);
				startGame();
			}
		}, 1000)


const gameTimer = setInterval(() => {
	this.currentGameTime++;
	debug("[gameTimer] currentGameTime = " + this.currentGameTime);
	if (this.currentGameTime >= this.maxGameTime) {
		clearInterval(this.gameTimer);
		endGame();
	}
}, 1000);

function checkGameStart(playerCount) {	
	if (startPlayerMinmum <= playerCount) {
		if (this.currentPlayerCount != playerCount) {
	    	this.currentPlayerCount = playerCount;
	   	 	this.currentWaitingTime = 0f;
	    	this.isChangedPlayerCount = true;
	    	clearInterval(this.waitingPlayerInterver);
	    	waitingPlayerInterver();		   		
		}
	}
}

function isRunningGame() {
	return this.isGameStarted && this.isGameEnd == false;
}

function startGame() {//Todo.GameStart
	debug("[startGame]");
	if (this.isGameStarted) {
		return;
	}
 	this.isGameStarted = true;
 	this.isGameEnd = false;
 	room.AssignTemaNumber();
 	gameTimer();
}

function endGame() {//Todo. send GameReuslt
	debug("[endGame]");
	this.isGameStarted = false;
	this.isGameEnd = true;
}

function addTeamScore(killerTeam) {
	if (killerTeam == 1) {
		scoreTeam1++;
	} else {
		scoreTeam2++;
	}	
}