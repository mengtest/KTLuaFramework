using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class KTDownloadHelper
{
    /// <summary>
    /// www方式下载文件，安卓需要，因为UnityWebRequest不支持从streamingAsset下载
    /// </summary>
    /// <param name="url"></param>
    /// <param name="completeCallback"></param>
    /// <param name="errorCallback"></param>
    /// <returns></returns>
    public static IEnumerator WWWDownloadRequest(string url, Action<WWW> completeCallback = null, Action<WWW> errorCallback = null)
    {
        using (WWW www = new WWW(url))
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                if (errorCallback != null)
                    errorCallback(www);

                yield break;
            }

            while (!www.isDone)
            {
                yield return null;
            }

            if (completeCallback != null)
                completeCallback(www);

            www.Dispose();
            yield break;
        }
    }

    /// <summary>
    /// 待改造
    /// </summary>
    /// <param name="url"></param>
    /// <param name="completeCallback"></param>
    /// <param name="errorCallback"></param>
    /// <param name="httpErrorCallback"></param>
    /// <param name="newworkCallback"></param>
    /// <returns></returns>
    public static IEnumerator AsyncGetRequest(string url,
    Action<UnityWebRequest> completeCallback=null,
    Action<UnityWebRequest> errorCallback=null,
    Action<UnityWebRequest> httpErrorCallback=null,
    Action<UnityWebRequest> newworkCallback=null)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.useHttpContinue = false;
            req.chunkedTransfer = false;
            req.redirectLimit = 0;  // disable redirects
            req.timeout = 60;
            yield return req.SendWebRequest();

            if(string.IsNullOrEmpty(req.error)&& errorCallback!=null)
                errorCallback(req);

            if (req.isHttpError && httpErrorCallback != null)
                httpErrorCallback(req);

            if (req.isNetworkError && newworkCallback != null)
                newworkCallback(req);

            if(req.isDone&& req.responseCode == 200&& completeCallback!=null)
                completeCallback(req);
        }
    }

    public static IEnumerator AsyncDownloadFileRequest(string url, string fileFullName,
    Action<UnityWebRequest,byte[]> completeCallback = null,
    Action<UnityWebRequest> errorCallback = null,
    Action<UnityWebRequest> httpErrorCallback = null,
    Action<UnityWebRequest> newworkCallback = null)
    {
        UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        req.useHttpContinue = false;
        req.chunkedTransfer = false;
        req.redirectLimit = 0;  // disable redirects
        req.timeout = 60;
        DownloadHandlerFile handler = new DownloadHandlerFile(fileFullName);
        handler.removeFileOnAbort = true;
        req.downloadHandler = handler;
        yield return req.SendWebRequest();

        if (string.IsNullOrEmpty(req.error))
        {
            if(errorCallback != null)
                errorCallback(req);

            yield break;
        }

        if (req.isHttpError)
        {
            if(httpErrorCallback != null)
                httpErrorCallback(req);

            yield break;
        }

        if (req.isNetworkError)
        {
            if(newworkCallback != null)
                newworkCallback(req);

            yield break;
        }

        while (req.isDone==false|| req.responseCode != 200)
        {
            yield return null;
        }

        if (completeCallback != null)
            completeCallback(req,handler.data);

        yield break;
    }

    public static UnityWebRequest DownloadFileRequest(string url,string fileFullName)
    {
        UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        req.useHttpContinue = false;
        req.chunkedTransfer = false;
        req.redirectLimit = 0;  // disable redirects
        req.timeout = 60;
        DownloadHandlerFile handler = new DownloadHandlerFile(fileFullName);
        handler.removeFileOnAbort = true;
        req.downloadHandler = handler;
        return req;
    }

    /// <summary>
    /// 待改造
    /// </summary>
    /// <param name="url"></param>
    /// <param name="completeCallback"></param>
    /// <param name="errorCallback"></param>
    /// <param name="httpErrorCallback"></param>
    /// <param name="newworkCallback"></param>
    /// <returns></returns>
    IEnumerator DownloadTextureRequest(string url,
    Action<Texture2D> completeCallback = null,
    Action<UnityWebRequest> errorCallback = null,
    Action<UnityWebRequest> httpErrorCallback = null,
    Action<UnityWebRequest> newworkCallback = null)
    {
        using (UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        {
            req.useHttpContinue = false;
            req.chunkedTransfer = false;
            req.redirectLimit = 0;  // disable redirects
            req.timeout = 60;
            req.downloadHandler = new DownloadHandlerTexture(true);
            yield return req.SendWebRequest();

            if (string.IsNullOrEmpty(req.error) && errorCallback != null)
                errorCallback(req);

            if (req.isHttpError && httpErrorCallback != null)
                httpErrorCallback(req);

            if (req.isNetworkError && newworkCallback != null)
                newworkCallback(req);

            if (req.isDone && req.responseCode == 200 && completeCallback != null)
                completeCallback(DownloadHandlerTexture.GetContent(req));
        }
    }

    /// <summary>
    /// 待改造
    /// </summary>
    /// <param name="url"></param>
    /// <param name="completeCallback"></param>
    /// <param name="errorCallback"></param>
    /// <param name="httpErrorCallback"></param>
    /// <param name="newworkCallback"></param>
    /// <returns></returns>
    IEnumerator DownloadBundleRequest(string url,
    Action<AssetBundle> completeCallback = null,
    Action<UnityWebRequest> errorCallback = null,
    Action<UnityWebRequest> httpErrorCallback = null,
    Action<UnityWebRequest> newworkCallback = null)
    {
        using (UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        {
            req.useHttpContinue = false;
            req.chunkedTransfer = false;
            req.redirectLimit = 0;  // disable redirects
            req.timeout = 60;
            req.downloadHandler = new DownloadHandlerAssetBundle(url,uint.MaxValue);
           
            yield return req.SendWebRequest();

            if (string.IsNullOrEmpty(req.error) && errorCallback != null)
                errorCallback(req);

            if (req.isHttpError && httpErrorCallback != null)
                httpErrorCallback(req);

            if (req.isNetworkError && newworkCallback != null)
                newworkCallback(req);

            if (req.isDone && req.responseCode == 200 && completeCallback != null)
                completeCallback(DownloadHandlerAssetBundle.GetContent(req));
        }
    }

    /// <summary>
    /// 待改造
    /// </summary>
    /// <param name="url"></param>
    /// <param name="type"></param>
    /// <param name="completeCallback"></param>
    /// <param name="errorCallback"></param>
    /// <param name="httpErrorCallback"></param>
    /// <param name="newworkCallback"></param>
    /// <returns></returns>
    IEnumerator DownloadAudioClipRequest(string url,AudioType type,
    Action<AudioClip> completeCallback = null,
    Action<UnityWebRequest> errorCallback = null,
    Action<UnityWebRequest> httpErrorCallback = null,
    Action<UnityWebRequest> newworkCallback = null)
    {
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url,type))
        {
            req.useHttpContinue = false;
            req.chunkedTransfer = false;
            req.redirectLimit = 0;  // disable redirects
            req.timeout = 60;
            req.downloadHandler = new DownloadHandlerAudioClip(url, type);
           
            yield return req.SendWebRequest();

            if (string.IsNullOrEmpty(req.error) && errorCallback != null)
                errorCallback(req);

            if (req.isHttpError && httpErrorCallback != null)
                httpErrorCallback(req);

            if (req.isNetworkError && newworkCallback != null)
                newworkCallback(req);

            if (req.isDone && req.responseCode == 200 && completeCallback != null)
                completeCallback(DownloadHandlerAudioClip.GetContent(req));
        }
    }
}
