var room = {
	players : []
}

module.exports.addPlayer = addPlayer;

function addPlayer(playerName) {
	var playerNum = room.players.length;
	var player = new Player(playerName, playerNum, getTeamCode(playerNum), 100, 100);
	room.players.push(player);
	return player;
}

function getTeamCode(playerNum) {
	if(playerNum % 2 == 0) {
		return 1;
	}
	return 2;
}

function attackPlayer(attackPlayer, damagedPlayer, attackPosition) {
	var player;
	for(var i in room.players) {
		if(i.playerNum == damagedPlayer) {
			i.currentHP -= 10;
			player = i;
		}
	}
	return player;
}

function Player(name, number, teamCode, currentHP, maxHP){
	this.name = name;
	this.number = number;
	this.teamCode = teamCode;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
}