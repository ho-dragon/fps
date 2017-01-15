using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

/// <summary>
/// reference link :  http://blog.naver.com/PostView.nhn?blogId=lene1359&logNo=80106310174
/// </summary>
public class TcpSocket : MonoBehaviourInstance<TcpSocket> {

    #region OnGUI
   //public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0)
   //{
   //    return new Rect(_width * raw, _height * column, _width, _height);
   //}

   //void OnGUI() {
   //    if (GUI.Button(GetRectPos(0,5, 200, 50), "bufferRecevier_1")) {
   
   //    }
   //    if (GUI.Button(GetRectPos(0, 6, 200, 50), "bufferRecevier_2"))
   //    {
 
   //    }
   //}

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

    #endregion

    private Socket m_Socket;
	public SocketRequest sender;
    public SocketResponse receiver;
    private NetworkStream ns;
    public string iPAdress = "127.0.0.1";
    public const int kPort = 8107;
    
    private int SenddataLength;                     // Send Data Length. (byte)
    private int ReceivedataLength;                     // Receive Data Length. (byte)

    private byte[] Sendbyte;                        // Data encoding to send. ( to Change bytes)
    private byte[] Receivebyte = new byte[2000];    // Receive data by this array to save.
    private string ReceiveString;                     // Receive bytes to Change string. 
    private bool isConnected = false;

    public bool IsConnected {
        get { return this.isConnected; }
    }

    public void Connect() {
        if (isConnected) {
            Debug.Log("Socket is already connected"); 
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

        IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
        IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
         try {
			this.m_Socket.Connect(ipEndPoint);
            this.ns = new NetworkStream(this.m_Socket);
            this.isConnected = true;
        }
        catch (SocketException e) {
            Debug.Log("Socket connect error! : " + e.ToString() ); 
            return;
        }

        //=======================================================
        // Send data write.
        try {
           this.sender.Send("init");
           Debug.Log("[TcpSocket.isConntected] SUCCESS");
        } catch (SocketException err) {
            Debug.Log("Socket send or receive error! : " + err.ToString() );
        }
    
    }

	public void ReceiveDataStream() {
        if (this.ns.DataAvailable == false) {
            return;
        }

        byte[] data = new byte[1024];
        if (this.ns.CanRead) {
            this.ns.Read(data, 0, data.Length);
            this.receiver.GetRecevieBuffer(data);
        }
        else {
            Debug.Log("Error: Can't read from this socket");
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
