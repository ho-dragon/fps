using UnityEngine;

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
            playerId = GUI.TextField(GetRectPos(1, 3, 200, 50), playerId, 25);

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
                EnterRoom(this.playerId);
            }
        }
    }
    #endregion
    public GameContext context;
    private EventManager eventManager;
    private string playerId = "";
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

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public string GetPlayerId() {
        return this.playerId;
    }

    private string CreatePlayerId() {
        System.Guid guid= System.Guid.NewGuid();
        string nickName = System.Convert.ToBase64String(guid.ToByteArray());
        nickName = nickName.Replace("=", "");
        nickName = nickName.Replace("+", "");
        return string.Format("User_{0}", nickName);
    }

    private void ConnectToServer() {
        if (string.IsNullOrEmpty(this.playerId)) {
            this.playerId = CreatePlayerId();
        }

        TcpSocket.inst.Connect(this.ip, System.Convert.ToInt32(this.port), (isConnected, msg) => {
            this.isTryConnected = true;
            Logger.Error("Connect result = {0}, msg ={1}", isConnected, msg);
            this.isConnected = isConnected;
            this.connectMsg = msg;
            EnterRoom(this.playerId);
        });
    }

    public void EnterRoom(string userName) {
        Logger.Debug("[Main.EnterRoom]");
        TcpSocket.inst.Request.EnterRoom(userName, (req, result) => {
            JoinRoom(false, result);
        });
    }

    public void JoinRoom(bool isRunningRoom, EnterRoomModel result) {
        if (result == null) {
            Logger.Error("[Main.JoinRoom] result is null");
            return;
        }

        if (result.player == null) {
            Logger.Debug("[Main.JoinRoom] player is null");
            return;
        }

        PlayerManager.inst.JoinPlayer(result.player, true);
        if (result.otherPlayers != null) {
            foreach (PlayerModel i in result.otherPlayers) {
                PlayerManager.inst.JoinPlayer(i, false);
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        this.isOnGUI = false;
        if (isRunningRoom) {
            StartGame(isRunningRoom, result.runningGameContext);
        }
    }

    public void StartGame(bool isRunningGame, GameContextModel result) {      
        if (result == null) {
            Logger.Error("[Main.StartGame] reuslt is null");
            return;
        }

        this.eventManager = new EventManager();
        PlayerManager.inst.AssignTeam(result.playerTeamNumbers);
        UIManager.inst.hud.AddEvents(this.eventManager);
        UIManager.inst.hud.EnablePlayerStatus();
        UIManager.inst.hud.SetMyTeamCode(PlayerManager.inst.GetLocalPlayer().GetTeamCode());

        if (isRunningGame) {
            UIManager.inst.ShowToastMessgae(string.Format("재접속 완료! 목표 점수 {0}", result.scoreGoal), 5f);
        } else {
            UIManager.inst.ShowToastMessgae(string.Format("게임이 시작! 목표 점수 {0}", result.scoreGoal), 5f);
        }
        
        this.context = new GameContext(this.eventManager
                                     , result.remainTime
                                     , result.scoreRed
                                     , result.scoreBlue
                                     , PlayerManager.inst.GetPlayerCount(TeamCode.RED)
                                     , PlayerManager.inst.GetPlayerCount(TeamCode.BLUE));

    }

    public void EndGame() {
        UIManager.inst.EndGame();
        this.eventManager = null;
        this.context = null;
    }
}
