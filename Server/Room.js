var room = {
	players : []
}

module.exports.addPlayer = addPlayer;

function addPlayer(playerName) {
	var player = {
		name : playerName,
		number : 0,
	}
	room.players.push(player);
	player.number = room.players.length;
	return player.number;
}