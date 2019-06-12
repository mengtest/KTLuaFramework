using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 协议管理器
/// </summary>
public class ProtocolManager : MonoBehaviour
{
    private Dictionary<int, List<IProtocol>> m_protocols;

    void Awake()
    {
        m_protocols = new Dictionary<int, List<IProtocol>>();
    }

    void Start()
    {
        //注册协议


    }

    public void register(IProtocol protocol)
    {
        if (protocol == null) return;

        if (m_protocols.ContainsKey(protocol.iCommand))
        {
            m_protocols[protocol.iCommand].Add(protocol);
        }
        else
        {
            List<IProtocol> list = new List<IProtocol>();
            list.Add(protocol);
            m_protocols.Add(protocol.iCommand, list);
        }
    }

    public List<IProtocol> getProtocol(int iCommand)
    {
        if (m_protocols.ContainsKey(iCommand)) return m_protocols[iCommand];
        return null;
    }

    public void process(Message_Body body)
    {
        Debug.LogWarning(string.Format(":::{0}:{1}", body.iCommand, System.Text.Encoding.UTF8.GetString(body.body)));
        List<IProtocol> list = getProtocol(body.iCommand);

        if (list != null && list.Count > 0)
        {
            list.ForEach(protocol => { protocol.Process(body); });//由子协议来处理具体问题
        }
    }
}
