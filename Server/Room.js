const debug = require('debug')('Room');
const model = require('./PacketModel');
var room = {
	players : []
}

module.exports.addPlayer = addPlayer;
module.exports.getOtherPlayers = getOtherPlayers;
module.exports.updateLastPosition = updateLastPosition;
module.exports.attackPlayer = attackPlayer;
module.exports.assingTemaNumber = assingTemaNumber;
module.exports.getTeamNumbers = getTeamNumbers;

function addPlayer(playerName) {
	if (isExistPlayer()) {
		return getPlayer(playerName);
	}
	var playerNum = room.players.length;
	var player = new model.player(playerName, playerNum, -1, 100, 100, null, 0, false, 0);
	room.players.push(player);
	debug("added player :: name = " + player.name + " / number = " + player.number);
	return player;
}

function isExistPlayer(playerName) {
	if (room == null) {
		return false;
	}

	for (let key in room.players) {
		if (room.players[key].name == playerName) {
			return true;
		}
	}
	return false;
}

function getPlayer(playerName) {
	for (let key in room.players) {
		if (room.players[key].name == playerName) {
			return room.players[key];
		}
	}
	return null;
}

function getPlayer(playerNum) {
	for (let key in room.players) {
		if (room.players[key].number == playerNum) {
			return room.players[key];
		}
	}
	return null;
}

function assignTemaNumber() {
	let isTeamRed = false;
	for (let key in room.players) {
		isTeamRed = !isTeamRed;
		if (isTeamRed) {
			room.players[key].teamCode = 1;	
		} else {
			room.players[key].teamCode = 2;	
		}		
	}
	return teamNumbers;
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

function attackPlayer(damagedPlayer, attackPosition) {
	for(var key in room.players) {
		if (room.players[key].number == damagedPlayer) {
			room.players[key].currentHP -= 10;
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
