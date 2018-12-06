
module.exports.playerMove = playerMove;
module.exports.playerDamage = playerDamage;
module.exports.enterRoom = enterRoom;
module.exports.responseFormat = responseFormat;
module.exports.notificationFormat = notificationFormat;
module.exports.player = player;
module.exports.playerAction = playerAction;
module.exports.deadPlayer = deadPlayer;

function playerMove(playerNum, playerPosX, playerPosY, playerPosZ, playerYaw) {
	this.playerNum = playerNum;
	this.playerPosX = playerPosX;
	this.playerPosY = playerPosY;
	this.playerPosZ = playerPosZ;
	this.playerYaw = playerYaw;
}

function playerDamage(attackPlayer, damagedPlayer, damage, currentHP, maxHP, isDead) {
	this.attackPlayer = attackPlayer;
	this.damagedPlayer = damagedPlayer;
	this.damage = damage;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
	this.isDead = isDead;
}

function deadPlayer(killerNumber, deaderNumber, killerKillCount, deaderDeadCount, scoreRed, scoreBlue) {
	this.killerNumber = killerNumber;
	this.deaderNumber = deaderNumber;
	this.killerKillCount = killerKillCount;
	this.deaderDeadCount = deaderDeadCount;
	this.scoreRed = scoreRed;
	this.scoreBlue = scoreBlue;
}

function enterRoom(player, otherPlayers) {
	this.player = player;
	this.otherPlayers = otherPlayers;
}

function responseFormat(code, id, msg, bytes) {
	this.method = "";
	this.code = code;
	this.id = id;
	this.msg = msg;
	this.bytes = bytes;
}

function notificationFormat(method, code, msg, bytes) {
	this.method = method;
	this.code = code;
	this.id = 0;
	this.msg = msg;
	this.bytes = bytes;
}

function player(name, number, teamCode, currentHP, maxHP, lastPosition, lastYaw, isDead, deadCount, killCount) {
	this.name = name;
	this.number = number;
	this.teamCode = teamCode;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
	this.lastPosition = lastPosition;
	this.lastYaw = lastYaw;
	this.isDead = isDead;
	this.deadCount = deadCount;
	this.killCount = killCount;
}

function playerAction(playerNum, actionType) {
	this.playerNum = playerNum;
	this.actionType = actionType;
}

function gameContext(playTime, maxPlayTime, playerTeamNumbers, scoreRed, scoreBlue) {
	this.playTime = playTime;
	this.maxPlayTime = maxPlayTime;
	this.playerTeamNumbers = playerTeamNumbers;
	this.scoreRed = scoreRed;
	this.scoreBlue = scoreBlue;
}

function gameTime(playTime) {
	this.playTime = playTime;
}

function waitingStatus(joinedPlayerCount, maxPlayerCount, remianTimeToPlay) {
	this.joinedPlayerCount = joinedPlayerCount;
	this.maxPlayerCount = maxPlayerCount;
	this.remianTimeToPlay = remianTimeToPlay;
}