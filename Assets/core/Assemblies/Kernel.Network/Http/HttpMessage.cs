using LitJson;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public interface IHttpMessage
{
    void ExecuteCallback();
}

/// <summary>
/// 要接收的消息
/// </summary>
/// <typeparam name="T">接收的数据类型</typeparam>
public sealed class ReceiveMessage<T> where T : BaseData
{
    public int result = 0;//0代表成功，其他数字表示错误号
    public int protocol = 0;//协议号
    public T data;
}

/// <summary>
/// 要发送的消息
/// </summary>
public sealed class SendMessage
{
    public int protocol = 0;//协议号
    public object data; 
}

public delegate void OnReceiveError(int result);
public delegate void OnReveiveOver(IHttpMessage msg);

/// <summary>
/// 要发送的消息
/// 返回的结果里要有result如果为0表示成功，>0表示错误号
/// 接收结果成功时，字段data可以取到值
/// </summary>
/// <typeparam name="T">接收消息时需要转换的类型</typeparam>
public sealed class HttpMessage<T>: IHttpMessage,IRelease where T :BaseData
{
    public const string PrefixProtocol = "?jsonObj=";
    public const int IO_ERROR = 10000;
    public const int ANALYZE_ERROR = 10001;

    public OnReceiveError onReceiveError = null;
    public OnReveiveOver onReceiveOver = null;

    private string m_ip;

    private object m_jsonToSend = null;//json数据
    private HttpType m_httpType = HttpType.Post;//发送方式
    private int m_result = 0;//0代表成功，其他数字表示错误号

    private HttpClient m_httpClient = null;
    private System.Action<int, int, T> m_callback = null;
    private ReceiveMessage<T> m_receiveMsg = null;//接收的消息

    public HttpMessage(string ip)
    {
        m_ip = ip;
        m_httpClient = new HttpClient(MessageCallback);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocol">协议</param>
    /// <param name="jsonToSend">数据对象</param>
    /// <param name="httpType">发送类型HttpType</param>
    /// <param name="callback"></param>
    public void SendHttpMessage(int protocol, object jsonToSend, HttpType httpType, System.Action<int,int,T> callback = null)
    {
        m_jsonToSend = jsonToSend;
        m_httpType = httpType;
        m_callback = callback;

        if (m_httpType == HttpType.Post)
        {
            SendMessage msg = new SendMessage();
            msg.protocol = protocol;
            msg.data = m_jsonToSend;

            m_httpClient.PostAsync(m_ip, JsonToBytes(msg));

        }
        else//HttpType.Get
        {
            m_httpClient.GetAsync(m_ip + PrefixProtocol + JsonToString(m_jsonToSend));
        }
    }

    public void SendHttpMessage(int protocol, string jsonStr, HttpType httpType, System.Action<int, int, T> callback = null)
    {
        m_jsonToSend = StringToJson<T>(jsonStr);

        SendHttpMessage(protocol, m_jsonToSend, httpType, callback);
    }

    public void SendHttpMessage(int protocol, byte[] jsonBytes, HttpType httpType, System.Action<int, int, T> callback = null)
    {
        m_jsonToSend = BytesToJson<T>(jsonBytes);

        SendHttpMessage(protocol, m_jsonToSend, httpType, callback);
    }

    private void MessageCallback(HttpContent content)
    {
        if(content==null)
        {
            onReceiveError(IO_ERROR);//io错误
            Release();
            HttpManager.Instance.RecycleMessage<T>(this);
            return;
        }

        try
        {
            byte[] b = content.GetBytes;
            ReceiveMessage<T> result = BytesToJson<ReceiveMessage<T>>(b);
            OnReceive(result);
        }
        catch (Exception e)
        {
            onReceiveError(ANALYZE_ERROR);//json序列化错误
            Release();
            HttpManager.Instance.RecycleMessage<T>(this);
        }
    }

    private void OnReceive(ReceiveMessage<T> msg)
    {
        m_receiveMsg = msg; 
        m_result = msg.result;

        if (m_result > 0 && onReceiveError != null)
        {
            onReceiveError(m_result);//逻辑错误
        }

        if (onReceiveOver != null)
            onReceiveOver(this);
    }

    /// <summary>
    /// 执行回调
    /// </summary>
    public void ExecuteCallback()
    {
        if (m_callback != null)
        {
            m_callback(m_receiveMsg.protocol, m_receiveMsg.result, m_receiveMsg.data);
        }

        Release();
        HttpManager.Instance.RecycleMessage<T>(this);
    }

    /// <summary>
    /// 发送时需要
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private string JsonToString(object obj)
    {
        return JsonMapper.ToJson(obj);
    }

    /// <summary>
    /// 发送时需要
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private byte[] JsonToBytes(object obj)
    {
        string jsonStr = JsonMapper.ToJson(obj);
        return System.Text.Encoding.UTF8.GetBytes(jsonStr);
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <typeparam name="V">要转换的类型</typeparam>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    private V StringToJson<V>(string jsonStr)
    {
        return JsonMapper.ToObject<V>(jsonStr);
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <typeparam name="T">要转换的类型</typeparam>
    /// <param name="jsonBytes"></param>
    /// <returns></returns>
    private V BytesToJson<V>(byte[] jsonBytes)
    {
        string jsonStr = System.Text.Encoding.UTF8.GetString(jsonBytes);
        Debug.Log(jsonStr);
        return JsonMapper.ToObject<V>(jsonStr);
    }

    public void Release(bool destroy = false)
    {
        m_jsonToSend = null;
        m_httpType = HttpType.Post;
        m_result = 0;

        m_callback = null;
        onReceiveError = null;
        onReceiveOver = null;
        m_receiveMsg = null;

        if (destroy)
        {
            m_httpClient = null;
        }
    }
}
