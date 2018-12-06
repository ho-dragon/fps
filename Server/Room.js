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

function addPlayer(isRunningGame, playerName) {
	if (isExistPlayer()) {
		return getPlayer(playerName);
	}

	let teamCode = 0;	
	if (isRunningGame) {//assignTemCode
		teamCode = assignTeamInPlaying();		
	}
	var playerNum = room.players.length;
	var player = new models.player(playerName, playerNum, teamCode, 100, 100, null, 0, false, 0, 0);
	room.players.push(player);
	debug("added player :: name = " + player.name + " / number = " + player.number);
	return player;
}

function getPlayerCount() {
	if (room.players == null) {
		return 0;
	}
	return room.players.length;
}

function isExistPlayer(playerName) {
	if (room.players == null) {
		return false;
	}

	for (let key in room.players) {
		if (room.players[key].name == playerName) {
			return true;
		}
	}
	return false;
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
	let teamNumbers = [];
	for (let key in room.players) {
		teamNumbers[room.players[key].number] = room.players[key].teamCode;
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

function applyDamage(damagedPlayer, damage) {
	for(var key in room.players) {
		if (room.players[key].number == damagedPlayer) {
			room.players[key].currentHP -= damage;
			if (room.players[key].currentHP < 0) {
				room.players[key].currentHP = 0;
				room.players[key].isDead = true;
				room.players[key].deadCount++;
			}
			return room.players[key];
		}
	}	
	return null;
}
