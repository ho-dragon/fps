using System.Net.Sockets;
using System.Net;

public class TcpSocket : MonoBehaviourInstance<TcpSocket> {
    private Socket socket;
    public PacketNotification receiver;
    public PacketManager packetManager;
    private NetworkStream ns;
    private string ip = "";
    private int port = 0;
    private int SenddataLength;
    private int ReceivedataLength;
    private byte[] Sendbyte;
    private byte[] Receivebyte = new byte[2000];
    private string ReceiveString;
    private bool isConnected = false;

    public PacketRequest Request {
         get { return this.packetManager.PacketRequest; }
    }

    public bool IsConnected {
        get { return this.isConnected; }
    }

    public void Connect(string ip, int port, System.Action<bool, string> callback) {
        this.ip = ip;
        this.port = port;
        if (this.isConnected) {
            Logger.Debug("Socket is already connected"); 
            return;
        }

		this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
		this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);     
        IPAddress ipAddr = System.Net.IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, port);
         try {
			this.socket.Connect(ipEndPoint);
            this.ns = new NetworkStream(this.socket);
            this.isConnected = true;
            this.packetManager.SetSocket(this.socket);
            callback(this.isConnected, "Connect success.");
        } catch (SocketException e) {
            this.isConnected = false;
            callback(this.isConnected, "Connect failed. error msg = " + e.ToString());
            return;
        }
    }

	public void ReceiveDataStream() {
        if (this.ns.DataAvailable == false) {
            return;
        }

        byte[] buffer = new byte[1024];
        if (this.ns.CanRead) {
            this.ns.Read(buffer, 0, buffer.Length);
            this.packetManager.ReceiveBuffer(buffer);
        } else {
            Logger.Debug("Error: Can't read from this socket");
            ns.Close();
            this.socket.Close();
            return;
        }
    }

    public void Send(byte[] data) {
        this.socket.Send(data, data.Length, SocketFlags.None);
    }

    void Update() {
        if (this.isConnected) {
            ReceiveDataStream();
        }
    }

    void OnApplicationQuit () {
        if (this.socket != null) {
            this.socket.Close();
            this.socket = null;
        }
    }
}
