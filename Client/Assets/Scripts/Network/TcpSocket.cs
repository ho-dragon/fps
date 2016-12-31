using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// reference link :  http://blog.naver.com/PostView.nhn?blogId=lene1359&logNo=80106310174
/// </summary>
public class TcpSocket : MonoBehaviourInstance<TcpSocket> {
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
        try {
            IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress); 
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
			this.m_Socket.Connect(ipEndPoint);
        }
        catch (SocketException e) {
            Debug.Log("Socket connect error! : " + e.ToString() ); 
            return;
        }
        
        //=======================================================
        // Send data write.
        StringBuilder sb = new StringBuilder(); // String Builder Create
        sb.Append("init");
        try {
             //=======================================================
            // Send.
            SenddataLength = Encoding.Default.GetByteCount(sb.ToString());
            Sendbyte = Encoding.Default.GetBytes(sb.ToString());
			this.m_Socket.Send(Sendbyte, Sendbyte.Length, 0);
                 
            //=======================================================       
            // Receive.
			this.m_Socket.Receive(Receivebyte);
            ReceiveString = Encoding.Default.GetString(Receivebyte);
            ReceivedataLength = Encoding.Default.GetByteCount(ReceiveString.ToString());
            Debug.Log("Init Receive Data From Server: " + ReceiveString + "(" + ReceivedataLength + ")");
            this.isConnected = true;
        }
        catch (SocketException err) {
            Debug.Log("Socket send or receive error! : " + err.ToString() );
        }
        this.ns = new NetworkStream(this.m_Socket);
    }

	public void ReceiveDataStream() {
        if (this.ns.DataAvailable == false) {
            return;
        }

        byte[] data = new byte[1024];
        int recv;
        string stringData;
        if (this.ns.CanRead)
        {
            recv = ns.Read(data, 0, data.Length);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Receviced Data From Server : data = " + stringData);
            this.receiver.Receiver(stringData);
        }
        else
        {
            Debug.Log("Error: Can't read from this socket");
            ns.Close();
            this.m_Socket.Close();
            return;
        }
    }

    void Update() {
        if(this.isConnected) {
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
