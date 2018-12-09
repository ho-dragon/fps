
'use strict';

const debug = require('debug')('room');
const models = require('./packetmodels');
var room = {
	players : []
}
module.exports.addPlayer = addPlayer;
module.exports.getOtherPlayers = getOtherPlayers;
module.exports.updateLastPosition = updateLastPosition;
module.exports.applyDamage = applyDamage;
module.exports.assignTeam = assignTeam;
module.exports.getTeamNumbers = getTeamNumbers;
module.exports.getPlayerByNumber = getPlayerByNumber;
module.exports.getPlayerCount = getPlayerCount;
module.exports.clearRoom = clearRoom;
module.exports.respawn = respawn;

const maxHP = 100;

function addPlayer(isRunningGame, nickName) {
	debug("addPlayer :: isRunningGame = " + isRunningGame);
	if (isExistPlayer(nickName)) {
		debug("isExistPlayer = true / nickName = "+nickName+" :: rejoin!!!!!!!!!!!!!!!!!!!");
		return getPlayerByName(nickName);
	}

	let teamCode = 0;	
	if (isRunningGame) {//assignTemCode
		teamCode = assignTeamInPlaying();		
	}
	var playerNum = room.players.length;
	debug("addPlayer :: playerNumber = " + playerNum);
	var player = new models.player(nickName, playerNum, teamCode, maxHP, maxHP, null, 0, false, 0, 0);
	room.players.push(player);
	debug("added player :: name = " + player.nickName + " / number = " + player.number + " / room.players size = " + room.players.length);
	return player;
}

function clearRoom() {
	room.players = [];
}

function getPlayerCount() {
	if (room.players == null) {
		return 0;
	}
	return room.players.length;
}

function isExistPlayer(nickName) {
	if (room.players == null) {
		return false;
	}

	for (let key in room.players) {
		if (room.players[key].nickName == nickName) {
			return true;
		}
	}
	return false;
}

function getPlayerByName(nickName) {
	for (let key in room.players) {
		if (room.players[key].nickName == nickName) {
			return room.players[key];
		}
	}
	return null;
}

function getPlayerByNumber(playerNum) {
	for (let key in room.players) {
		if (room.players[key].number == playerNum) {
			return room.players[key];
		}
	}
	return null;
}

function assignTeamInPlaying() {
	let redTeamCount = 0;
	let blueTeamCount = 0;
	for (let key in room.players) {
		if (room.players[key].teamCode == 1) {
			redTeamCount++;	
		} else {
			blueTeamCount++;
		}		
	}

	if (redTeamCount == blueTeamCount) {
		return (Math.random() >0.5)? 1 : 2; 
	} else if (redTeamCount > blueTeamCount){
		return 2;
	} else {
		return 1;
	}
}

function assignTeam() {
	let isTeamRed = false;
	for (let key in room.players) {
		isTeamRed = !isTeamRed;
		if (isTeamRed) {
			room.players[key].teamCode = 1;	
		} else {
			room.players[key].teamCode = 2;	
		}		
	}
}

function getTeamNumbers() {
	debug("[getTEamNumbers] room.players size =" + room.players.length);
	let teamNumbers = {};
	for (let key in room.players) {
		teamNumbers[room.players[key].number] = room.players[key].teamCode;
		debug("[getTEamNumbers] teamNumber =" + teamNumbers[room.players[key].number]);
	}
	return teamNumbers;
}

function getOtherPlayers(playerNum) {
	let otherPlayers = [];
	for (let key in room.players) {
		if (room.players[key].number != playerNum) {
			otherPlayers.push(room.players[key]);
		}
	}
	return otherPlayers;
}

function updateLastPosition(playerNum, lastPosition, lastYaw) {
	for (let key in room.players) {
		if (room.players[key].number == playerNum) {
			room.players[key].lastPosition = lastPosition;
			room.players[key].lastYaw = lastYaw;
		}
	}
}

function applyDamage(attckPlayer, damagedPlayer, damage) {
	for( let key in room.players) {
		if (room.players[key].number == damagedPlayer.number) {
			room.players[key].currentHP -= damage;
			if (room.players[key].currentHP <= 0) {
				room.players[key].currentHP = 0;
				room.players[key].isDead = true;
				room.players[key].deadCount++;
				attckPlayer.killCount++;
			}
			return room.players[key];
		}
	}	
	return null;
}

function respawn(playerNum) {
	for (let key in room.players) {
		if (room.players[key].number == playerNum) {
			if (room.players[key].isDead) {
				room.players[key].isDead = false;
				room.players[key].currentHP = maxHP;
				room.players[key].maxHP = maxHP;
				return room.players[key];
			}			
		}
	}
	return null;
}
