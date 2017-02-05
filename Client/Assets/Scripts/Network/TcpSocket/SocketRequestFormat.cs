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
    public long id;

    [JsonIgnoreAttribute]
    public long time;

    public int code = 0;
    public string msg = "123";
    //public Hashtable param;

    public Dictionary<string, object> param;

    [JsonIgnoreAttribute]
    public byte[] bytes;//이건 시리얼라이즈 하지 않음

    [JsonIgnoreAttribute]
    public bool IsNotification { get { return !string.IsNullOrEmpty(method); } }

    public SocketRequestFormat(string method, long id, long time, params object[] args)
    {
        this.method = method;
        this.id = id;
        this.time = time;

        if(args != null) {
            param = new Dictionary<string, object>();
            int length = args.Length;
            for (int i = 0; i < length; i += 2) {
               param.Add(args[i] as string, i + 1 >= length ? null : args[i + 1]);
            }

            foreach (KeyValuePair<string, object> i in param)
            {
                Debug.Log("[SocketRequestFormat] key = " + i.Key + " / value = " + i.Value);
            }
        }
        //this.param = new KeyValueList(args).ToHashtable();
    }
}

public class WebSocketParam<T>
{
    //[JsonPropertyAttribute("prm")]
    public T param;
}


public class WebSocketResult<T>
{
   //[JsonPropertyAttribute("rst")]
    public T result;
}


public class KeyValueList : List<KeyValuePair<string, object>>
{
    public KeyValueList()
    {
    }

    public KeyValueList(List<KeyValuePair<string, object>> list)
        : base(list)
    {
    }

    public KeyValueList(string query)
    {
        foreach (string q in query.Split('&'))
        {
            int index = q.IndexOf('=');
            if (index >= 0)
            {
                Add(WWW.UnEscapeURL(q.Substring(0, index)), WWW.UnEscapeURL(q.Substring(index + 1)));
            }
            else
            {
                Add(WWW.UnEscapeURL(q), null);
            }
        }
    }

    public KeyValueList(IDictionary dict)
    {
        foreach (object k in dict.Keys)
        {
            Add(k as string, dict[k]);
        }
    }

    public KeyValueList(params object[] args)
    {
        int length = args.Length;
        for (int i = 0; i < length; i += 2)
        {
            Add(args[i] as string, i + 1 >= length ? null : args[i + 1]);
        }
    }

    public List<KeyValuePair<string, string>> ToStringValueList()
    {
        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
        foreach (KeyValuePair<string, object> pair in this)
        {
            object value = pair.Value;
            if (value != null && value is IList)
            {
                foreach (object v in value as IList)
                {
                    list.Add(new KeyValuePair<string, string>(pair.Key, v == null ? null : v.ToString()));
                }
            }
            else
            {
                list.Add(new KeyValuePair<string, string>(pair.Key, value == null ? null : value.ToString()));
            }
        }
        return list;
    }

    public WWWForm ToForm()
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> pair in ToStringValueList())
        {
            form.AddField(pair.Key, pair.Value);
        }
        return form;
    }

    public string ToQuery()
    {
        return string.Join("&", ToStringValueList().ConvertAll<string>(
            p => string.Format("{0}={1}",
                           WWW.EscapeURL(p.Key),
                           p.Value == null ? string.Empty : WWW.EscapeURL(p.Value.ToString())))
                           .ToArray());
    }

    public Hashtable ToHashtable()
    {
        Hashtable ht = new Hashtable();
        foreach (KeyValuePair<string, object> pair in this)
        {
            if (ht.ContainsKey(pair.Key))
            {
                object v = ht[pair.Key];
                if (v != null && v is IList)
                {
                    ((IList)v).Add(pair.Value);
                }
                else
                {
                    List<object> lst = new List<object>();
                    lst.Add(v);
                    lst.Add(pair.Value);
                    ht[pair.Key] = lst;
                }
            }
            else
            {
                ht.Add(pair.Key, pair.Value);
            }
        }
        return ht;
    }

    public void Add(string key, object value)
    {
        Add(new KeyValuePair<string, object>(key, value));
    }

    public void Set(string key, string value)
    {
        int index = FindLastIndex(m => m.Key == key);
        if (index < 0)
            Add(key, value);
        else
            base[index] = new KeyValuePair<string, object>(key, value);
    }

    public object Get(string key)
    {
        int index = FindIndex(m => m.Key == key);
        return index < 0 ? null : base[index].Value;
    }

    public object GetLast(string key)
    {
        int index = FindLastIndex(m => m.Key == key);
        return index < 0 ? null : base[index].Value;
    }

    public List<object> GetAll(string key)
    {
        return FindAll(m => m.Key == key).ConvertAll<object>(p => p.Value);
    }
}