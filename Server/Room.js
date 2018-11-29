const debug = require('debug')('Room');
var model = require('./PacketModel.js');
var room = {
	players : []
}

module.exports.addPlayer = addPlayer;
module.exports.getOtherPlayers = getOtherPlayers;
module.exports.updateLastPosition = updateLastPosition;
module.exports.attackPlayer = attackPlayer;

function addPlayer(playerName) {
	var playerNum = room.players.length;
	var player = new model.player(playerName, playerNum, getTeamCode(playerNum), 100, 100, null);
	room.players.push(player);
	debug("added player :: name = " + player.name + " / number = " + player.number);
	return player;
}

function getTeamCode(playerNum) {
	if(playerNum % 2 == 0) {
		return 1;
	}
	return 2;
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

function updateLastPosition(playerNum, lastPosition) {
	for (let key in room.players) {
		if (room.players[key].number == playerNum) {
			room.players[key].lastPosition = lastPosition;
		}
	}
}

function attackPlayer(attackPlayer, damagedPlayer, attackPosition) {
	for(var key in room.players) {
		if (room.players[key].number == damagedPlayer) {
			room.players[key].currentHP -= 10;
			if (room.players[key].currentHP < 0) {
				room.players[key].currentHP = 0;
			}
			return room.players[key];
		}
	}	
	return null;
}
