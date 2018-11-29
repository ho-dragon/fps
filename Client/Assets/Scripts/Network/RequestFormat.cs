using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;

public class RequestFormat {
    public string method;
    public int code = 0;
    public string msg;
    public int id;

    public Dictionary<string, object> param;

    public byte[] bytes;

    [JsonIgnoreAttribute]
    public bool IsNotification { get { return !string.IsNullOrEmpty(method); } }

    public RequestFormat(string method, long id, params object[] args) {
        this.method = method;
        this.id = (int)id;

        if(args != null) {
            param = new Dictionary<string, object>();
            int length = args.Length;
            for (int i = 0; i < length; i += 2) {
               param.Add(args[i] as string, i + 1 >= length ? null : args[i + 1]);
            }

            if (Logger.IsMutePacket(method) == false) {
                foreach (KeyValuePair<string, object> i in param) {
                    Logger.Debug("[SocketRequestFormat] key = " + i.Key + " / value = " + i.Value);
                }
            }            
        }
    }
}

public class ResponseFormat {
    public string method;
    public int code = 0;
    public string msg;
    public int id;
    public byte[] bytes;

    [JsonIgnoreAttribute]
    public bool IsNotification { get { return !string.IsNullOrEmpty(method); } }
}

public class SocketResult<T> {
    public T result;
}

