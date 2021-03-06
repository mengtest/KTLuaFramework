﻿using System;
using System.Collections.Generic;

namespace BundleLoader
{
    //队列加载进度回调
    public delegate void QueueLoadingProgress(float progress);

    /// <summary>
    /// 队列加载信息
    /// </summary>
    public class QueueLoaderDesc
    {
        public QueueLoaderDesc(object script, string path, string name, ResourceType type, Order order, bool instance, params string[] args)
        {
            this.script = script;
            this.path = path;
            this.name = name;
            this.type = type;
            this.order = order;
            this.instance = instance;
            this.args = args;
        }

        public object script;
        public string path;
        public string name;
        public ResourceType type;
        public Order order;
        public bool instance;
        public string[] args;
        public float progress = 0;//当前进度

        public void Release()
        {
            script = null;
        }
    }

    /// <summary>
    /// 类主要功能：队列加载器,一次性加载一个队列，直到完成，中途不可混合其他加载
    /// 注意事项：
    /// 创建日期：2015.9.24
    /// 最后修改日期：2015.9.24
    /// 最后修改人（包括创建人）：lijunfeng
    /// </summary>
    public sealed class QueueLoadManager : IRelease
    {
        private static QueueLoadManager _instance = null;
        private Dictionary<string, QueueLoaderDesc> m_queue;
        private int m_totalProgress = 0;//总进度
        private int m_loadedCount = 0;//当前加载完成的个数
        private float m_totalCurrProgress = 0;//总当前进度
        private Action m_onComplete = null;//总完成回调
        private QueueLoadingProgress m_onProgress = null;//总进度回调

        public static QueueLoadManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QueueLoadManager();

                return _instance;
            }
        }

        /// <summary>
        /// 提前预加载的所有资源，都需要由提供加载列表的指定脚本调用资源管理器的DiscardUsage销毁
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onComplete">全部完成回调</param>
        /// <param name="onProgress">总进度</param>
        public void Load(List<QueueLoaderDesc> list, Action onComplete, QueueLoadingProgress onProgress = null)
        {
            if (list.Count < 1)
                return;

            if (m_queue == null)
                m_queue = new Dictionary<string, QueueLoaderDesc>();

            m_onComplete = onComplete;
            m_onProgress = onProgress;
            m_loadedCount = 0;

            LoadingProgressCallback t_progress = null;

            if (onProgress != null)
                t_progress = OnProgress;

            UnityEngine.Object t_obj = null;

            foreach (var item in list)
            {
                //if (!Global.It.LocalResMgr.GetLocalResource(item.script, item.path, item.name, item.instance,item.type, out t_obj))
                //{
                //    m_queue.Add(item.path, item);
                //    Global.It.LocalResMgr.GetLocalResource(item.script, item.path, item.name, t_progress, OnComplete, item.type, item.order, item.instance, item.args);
                //}
            }

            m_totalProgress = m_queue.Count;
            list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void OnProgress(string path, string name, float progress, params string[] args)
        {
            float temp = progress - m_queue[path].progress;
            m_queue[path].progress = progress;
            m_totalCurrProgress += temp;

            if (m_onProgress != null)
                m_onProgress(m_totalCurrProgress / m_totalProgress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="arg"></param>
        private void OnComplete(string path, string name, UnityEngine.Object obj, params string[] args)
        {
            m_loadedCount++;

            if (m_loadedCount == m_totalProgress)
            {
                m_totalProgress = 0;
                m_totalCurrProgress = 0;
                m_onComplete();
                m_onComplete = null;
                m_onProgress = null;

                foreach (var item in m_queue)
                {
                    item.Value.Release();
                }

                m_queue.Clear();
            }
        }

        public void Release(bool destroy = false)
        {
            if (m_queue != null)
                m_queue.Clear();

            m_loadedCount = 0;
            m_totalProgress = 0;
            m_totalCurrProgress = 0f;
            m_onComplete = null;
            m_onProgress = null;

            if (destroy)
                _instance = null;
        }
    }
}

