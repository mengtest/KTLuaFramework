using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    public delegate void OnReceive(KeyValuePair<int, ByteBuffer> msg);

    [SLua.CustomLuaClass]
    public class KTNetworkManager:MonoBehaviour,IRelease
    {
        public static KTNetworkManager Instance { get; private set; }
        private SocketClient m_socket;
        private static readonly object m_lockObject = new object();
        private static Queue<KeyValuePair<int, ByteBuffer>> m_events = new Queue<KeyValuePair<int, ByteBuffer>>();

        public OnReceive onReceive = null;

        private void Awake()
        {
            Instance = this;
            m_socket = new SocketClient();
            m_socket.Init();
            m_socket.onSocket = OnSocket;
            m_socket.onReceive = OnReceive;
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void Connect()
        {
            m_socket.Connect(KTConfigs.kSocketAddress, KTConfigs.kSocketPort);
        }

        /// <summary>
        /// 发送SOCKET消息，可以考虑从池中取出ByteBuffer
        /// </summary>
        public void Send(ByteBuffer buffer)
        {
            m_socket.Send(buffer.ToBytes());
        }

        private void OnSocket(SocketMessage msg)
        {
            AddEvent((int)msg, new ByteBuffer());
        }

        private void OnReceive(byte[] rec)
        {
            ByteBuffer buffer = new ByteBuffer(rec);
            int id = buffer.ReadShort();

            AddEvent(id, buffer);
        }

        private void AddEvent(int id, ByteBuffer msg)
        {
            lock (m_lockObject)
            {
                m_events.Enqueue(new KeyValuePair<int, ByteBuffer>(id, msg));
            }
        }

        /// <summary>
        /// 交给Command，这里不想关心发给谁。
        /// </summary>
        private void Update()
        {
            while (m_events.Count > 0)
            {
                KeyValuePair<int, ByteBuffer> kv = m_events.Dequeue();
                if (onReceive != null)
                    onReceive(kv);
                //可以考虑回收ByteBuffer
            }
        }

        public void Close()
        {
            if(m_socket!=null)
                m_socket.Close();
        }

        public void Release(bool destroy = false)
        {
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
        }
    }
}