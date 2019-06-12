using UnityEngine;
using System.Collections;

public class PingUtil : MonoBehaviour
{

    public string ip = string.Empty;
    Ping ping;
    string label;
    GUIStyle guiStyle;

    void Start()
    {
        //if (DataDefine.isOutLine)
        //{
        //    ip = "47.93.59.193";    // http://api.tianyuonline.cn
        //}
        //else
        //{
        //    ip = "192.168.3.251";
        //}
        SendPing();


        guiStyle = new GUIStyle();
        guiStyle.normal.background = null;
        guiStyle.fontSize = 40;

    }

    bool isNetWorkLose = false;
    void OnGUI()
    {
        //if (DataDefine.isShowPing)
        //{
        //    if (!ReloadGame.Instance.NetWorkIsOK())
        //    {
        //        label = "460";
        //        SetColor(460);
        //        isNetWorkLose = true;
        //    }
        //    else if (null != ping && (isNetWorkLose || ping.isDone))
        //    {
        //        isNetWorkLose = false;
        //        label = ping.time.ToString();
        //        SetColor(ping.time);
        //        ping.DestroyPing();
        //        ping = null;
        //        Invoke("SendPing", 1);//每秒Ping一次
        //    }

        //    GUI.Label(new Rect(10, 50, 200, 50), "ping:" + label + "ms", guiStyle);
        //}
    }

    void SendPing()
    {
        ping = new Ping(ip);
    }

    void SetColor(int pingValue)
    {
        if (pingValue < 100)
        {
            guiStyle.normal.textColor = new Color(0, 1, 0);
        }
        else if (pingValue < 200)
        {
            guiStyle.normal.textColor = new Color(1, 1, 0);
        }
        else
        {
            guiStyle.normal.textColor = new Color(1, 0, 0);
        }
    }
}