using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace LuaFramework
{
    [SLua.CustomLuaClass]
    public class KTGameManager : MonoBehaviour, IRelease
    {
        public static KTGameManager Instance { get; private set; }
        private KTLuaManager m_luaMgr;

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Assert.IsNull(Instance);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = KTConfigs.kGameFrameRate;
            m_luaMgr = new KTLuaManager();
        }

        private void Start()
        {
            if (KTConfigs.kUpdateMode)
            {
                KTHotUpdateManager.Instance.hotUpdateComplete = () => StartGame();
                KTHotUpdateManager.Instance.hotUpdateFail = () => Application.Quit();
                KTHotUpdateManager.Instance.CheckExtractAndUpdate();
            }
            else
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            StartCoroutine(DoRestart());
        }

        IEnumerator DoRestart()
        {
            Debug.Log("Restart begin");
            m_luaMgr.Close();
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //资源管理器加载并缓存所有必要的资源配置，方便lua使用
            yield return m_luaMgr.Init();
            Debug.Log("Restart end");
        }

        void Update()
        {
            if (m_luaMgr != null)
            {
                m_luaMgr.Update();
            }
        }

        void FixedUpdate()
        {
            if (m_luaMgr != null)
            {
                m_luaMgr.FixedUpdate();
            }
        }

        void LateUpdate()
        {
            if (m_luaMgr != null)
            {
                m_luaMgr.LateUpdate();
            }
        }

        void OnApplicationQuit()
        {
            Assert.IsNotNull(Instance);
            Instance = null;
            m_luaMgr.Close();
            m_luaMgr = null;
        }

        public void Release(bool destroy = false)
        {
            
        }
    }
}