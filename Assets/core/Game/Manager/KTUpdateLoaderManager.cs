using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

namespace LuaFramework
{
    /// <summary>
    /// 顺序下载管理器，一次性顺序批量下载
    /// </summary>
    public class KTUpdateLoaderManager : MonoBehaviour, IRelease
    {
        public class LoaderInfo:IDisposable
        {
            public string url;//下载地址
            public string fileFullName;//要保存到本地的文件全路径
            public UnityWebRequest request;//需要改为UnityWebRequest
            public UnityWebRequestAsyncOperation opt;

            public void Dispose()
            {
                request.Dispose();
                opt = null;
                request = null;
            }
        }

        public static KTUpdateLoaderManager It{ get; private set; }

        public Action<string,byte[]> singleLoadedCallback = null;//单个文件下载完成回调
        public Action allLoadedCallback = null;//所有文件加载完成
        public Action<string> errorCallback = null;//下载错误回调
        public Action<string> httpErrorCallback = null;
        public Action<string> newworkCallback = null;
        public Action<string> singleProcessCallback = null;//单个文件下载进度回调
 
        private Queue<LoaderInfo> loaderRequests = null;
        private LoaderInfo currLoaderInfo = null;
        private int m_totalCount=0;
        private int m_loadedCount=0;

        void Awake()
        {
            loaderRequests = new Queue<LoaderInfo>();
            DontDestroyOnLoad(gameObject);  //防止销毁自己
            It = this;
        }

        /// <summary>
        /// 添加加载任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileFullName"></param>
        public void Add(string url, string fileFullName)
        {
            LoaderInfo info = new LoaderInfo() { url = url, fileFullName = fileFullName };
            loaderRequests.Enqueue(info);
            m_totalCount= loaderRequests.Count;
            m_loadedCount = 0;
        }

        /// <summary>
        /// Update方式加载队列资源
        /// 需要Add好所有LoaderInfo后才可以开始加载，有任意一个错误，会中断所有加载
        /// 加载完的资源不缓存
        /// </summary>
        public void StartLoad()
        {
            if (m_totalCount == 0)
                return;

            currLoaderInfo = loaderRequests.Dequeue();
            currLoaderInfo.request = KTDownloadHelper.DownloadFileRequest(currLoaderInfo.url, currLoaderInfo.fileFullName);
            currLoaderInfo.opt = currLoaderInfo.request.SendWebRequest();
        }

        /// <summary>
        /// 异步加载队列yield return方式
        /// 需要Add好所有LoaderInfo后才可以开始加载，有任意一个错误，会中断所有加载
        /// 不显示任何进度，加载完的资源不缓存
        /// </summary>
        /// <param name="allLoadedCallback"></param>
        /// <param name="singleLoadedCallback"></param>
        /// <returns></returns>
        public IEnumerator StartLoadAsync(Action<byte[]> allLoadedCallback=null, Action<byte[]> singleLoadedCallback=null,Action<string> errorCallback=null)
        {
            if (m_totalCount == 0)
                yield break;

            var loader_info = loaderRequests.Dequeue();
            yield return KTDownloadHelper.AsyncDownloadFileRequest(loader_info.url, loader_info.fileFullName,
            (req,data) =>
            {
                m_loadedCount++;

                if (singleLoadedCallback != null)
                {
                    File.WriteAllBytes(loader_info.fileFullName, data);
                    singleLoadedCallback(data);
                    req.Dispose();
                }

                if (m_loadedCount== m_totalCount)
                {
                    if (allLoadedCallback != null)
                        allLoadedCallback(data);
                }
                else
                {
                    StartCoroutine(StartLoadAsync(allLoadedCallback, singleLoadedCallback));
                }
            },
            (req)=>
            {
                if (errorCallback != null)
                    errorCallback(loader_info.url);

                req.Abort();
                req.Dispose();
            },
            (req) =>
            {
                if (errorCallback != null)
                    errorCallback(loader_info.url);

                req.Abort();
                req.Dispose();
            },
            (req) =>
            {
                if (errorCallback != null)
                    errorCallback(loader_info.url);

                req.Abort();
                req.Dispose();
            });
        }

        public void Stop()
        {
            if (currLoaderInfo != null)
            {
                currLoaderInfo.request.Abort();
                currLoaderInfo.Dispose();
                currLoaderInfo = null;
            }

            foreach(var info in loaderRequests)
            {
                info.Dispose();
            }

            loaderRequests.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            if (currLoaderInfo == null)
                return;

            if (string.IsNullOrEmpty(currLoaderInfo.request.error) && errorCallback != null)
            {
                errorCallback(currLoaderInfo.request.error);
                Stop();
                return;
            }

            if (currLoaderInfo.request.isHttpError && httpErrorCallback != null)
            {
                httpErrorCallback(Path.GetFileNameWithoutExtension(currLoaderInfo.fileFullName) + "http error");
                Stop();
                return;
            }

            if (currLoaderInfo.request.isNetworkError && newworkCallback != null)
            {
                newworkCallback(Path.GetFileNameWithoutExtension(currLoaderInfo.fileFullName) + "network error");
                Stop();
                return;
            }

            if (singleLoadedCallback != null)
            {
                //singleProcessCallback(Path.GetFileNameWithoutExtension(currLoaderInfo.fileFullName) + "..." + (currLoaderInfo.request.downloadProgress / 1.0f).ToString());
                Stop();
                return;
            }

            if (currLoaderInfo.request.isDone && currLoaderInfo.request.responseCode == 200)
                OnLoadComplete(currLoaderInfo);
        }

        /// <summary>
        /// 下载完一个文件
        /// </summary>
        /// <param name="info"></param>
        private void OnLoadComplete(LoaderInfo info)
        {
            File.WriteAllBytes(info.fileFullName, info.request.downloadHandler.data);

            if (singleLoadedCallback!=null)
                singleLoadedCallback(Path.GetFileNameWithoutExtension(currLoaderInfo.fileFullName) + " is loaded", currLoaderInfo.request.downloadHandler.data);

            this.m_loadedCount++;

            if (this.m_loadedCount == this.m_totalCount)
            {
                currLoaderInfo = null;

                if (allLoadedCallback != null)
                    allLoadedCallback();
            }
            else
            {
                StartLoad();
            }
        }

        public void Release(bool destroy = false)
        {
            singleLoadedCallback = null;
            allLoadedCallback = null;
            errorCallback = null;
            httpErrorCallback = null;
            newworkCallback = null;
            singleProcessCallback = null;
            Stop();
            loaderRequests = null;
        }
    }
}