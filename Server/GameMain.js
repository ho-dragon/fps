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
const maxWaitingTime = 20f;
const maxPlayTime = 300f;
const countDownTime = 5f;

var currentWaitingTime = 0f;
var currentPlayerCount = 0;
var currentPlayTime = 0f;

var isGameStarted = false;
var isGameEnd = false;

var scoreTeamRed = 0;
var scoreTeamBlue = 0;

module.exports.checkGameStart = checkGameStart;

var isWaiting = false;

const waitingTimer = setInterval(() => {
			if (this.isWaiting == false) {
				return;
			}

			this.currentWaitingTime++;
			debug("[waitingTimer] currentWaitingTime = " + this.currentWaitingTime);			
			if (playerCount >= this.maxPlayerCount) {//바로 시작 
				startGame();
			} else {
				if (this.minPlayerCount <= playerCount) {//최소 시작 인원 달성
	   	 			if (this.currentWaitingTime >= this.maxWaitingTime) {
	   	 				StartGame();
	   	 			}
				}
			}	
}, 1000)


const gameTimer = setInterval(() => {
	this.currentPlayTime++;
	debug("[gameTimer] currentPlayTime = " + this.currentPlayTime);
	if (this.currentPlayTime >= this.maxPlayTime) {
		clearInterval(this.gameTimer);
		endGame();
	}
}, 1000);


function sendWaitingStatus() {//클라이언트에게 대기 정보 전송
	

}


function checkGameStart(playerCount) {
	this.currentPlayerCount = playerCount;
	if (this.isWaiting == false) {
		this.isWaiting = true;
		waitingTimer();
	}
}

function isRunningGame() {
	return this.isGameStarted && this.isGameEnd == false;
}

function startGame() {//Todo.GameStart
	debug("[startGame]");
	this.isWaiting = false;
	clearInterval(this.waitingTimer);
	
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
		scoreTeamRed++;
	} else {
		scoreTeamBlue++;
	}	
}