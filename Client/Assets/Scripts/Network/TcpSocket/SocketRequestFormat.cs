using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;

public class SocketRequestFormat
{
    public string method;
    public int code = 0;
    public string msg = "123";
    public int id;

    public Dictionary<string, object> param;

    [JsonIgnoreAttribute]
    public long time;

    public byte[] bytes;

    [JsonIgnoreAttribute]
    public bool IsNotification { get { return !string.IsNullOrEmpty(method); } }

    public SocketRequestFormat(string method, long id, long time, params object[] args) {
        this.method = method;
        this.id = (int)id;
        this.time = time;

        if(args != null) {
            param = new Dictionary<string, object>();
            int length = args.Length;
            for (int i = 0; i < length; i += 2) {
               param.Add(args[i] as string, i + 1 >= length ? null : args[i + 1]);
            }

            foreach (KeyValuePair<string, object> i in param) {
                Debug.Log("[SocketRequestFormat] key = " + i.Key + " / value = " + i.Value);
            }
        }
        //this.param = new KeyValueList(args).ToHashtable();
    }
}

public class SocketResult<T> {
    public T result;
}

