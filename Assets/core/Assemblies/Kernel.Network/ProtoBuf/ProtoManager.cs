using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// protobuf管理器
/// </summary>
public class ProtoManager : MonoBehaviour
{
    public string ip = "183.60.243.195";
    public int port = 31009;

    private XTcpClient m_client;
    private Action m_connectSuccessCallBack;
    private bool m_lostConnection;

    void Awake()
    {
        init();
    }

    void init()
    {
        m_client = new XTcpClient();
        m_client.OnConnected += connectedHandler;
        m_client.OnDisconnected += disconnectedHandler;
        m_client.OnError += clientErrorHandler;
    }

    void clientErrorHandler(object sender, DSCClientErrorEventArgs e)
    {
        Debug.LogWarning("::OnError");
    }

    void disconnectedHandler(object sender, DSCClientConnectedEventArgs e)
    {
        Debug.LogWarning("::OnDisconnected");
    }

    void connectedHandler(object sender, DSCClientConnectedEventArgs e)
    {
        Debug.LogWarning("::OnConnected");

        if (connected)
        {
            if (m_connectSuccessCallBack != null)
            {
                m_connectSuccessCallBack();
                m_connectSuccessCallBack = null;
            }
        }
        else
        {
            m_lostConnection = true;
        }
    }

    void FixedUpdate()
    {
        if (m_client != null && m_client.Connected)
        {
         //   Globals.It.processMsg(m_client.Loop());
        }

        if (m_lostConnection)
        {
            m_lostConnection = false;
        }
    }

    //连接远程服务器
    public void connect()
    {
        m_client.Connect(ip, port);
    }

    //连接远程服务器
    public void connect(Action callback)
    {
        m_connectSuccessCallBack = callback;
        m_client.Connect(ip, port);
    }

    //发送消息
    public void send(byte[] buffer)
    {
        if (buffer != null && connected)
        {
            m_client.Send(buffer);
        }
    }

    //关闭连接
    public void close()
    {
        if (connected)
        {
            m_client.Close();
        }
    }

    //连接状态
    public bool connected
    {
        get
        {
            return m_client != null && m_client.Connected;
        }
    }
}
