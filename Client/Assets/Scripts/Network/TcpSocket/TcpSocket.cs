using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

/// <summary>
/// reference link :  http://blog.naver.com/PostView.nhn?blogId=lene1359&logNo=80106310174
/// </summary>
public class TcpSocket : MonoBehaviourInstance<TcpSocket> {


    //public int testInt = 200;
    //#region OnGUI
    //public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0)
    //{
    //    return new Rect(_width * raw, _height * column, _width, _height);
    //}
    
    //void OnGUI() {
    //    if (GUI.Button(GetRectPos(0,5, 200, 50), "int Test")) {
    //        TestInteger(this.testInt);
    //    }
    //}

    public class Packet {
        public enum KIND
        {
            LOG_IN,
            CHATTING,
            LOBBY,
            GAME
        };
        public KIND kind;
        public string message;
        public List<int> itemList = new List<int>();
        public Int32 x;
    }

    public void SerializerTest() {
        // 패킷생성
        Packet p = new Packet();
        p.kind = Packet.KIND.CHATTING;
        p.message = "Hello";
        p.itemList.Add(123);
        p.itemList.Add(124);
        p.itemList.Add(125);
        p.x = Convert.ToInt32(2000);

        // Packet클래스를 BSON으로 직렬화
        MemoryStream ms = new MemoryStream();
        JsonSerializer serializer = new JsonSerializer();
        BsonWriter writer = new BsonWriter(ms);
        serializer.Serialize(writer, p);
        ms.Seek(0, SeekOrigin.Begin);

        // BSON을 바이트로 전환해 화면에 출력
        byte[] byteBSON = ms.ToArray();
        int byteLength = 0;

        foreach (byte a in byteBSON) {
            byteLength++;
            //Logger.Debug(a);
        }
        Logger.Debug("[Serializer] byte length : " + byteLength );
        Packet x = Deserializaer<Packet>(byteBSON);
        Logger.Debug("[Deserializaer] Packet.kind : " + x.kind);
        Logger.Debug("[Deserializaer] Packet.message : " + x.message);
        Logger.Debug("[Deserializaer] Packet.ltemList count : " + x.itemList.Count);
        Logger.Debug("[Deserializaer] Packet.x type : " + x.x.GetType().ToString());
    }
    private class TestClass {
        public int x;
    }
    public void TestInteger(int testInt) {
        TestClass x = new TestClass();
        x.x = testInt;

        byte[] y = SerializeToByte(x);
        for(int i = 0; i < y.Length; i++) {
            Logger.Debug("[test] [" + i + "] = " + y[i]);
        }

        TestClass result = Deserializaer<TestClass>(y);

        Logger.Debug("[result] int = " + result.x);
    }

    //public void TEST_1()
    //{
    //    string testData = "asdf";
    //    byte[] SenddataLength = BitConverter.GetBytes(testData.Length);
    //    byte[] Sendbyte = Encoding.Default.GetBytes(testData);
    //    byte[] total = byte_merge(SenddataLength, Sendbyte);

    //    string testData2 = "connected success";
    //    byte[] SenddataLength2 = BitConverter.GetBytes(testData2.Length);
    //    byte[] Sendbyte2 = Encoding.Default.GetBytes(testData2);
    //    byte[] total2 = byte_merge(SenddataLength2, Sendbyte2);
    //    byte[] total3 = byte_merge(total, total2);

    //    receiver.GetRecevieBuffer(total2);
    //    return;
    //    DataResolver x = new DataResolver();
    //    x.on_receive(total3, 0, total3.Length, receiver.CallbackRecevieBuffer);
    //}

    //public byte[] byte_merge(byte[] arg1, byte[] arg2)
    //{
    //    byte[] tmp = new byte[arg1.Length + arg2.Length];
    //    for (int i = 0; i < arg1.Length; i++)
    //    {
    //        tmp[i] = arg1[i];
    //    }
    //    for (int j = 0; j < arg2.Length; j++)
    //    {
    //        tmp[arg1.Length + j] = arg2[j];
    //    }
    //    return tmp;
    //}

    //#endregion

    public byte[] SerializeToByte(object x){
        // Packet클래스를 BSON으로 직렬화
        MemoryStream ms = new MemoryStream();
        JsonSerializer serializer = new JsonSerializer();
        BsonWriter writer = new BsonWriter(ms);
        serializer.Serialize(writer, x);
        ms.Seek(0, SeekOrigin.Begin);

        // BSON을 바이트로 전환해 화면에 출력
        return ms.ToArray();
    }

    public T Deserializaer<T>(byte[] data) {
        MemoryStream ms = new MemoryStream(data);
        ms.Seek(0, SeekOrigin.Begin);
        byte[] byteBSON = ms.ToArray();
        int byteLength = 0;
        foreach (byte a in byteBSON) {
            byteLength++;
            //Logger.Debug(a + " ");
        }
        Logger.Debug("[Deserializaer] byte length : " + byteLength);
        JsonSerializer deserializaer = new JsonSerializer();
        BsonReader reader = new BsonReader(ms);
        T dp = deserializaer.Deserialize<T>(reader);
        return dp;
    }


    private Socket m_Socket;
	public SocketRequest sender;
    public IngameClient client;
    public SocketResponse receiver;
    private NetworkStream ns;
    private string ip = "";
    private int port = 0;
    
    private int SenddataLength;                     // Send Data Length. (byte)
    private int ReceivedataLength;                     // Receive Data Length. (byte)

    private byte[] Sendbyte;                        // Data encoding to send. ( to Change bytes)
    private byte[] Receivebyte = new byte[2000];    // Receive data by this array to save.
    private string ReceiveString;                     // Receive bytes to Change string. 
    private bool isConnected = false;

    public bool IsConnected {
        get { return this.isConnected; }
    }

    public void Connect(string ip, int port, System.Action<bool> callback) {
        this.ip = ip;
        this.port = port;

        if (isConnected) {
            Logger.Debug("Socket is already connected"); 
            return;
        }

        //=======================================================
        // Socket create.
		this.m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		sender.m_Socket = this.m_Socket;
		this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
		this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
        //=======================================================
        // Socket connect.

        IPAddress ipAddr = System.Net.IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, port);
         try {
			this.m_Socket.Connect(ipEndPoint);
            this.ns = new NetworkStream(this.m_Socket);
            this.isConnected = true;
            callback(this.isConnected);
        } catch (SocketException e) {
            Logger.Debug("Socket connect error! : " + e.ToString() );
            this.isConnected = false;
            callback(this.isConnected);
            return;
        }

        //=======================================================
        // Send data write.
        //this.client.Init("hoho~!", true, 15, 150, (req, result) => {
        //    Logger.Debug("[TcpSocket.isConntected] SUCCESS");
        //});

    }

	public void ReceiveDataStream() {
        if (this.ns.DataAvailable == false) {
            return;
        }

        byte[] data = new byte[1024];
        if (this.ns.CanRead) {
            this.ns.Read(data, 0, data.Length);
            this.receiver.GetRecevieBuffer(data);
        } else {
            Logger.Debug("Error: Can't read from this socket");
            ns.Close();
            this.m_Socket.Close();
            return;
        }
    }

    void Update() {
        if (this.isConnected) {
            ReceiveDataStream();
        }
    }
    void OnApplicationQuit () {
        if(m_Socket != null) {
            this.m_Socket.Close();
            this.m_Socket = null;
        }
    }
}
