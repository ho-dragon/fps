
module.exports.playerMove = playerMove;
module.exports.playerDamage = playerDamage;
module.exports.enterRoom = enterRoom;
module.exports.responseFormat = responseFormat;
module.exports.notificationFormat = notificationFormat;
module.exports.player = player;
module.exports.playerAction = playerAction;


function playerMove(playerNum, playerPosX, playerPosY, playerPosZ, playerYaw) {
	this.playerNum = playerNum;
	this.playerPosX = playerPosX;
	this.playerPosY = playerPosY;
	this.playerPosZ = playerPosZ;
	this.playerYaw = playerYaw;
}

function playerDamage(attackPlayer, damagedPlayer, damage, currentHP, maxHP, isDead, scoreTeam1, scoreTeam2) {
	this.attackPlayer = attackPlayer;
	this.damagedPlayer = damagedPlayer;
	this.damage = damage;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
	this.isDead = isDead;
	this.scoreTeam1 = scoreTeam1;
	this.scoreTeam2 = scoreTeam2;
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


function player(name, number, teamCode, currentHP, maxHP, lastPosition, lastYaw, isDead, deadCount) {
	this.name = name;
	this.number = number;
	this.teamCode = teamCode;
	this.currentHP = currentHP;
	this.maxHP = maxHP;
	this.lastPosition = lastPosition;
	this.lastYaw = lastYaw;
	this.isDead = isDead;
	this.deadCount = deadCount;
}

function playerAction(playerNum, actionType) {
	this.playerNum = playerNum;
	this.actionType = actionType;
}

function GameInfo(isStarted, score) {
	this.score = score;
	this.isStarted = isStarted;
}


function StartGame(remainTime, playerTeamNumber, GameInfo) {


}

function GameTime(remainTime) {//초당 보내는 게임 시작
	this.remainTime = remainTime;
}


function GameResult(scoreTeam1, scoreTeam2) {

}