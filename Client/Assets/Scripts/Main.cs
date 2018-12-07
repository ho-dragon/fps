using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviourInstance<Main> {
    #region OnGUI
    private bool isOnGUI = true;
    private bool isTryConnected = false;
    private string connectMsg = "";
    private bool isConnected = false;
    public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0) {
        return new Rect(_width * raw, _height * column, _width, _height);
    }

    void OnGUI() {
        if (isOnGUI == false) {
            return;
        }

        if (this.isConnected == false) {
            GUI.Label(GetRectPos(1, 1, 100, 50), "IPAddress");
            ip = GUI.TextField(GetRectPos(1, 1, 200, 50), ip, 25);
            GUI.Label(GetRectPos(1, 2, 100, 50), "Port");
            port = GUI.TextField(GetRectPos(1, 2, 200, 50), port, 25);
            GUI.Label(GetRectPos(1, 3, 100, 50), "NickName");
            userName = GUI.TextField(GetRectPos(1, 3, 200, 50), userName, 25);

            if (GUI.Button(GetRectPos(1, 4, 200, 50), "Connect Socket")) {
                ConnectToServer();
            }            
        }

        if (this.isTryConnected) {
            if (this.isConnected == false) {
                GUI.Label(GetRectPos(0, 5, 500, 50), this.connectMsg);
            }
        }

        if (this.isConnected) {
            GUI.Label(GetRectPos(0, 1, 500, 50), this.connectMsg + " Click EnterRoom.");
            if (GUI.Button(GetRectPos(0, 2, 200, 50), "EnterRoom")) {
                EnterRoom(this.userName);
            }
        }
    }
    #endregion
    public GameContext context;
    private EventManager eventManager;
    public string userName = "PlayerName";
    public string ip = "127.0.0.1";
    public string port = "8107";
    public Logger.LogLevel logLevel = Logger.LogLevel.debug;
    public bool isDebugMuted = false;

    void Start() {
        Application.targetFrameRate = 60;
        Logger.Debug("[Main] Start!");
        Logger.isMuted = isDebugMuted;
        Logger.SetLogLevel(this.logLevel);
        ConnectToServer();
    }


    private void ConnectToServer() {
        TcpSocket.inst.Connect(this.ip, System.Convert.ToInt32(this.port), (isConnected, msg) => {
            this.isTryConnected = true;
            Logger.Error("Connect result = {0}, msg ={1}", isConnected, msg);
            this.isConnected = isConnected;
            this.connectMsg = msg;
            EnterRoom(this.userName);
        });
    }

    public void EnterRoom(string userName) {
        Logger.Debug("[Main.EnterRoom]");
        TcpSocket.inst.Request.EnterRoom(userName, (req, result) => {
            Logger.DebugHighlight("[Main.EnterRoom] test size = "+ result.test.Count);
            foreach (KeyValuePair<string, string> i in result.test) {
                Logger.DebugHighlight("[Main.EnterRoom] test----key = " + i.Key+ " vlaue =" + i.Value);
            }            
            JoinRoom(false, result);
        });
    }

    public void JoinRoom(bool isRunningRoom, EnterRoomModel result) {
        if (result == null) {
            Logger.Error("[Main.EnterRoom] result is null");
            return;
        }

        if (result.player == null) {
            Logger.Debug("[Main.EnterRoom] player is null");
            return;
        }

        Logger.Debug("[Main.EnterRoom] SUCCESS enter room");
        PlayerManager.inst.JoinedPlayer(result.player, true);
        if (result.otherPlayers != null) {
            foreach (PlayerModel i in result.otherPlayers) {
                PlayerManager.inst.JoinedPlayer(i, false);
            }
        } else {
            Logger.Debug("[Main] otherPlayers is null");
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isOnGUI = false;
    }

    public void StartGame(GameContextModel result) {        
        PlayerManager.inst.AssignTeam(result.playerTeamNumbers);
        this.eventManager = new EventManager();
        this.context = new GameContext(this.eventManager, result.maxPlayTime, result.playTime, result.scoreRed, result.scoreBlue, PlayerManager.inst.GetPlayerCount());
        //Todo.UI
    }

    public void EndGame() {//Todo.UI
        this.eventManager = null;
        this.context = null;
    }
}
