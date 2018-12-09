
module.exports.playerMove = playerMove;
module.exports.playerDamage = playerDamage;
module.exports.enterRoom = enterRoom;
module.exports.responseFormat = responseFormat;
module.exports.notificationFormat = notificationFormat;
module.exports.player = player;
module.exports.playerAction = playerAction;
module.exports.deadPlayer = deadPlayer;
module.exports.gameContext = gameContext;
module.exports.waitingStatus = waitingStatus;
module.exports.gameTime  = gameTime;
module.exports.respawnModel  = respawnModel;

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

function deadPlayer(lastDamageInfo, killerKillCount, deaderDeadCount, scoreRed, scoreBlue) {
	this.lastDamageInfo = lastDamageInfo;
	this.killerKillCount = killerKillCount;
	this.deaderDeadCount = deaderDeadCount;
	this.scoreRed = scoreRed;
	this.scoreBlue = scoreBlue;
}

function enterRoom(player, otherPlayers, runningGameContext) {
	this.player = player;
	this.otherPlayers = otherPlayers;
	this.runningGameContext = runningGameContext;
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

function player(nickName, number, teamCode, currentHP, maxHP, lastPosition, lastYaw, isDead, deadCount, killCount) {
	this.nickName = nickName;
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

function gameContext(remainTime, playerTeamNumbers, scoreRed, scoreBlue, scoreGoal) {
	this.playerTeamNumbers = playerTeamNumbers;
	this.remainTime = remainTime;
	this.scoreRed = scoreRed;
	this.scoreBlue = scoreBlue;
	this.scoreGoal = scoreGoal;
}

function gameTime(remainTime) {
	this.remainTime = remainTime;
}

function waitingStatus(joinedPlayerCount, maxPlayerCount, remianTimeToPlay) {
	this.joinedPlayerCount = joinedPlayerCount;
	this.maxPlayerCount = maxPlayerCount;
	this.remianTimeToPlay = remianTimeToPlay;
}

function respawnModel(playerNumber,currentHP, maxHP) {
	this.playerNumber = playerNumber;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
}