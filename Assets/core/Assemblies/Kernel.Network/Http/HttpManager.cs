using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 提交方式
/// </summary>
public enum HttpType
{
    Get,
    Post
}

/// <summary>
/// 类主要功能：http网络通信管理器
/// 注意事项：
/// 创建日期：2015.8.14
/// 最后修改日期：2015.8.14
/// 最后修改人（包括创建人）：lijunfeng
/// </summary>
public class HttpManager : AbsBehaviour<HttpManager>,IRelease
{
    private static HttpManager _instance =null;
    
    private List<IHttpMessage> m_httpMessageList = null;
    private List<IHttpMessage> m_receiveList = null;
    private Dictionary<int, StackPoolManager<IHttpMessage>> m_messagePool = null;

    private string m_ip;

    public void Init(string ip)
    {
        m_ip = ip;
    }

    /// <summary>
    /// 注：需要加密
    /// post方式只能发送2进制
    /// get只能发字符串
    /// </summary>
    /// <typeparam name="T">接收消息是需要转换的类型，BaseData子类型</typeparam>
    /// <param name="protocol"></param>
    /// <param name="jsonToSend"></param>
    /// <param name="httpType">发送格式</param>
    /// <param name="receiveType">接收数据类型enum DataType</param>
    /// <param name="callback"></param>
    public void SendHttpMessage<T>(int protocol, object jsonToSend, HttpType httpType, System.Action<int,int,T> callback = null) where T :BaseData
    {
        HttpMessage<T> t_msg= GetFreeMessage<T>() as HttpMessage<T>;
        t_msg.SendHttpMessage(protocol, jsonToSend, httpType, callback);
        t_msg.onReceiveError = OnReceiveError;
        t_msg.onReceiveOver = OnReceiveOver;
        m_httpMessageList.Add(t_msg);
    }

    private IHttpMessage GetFreeMessage<T>() where T : BaseData
    {
        StackPoolManager<IHttpMessage> t_pool = null;

        if(!m_messagePool.TryGetValue(typeof(T).GetHashCode(),out t_pool))
        {
            t_pool = new StackPoolManager<IHttpMessage>();
            m_messagePool.Add(typeof(T).GetHashCode(), t_pool);
        }
            
        IHttpMessage t_msg = t_pool.PoolFrom();

        if (t_msg == null)
            t_msg = new HttpMessage<T>(m_ip);    

        return t_msg;
    }

    public int MessageCount {get { return m_httpMessageList.Count; }  }

    public void RecycleMessage<T>(IHttpMessage httpMessage) where T :BaseData
    {
        Debug.Log("recycled message");
        m_httpMessageList.Remove(httpMessage);
        m_messagePool[typeof(T).GetHashCode()].PoolTo(httpMessage);
    }

    private void OnReceiveOver(IHttpMessage msg)
    {
        lock(m_receiveList)
        {
            m_receiveList.Add(msg);
        }
    }

    /// <summary>
    /// 接收消息错误
    /// </summary>
    /// <param name="result">错误号</param>
    private void OnReceiveError(int result)
    {
        switch(result)
        {
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        _instance = this;
        m_httpMessageList = new List<IHttpMessage>();
        m_receiveList = new List<IHttpMessage>();
        m_messagePool = new Dictionary<int, StackPoolManager<IHttpMessage>>();
    }

    void FixedUpdate()
    {
        if (m_receiveList == null)
            return;

        lock(m_receiveList)
        {
            while(m_receiveList.Count > 0)
            {
                IHttpMessage t_msg = m_receiveList[0];
                m_receiveList.RemoveAt(0);
                t_msg.ExecuteCallback();
            }
        }
    }

    public void Release(bool destory = false)
    {
        foreach (var item in m_httpMessageList)
        {
            (item as IRelease).Release();
        }

        m_httpMessageList.Clear();

        foreach (var item in m_receiveList)
        {
            (item as IRelease).Release();
        }

        m_receiveList.Clear();

        foreach(var item in m_messagePool)
        {
            item.Value.Release(destory);
        }

        m_messagePool.Clear();

        if (destory)
        {
            m_httpMessageList = null;
            m_receiveList = null;
            m_messagePool = null;
            _instance = null;
            Destroy(this);
        }
    }

}
