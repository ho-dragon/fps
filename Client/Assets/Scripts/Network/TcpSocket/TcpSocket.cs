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

public class TcpSocket : MonoBehaviourInstance<TcpSocket> {
    private Socket socket;
    public SocketResponse receiver;
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

    public bool IsConnected {
        get { return this.isConnected; }
    }

    public void Connect(string ip, int port, System.Action<bool> callback) {
        this.ip = ip;
        this.port = port;
        if (this.isConnected) {
            Logger.Debug("Socket is already connected"); 
            return;
        }

		this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
		this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
        this.packetManager.socket = this.socket;

        IPAddress ipAddr = System.Net.IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, port);
         try {
			this.socket.Connect(ipEndPoint);
            this.ns = new NetworkStream(this.socket);
            this.isConnected = true;
            callback(this.isConnected);
        } catch (SocketException e) {
            Logger.Debug("Socket connect error! : " + e.ToString() );
            this.isConnected = false;
            callback(this.isConnected);
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
            this.receiver.GetRecevieBuffer(buffer);
        } else {
            Logger.Debug("Error: Can't read from this socket");
            ns.Close();
            this.socket.Close();
            return;
        }
    }

    public byte[] SerializeToByte(object data) {
        MemoryStream ms = new MemoryStream();
        JsonSerializer serializer = new JsonSerializer();
        BsonWriter writer = new BsonWriter(ms);
        serializer.Serialize(writer, data);
        ms.Seek(0, SeekOrigin.Begin);
        return ms.ToArray();
    }

    public T Deserialize<T>(byte[] data) {
        MemoryStream ms = new MemoryStream(data);
        ms.Seek(0, SeekOrigin.Begin);
        byte[] byteBSON = ms.ToArray();
        JsonSerializer deserializaer = new JsonSerializer();
        BsonReader reader = new BsonReader(ms);
        T result = deserializaer.Deserialize<T>(reader);
        return result;
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
