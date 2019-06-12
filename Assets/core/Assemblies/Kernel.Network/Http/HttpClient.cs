using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/// <summary>
/// 请求返回内容
/// </summary>
public class HttpContent
{
    private byte[] m_result;

    /// <summary>
    /// 
    /// </summary>
    public byte[] GetBytes { get { return m_result; } }

    /// <summary>
    /// 
    /// </summary>
    public string GetString { get { return System.Text.Encoding.UTF8.GetString(m_result); } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
	public HttpContent(byte[] response)
    {
        m_result = response;
    }
}

public delegate void HttpReceiveCallback(HttpContent cs);

/// <summary>
/// 提供基本类，用于发送 HTTP 请求和接收来自通过 URI 确认的资源的 HTTP 响应。 
/// by lijunfeng 2015/9/16
/// </summary>
public sealed class HttpClient
{
    private string m_url=string.Empty;
    private string m_contentType = "application/json";
    private int m_maxResponseContentBufferSize = 1024;
    private int m_timeout = 10000;//10s Time Out

    private HttpWebRequest m_request;
    private HttpWebResponse m_response;
    private ManualResetEvent m_allDone;
    private HttpReceiveCallback m_httpReceiveCallback;

    private byte[] m_requestBytes;//请求字节
    private bool m_canRecycle = false;

    /// <summary>
    /// 获取或设置发送请求时使用的 Internet 资源的统一资源标识符 (URI) 的基址。
    /// </summary>
    public string BaseAddress
    {
        get
        {
            return m_url;
        }

        set
        {
            m_url = value;
        }
    }

    /// <summary>
    /// 请求数据格式
    /// </summary>
    public string ContentType
    {
        get
        {
            return m_contentType;
        }

        set
        {
            m_contentType = value;
        }
    }

    /// <summary>
    /// 获取或设置读取响应内容时要缓冲的最大字节数。 
    /// </summary>
    public int MaxResponseContentBufferSize
    {
        get
        {
            return m_maxResponseContentBufferSize;
        }

        set
        {
            m_maxResponseContentBufferSize = value;
        }
    }

    /// <summary>
    /// 获取或设置请求超时前等待的时间跨度
    /// </summary>
    public int Timeout
    {
        get
        {
            return m_timeout;
        }

        set
        {
            m_timeout = value;
        }
    }

    public bool CanRecycle
    {
        get
        {
            return m_canRecycle;
        }
    }

    public static HttpClient CreateInstance()
    {
        HttpClient result = new HttpClient();
        return result;
    }

    public HttpClient()
    {
    }

    public HttpClient(HttpReceiveCallback callback)
    {
        m_httpReceiveCallback = callback;
    }

    /// <summary>
    /// 用以异步操作的取消标记发送 POST 请求。 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="content"></param>
    public void PostAsync(string url,byte[] content)
    {
        try
        {
            m_requestBytes = content;

            if (m_allDone == null)
                m_allDone = new ManualResetEvent(false);

        //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            m_request = (HttpWebRequest)WebRequest.Create(url);
            m_request.Method = "POST";
            m_request.ContentType = m_contentType;
        //    m_request.ProtocolVersion = HttpVersion.Version10;
            WaitResponse();
        }
        catch (Exception e)
        {
            m_canRecycle = true;
            UnityEngine.Debug.Log("SendRequest Catch Exception:\n" + e.ToString());
            m_httpReceiveCallback(null);
        }
    }

    /// <summary>
    /// 用以异步操作的 HTTP 完成选项和取消标记发送 GET 请求到指定的 URI。 
    /// </summary>
    /// <param name="url"></param>
    public void GetAsync(string url)
    {
        try
        {
            m_requestBytes = null;

            if (m_allDone == null)
                m_allDone = new ManualResetEvent(false);

            m_request = (HttpWebRequest)WebRequest.Create(url);
            m_request.Method = "GET";
            m_request.ContentType = ContentType;
            WaitResponse();
        }
        catch (Exception e)
        {
            m_canRecycle = true;
            UnityEngine.Debug.Log("SendRequest Catch Exception:\n" + e.ToString());
            m_httpReceiveCallback(null);
        }
    }

    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true; //总是接受  
    }

    private void WaitResponse()
    {
        try
        {
            if (m_requestBytes != null)
            {
                m_request.ContentLength = m_requestBytes.Length;
                m_request.BeginGetRequestStream(new AsyncCallback(ReadReqCallback), m_request);
            }
            else
            {
                m_allDone.Set();//SetStreamWirteDone
            }

            IAsyncResult t_result = (IAsyncResult)m_request.BeginGetResponse(new AsyncCallback(RespCallback),m_request);
            ThreadPool.RegisterWaitForSingleObject(t_result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), m_request, m_timeout, true);
        }
        catch (Exception e)
        {
            m_canRecycle = true;
            UnityEngine.Debug.Log("SendRequest Catch Exception:\n" + e.ToString());
            m_httpReceiveCallback(null);
        }
    }

    private void TimeoutCallback(object state, bool timedOut)
    {
        if (timedOut)
        {
            HttpWebRequest t_request = state as HttpWebRequest;

            if (t_request != null)
            {
                UnityEngine.Debug.Log("TimeoutCallback:\nTime Out");
                t_request.Abort();
                m_response.Close();
                m_allDone.Set();
            }
        }
    }

    private void ReadReqCallback(IAsyncResult asynchronousResult)
    {
        try
        {
            HttpWebRequest t_request = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream t_streamWrite = t_request.EndGetRequestStream(asynchronousResult);
            int t_len = m_requestBytes.Length;
            t_streamWrite.Write(m_requestBytes, 0, t_len);
            t_streamWrite.Close();
            m_allDone.Set();//SetStreamWirteDone
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("TimeoutCallback:\nTime Out");
            m_request.Abort();
            m_response.Close();
            m_allDone.Set();
        }
    }

    private void RespCallback(IAsyncResult asynchronousResult)
    {
        try
        {
            m_allDone.WaitOne();

            HttpWebRequest t_request = asynchronousResult.AsyncState as HttpWebRequest;
            m_response = (HttpWebResponse)t_request.EndGetResponse(asynchronousResult);
            Stream t_streamRead = m_response.GetResponseStream();

            if (t_streamRead.CanRead)
            {
                int t_read = 0;
                List<byte> t_result = new List<byte>();
                byte[] t_buffer = new byte[m_maxResponseContentBufferSize];

                do
                {
                    t_read = t_streamRead.Read(t_buffer, 0, m_maxResponseContentBufferSize);
                    t_result.AddRange(GetBytes(t_buffer, t_read));

                }
                while (t_read > 0);


                m_httpReceiveCallback(new HttpContent(t_result.ToArray()));
                m_canRecycle = true;
            }

            m_response.Close();
        }
        catch (Exception e)
        {
            m_response.Close();
            m_canRecycle = true;
            UnityEngine.Debug.Log("SendRequest Catch Exception:\n" + e.ToString());
            m_httpReceiveCallback(null);
        }
    }

    private byte[] GetBytes(byte[] bytes, int count)
    {
        byte[] result = new byte[count];
        for (int i = 0; i < count; ++i)
        {
            result[i] = bytes[i];
        }
        return result;
    }
}