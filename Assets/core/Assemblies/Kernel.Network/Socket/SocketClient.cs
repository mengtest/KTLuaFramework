using System;
using System.Net.Sockets;
using System.Net;

namespace LuaFramework
{
    public enum eNetState
    {
        net_none = 1,
        net_working,
        net_dropped,
    }

    public enum SocketMessage
    {
        Connect = 101,
        Disconnect = 102,
        Exception = 103,
    }

    public delegate void SocketCallback(SocketMessage msg);
    public delegate void ReceiveMessageCallback(byte[] rec);

    /// <summary>
    /// 主要功能：网络字节tcp socket通信类
    /// 注意事项：
    /// 创建日期：2017.7.22
    /// 最后修改日期：2017.7.22
    /// 最后修改人（包括创建人）：lijunfeng
    /// </summary>
    public class SocketClient
    {
        struct sBufferInfo
        {
            public ushort usBufferSize;
            public ushort usReadSize;
        }

        public static int m_buffSize = 1024;//接收缓冲区大小

        private TcpClient m_tcpClient = null;
        private NetworkStream m_netStream = null;
        private IPAddress m_address = null;
        private int m_port = 0;

        private eNetState m_netState = eNetState.net_none;
        private bool m_isConnected = false;

        private byte[] t_headBuff = new byte[2];
        private byte[] t_bodyBuff = null;
        private sBufferInfo t_bufferInfo = new sBufferInfo();
        public event EventHandler<SocketEventHandle> m_NetEvent;

        public string errorInfo;
        public SocketCallback onSocket = null;//连接事件
        public ReceiveMessageCallback onReceive = null;

        public void Init()
        {
            t_bodyBuff = new byte[m_buffSize];
        }

        ~SocketClient()
        {
            if (m_tcpClient != null)
            {
                m_tcpClient.Close();
                m_tcpClient = null;
            }

            if (m_netStream != null)
            {
                m_netStream.Close();
                m_netStream = null;
            }

            m_address = null;
            t_headBuff = null;
            t_bodyBuff = null;
            onSocket = null;
            onReceive = null;
        }

        public void Connect(string IP, int port)
        {
            try
            {
                this.m_address = IPAddress.Parse(IP);
                this.m_port = port;
            }
            catch (Exception e)
            {
                errorInfo = "NConnect: " + e.Message;

                if (onSocket != null)
                    onSocket(SocketMessage.Exception);
            }

            m_isConnected = true;

            while (true)
            {
                try
                {
                    if (m_tcpClient == null)
                        m_tcpClient = new TcpClient();

                    m_tcpClient.BeginConnect(m_address, m_port,
                                new AsyncCallback(ConnectCallback), null);

                    break;
                }
                catch (Exception e)
                {
                    NetDrop(e.Message);
                }
            }

            m_isConnected = false;
        }

        public void Send(byte[] msg)
        {
            try
            {
                ushort length = (ushort)msg.Length;

                if (length > sizeof(ushort) && length < m_buffSize)
                {
                    if (m_netStream.CanWrite)
                    {
                        byte[] _data = new byte[length + sizeof(ushort)];
                        Array.Copy(BitConverter.GetBytes(length), _data, sizeof(ushort));
                        Array.Copy(msg, 0, _data, sizeof(ushort), length);
                        m_netStream.BeginWrite(_data, 0, length + sizeof(ushort), null, null);
                    }
                    else
                    {
                        NetDrop("NetStrm don't CanRead!");
                    }
                }
                else
                {
                    NetDrop("byData.Length error!");
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (m_tcpClient != null)
                {
                    m_tcpClient.Close();

                    m_tcpClient = null;
                    m_netStream = null;
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        public eNetState CurNetState
        {
            get { return m_netState; }
        }

        public bool IsStartConnecting()
        {
            return m_isConnected;
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                if (m_tcpClient != null)
                {
                    m_tcpClient.EndConnect(result);

                    if (m_tcpClient.Connected)
                    {
                        m_netStream = m_tcpClient.GetStream();
                        m_netState = eNetState.net_working;

                        if (onSocket != null)
                            onSocket(SocketMessage.Connect);
                    }

                    this.ReceiveHeadLen();
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        private void ReceiveHeadLen()
        {
            try
            {
                if (m_netStream != null && m_netStream.CanRead)
                {
                    t_bufferInfo.usBufferSize = sizeof(ushort);
                    t_bufferInfo.usReadSize = 0;

                    m_netStream.BeginRead(t_headBuff, 0, sizeof(ushort), new AsyncCallback(HeadLenCallBack), null);
                }
                else
                {
                    NetDrop("NetStrm don't CanRead!");
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        private void HeadLenCallBack(IAsyncResult result)
        {
            try
            {
                if (m_netStream != null)
                {
                    t_bufferInfo.usReadSize += (ushort)m_netStream.EndRead(result);

                    if (t_bufferInfo.usReadSize < t_bufferInfo.usBufferSize)
                    {
                        m_netStream.BeginRead(t_headBuff, t_bufferInfo.usReadSize,
                                            t_bufferInfo.usBufferSize - t_bufferInfo.usReadSize,
                                            new AsyncCallback(HeadLenCallBack), null);
                    }
                    else
                    {
                        ushort sBodyLen = (ushort)BitConverter.ToUInt16(t_headBuff, 0);//包体长度
                        t_bufferInfo.usBufferSize = sBodyLen;
                        t_bufferInfo.usReadSize = 0;

                        if (sBodyLen > 0 && sBodyLen < m_buffSize)
                        {
                            m_netStream.BeginRead(t_bodyBuff, 0, sBodyLen,
                                           new AsyncCallback(ReceiveCallBack), null);
                        }
                        else
                        {
                            NetDrop("HeadLen = " + sBodyLen.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            t_bufferInfo.usReadSize += (ushort)m_netStream.EndRead(result);

            try
            {
                if (t_bufferInfo.usReadSize < t_bufferInfo.usBufferSize)
                {
                    m_netStream.BeginRead(t_bodyBuff, t_bufferInfo.usReadSize,
                                    t_bufferInfo.usBufferSize - t_bufferInfo.usReadSize,
                                    new AsyncCallback(ReceiveCallBack), null);
                }
                else
                {
                    byte[] body = new byte[t_bufferInfo.usBufferSize];
                    Array.Copy(t_bodyBuff, body, t_bufferInfo.usBufferSize);

                    if (onReceive != null)
                        onReceive(body);

                    //EventHandler<NetEventHandle> temp = m_NetEvent;
                    //if (temp != null)
                    //{
                    //    int nSize = mCompress.decompress(m_byReceiveBuf, m_BufferInfo.usBufferSize,
                    //                            m_byUnCompressBuf);
                    //    if ( nSize == -1 )
                    //    {
                    //        NetDrop("Decompress error!");
                    //        return;
                    //    }

                    //    m_NetEvent(this, new NetEventHandle(m_byUnCompressBuf, (UInt16)nSize));
                    //}

                    this.ReceiveHeadLen();
                }
            }
            catch (Exception e)
            {
                NetDrop(e.Message);
            }
        }

        private void NetDrop(string except)
        {
            m_netState = eNetState.net_dropped;
            this.Close();
            errorInfo = "net_dropped: " + except;

            if (onSocket != null)
                onSocket(SocketMessage.Exception);
        }
    }
}