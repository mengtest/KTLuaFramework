using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using SLua;

namespace LuaFramework
{
    public class KTLuaManager
    {
        private LuaSvr m_luaServer;
        private LuaFunction m_luaUpdate;
        private LuaFunction m_luaLateUpdate;
        private LuaFunction m_FixedUpdate;
        private bool m_inited = false;
        private int m_progress = 0;

        public IEnumerator Init(string mainFile = "main")
        {
            Assert.IsNull(m_luaServer);
            Assert.IsFalse(m_inited);

            m_luaServer = new LuaSvr();
            LuaSvr.mainState.loaderDelegate = (string path, ref string absoluteFn) =>
            {
                absoluteFn = "";
                return DevLuaLoader(KTPathHelper.LuaCodePath, path);
            };
            LuaSvr.mainState.logDelegate = (string msg) =>
              {
                 //Debug.Log(msg);
              };

            LuaSvr.mainState.errorDelegate = (string msg) =>
            {
                Debug.LogError(msg);
            };

            LuaSvr.mainState.warnDelegate = (string msg) =>
            {
                Debug.LogWarning(msg);
            };

            m_luaServer.init((p) =>
            {
                m_progress = p;
            },
            () =>
            {
                m_inited = true;
            }, LuaSvrFlag.LSF_EXTLIB | LuaSvrFlag.LSF_3RDDLL | LuaSvrFlag.LSF_BASIC);

            while (!m_inited)
            {
                yield return null;
            }

            m_luaServer.start(mainFile);
            m_luaUpdate = LuaSvr.mainState.getFunction("update");
            m_luaLateUpdate = LuaSvr.mainState.getFunction("late_update");
            m_FixedUpdate = LuaSvr.mainState.getFunction("fixed_update");
        }

        public void Close()
        {
            m_luaServer = null;
            if (m_luaUpdate != null)
            {
                m_luaUpdate.Dispose();
                m_luaUpdate = null;
            }

            if (m_luaUpdate != null)
            {
                m_luaLateUpdate.Dispose();
                m_luaLateUpdate = null;
            }

            if (m_FixedUpdate != null)
            {
                m_FixedUpdate.Dispose();
                m_FixedUpdate = null;
            }

            m_inited = false;
            m_progress = 0;
            LuaSvr.mainState = null;
        }

        public void Update()
        {
            if (m_inited && m_luaUpdate != null)
            {
                m_luaUpdate.call();
            }
        }

        public void FixedUpdate()
        {
            if (m_inited && m_luaLateUpdate != null)
            {
                m_luaLateUpdate.call();
            }
        }

        public void LateUpdate()
        {
            if (m_inited && m_FixedUpdate != null)
            {
                m_FixedUpdate.call();
            }
        }

        private static byte[] DevLuaLoader(string luaDir, string path)
        {
            var sb = KTStringBuilderCache.Acquire()
            .Append(luaDir)
            .Append('/')
            .Append(path)
            .Replace('.', '/')
            .Append(KTConfigs.kLuaExt);
            var fullpath = KTStringBuilderCache.GetStringAndRelease(sb);
            if (File.Exists(fullpath))
            {
                return File.ReadAllBytes(fullpath);
            }
            else
            {
                Debug.LogErrorFormat("Lua 路径不对{0}", path + ":" + fullpath);
            }

            return null;
        }
    }
}