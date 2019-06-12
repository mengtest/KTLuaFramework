using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using Octodiff;

namespace LuaFramework
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    public class KTHotUpdateManager : MonoBehaviour, IRelease
    {
        public enum UpdatePhase
        {
            DeltaExtractFile,//第一次释放资源到persistentDataPath
            DeltaExtractSrc,
            DeltaUpdate,
            DeltaDownloadNewFile,
            DownloadDelta,
            PitchFile,
            UnZipFile,
            UpdateOver,
            ExtractFile,
            ExtractSrc,
            NormalUpdate,
            DownloadNewFile,
            DownloadSrc,
        }

        public static KTHotUpdateManager Instance { get; private set; }
        public Action hotUpdateComplete = null;//热更新完成回调
        public Action hotUpdateFail = null;//热更新失败回调
        public Action<float> hotUpdateProgress = null;//热更新进度

        private List<string> m_fileToDelete = null;//需要删除的文件
        private List<string> m_fileToCreate = null;//需要创建的文件
        private UpdatePhase m_updatPhase= UpdatePhase.DeltaExtractFile;
        private byte[] newFileData;

        private void Awake()
        {
            Instance = this;
        }

        private void InitUpdate()
        {
            m_fileToDelete = new List<string>();
            m_fileToCreate = new List<string>();
        }

        /// <summary>
        /// 检查解包和热更新
        /// </summary>
        public void CheckExtractAndUpdate()
        {
            bool isExists = Directory.Exists(KTPathHelper.DataPath) && File.Exists(KTPathHelper.DataPath + "files.txt");

            if(KTConfigs.kIncrementalmMode)
            {
                if(isExists)
                    StartCoroutine(DeltaUpdateResource());//启动增量更新
                else
                    StartCoroutine(DeltaExtractResource());//启动释放协成 
            }

            if (isExists)
            {
                if (KTConfigs.kIncrementalmMode)
                    StartCoroutine(DeltaUpdateResource());
                else
                    StartCoroutine(UpdateResource());

                return;//文件已经解压过了，自己可添加检查文件列表逻辑
            }
        }

        /// <summary>
        /// 第一次运行前，在编辑器模式下，需要把src.zip和files文件从NewVersion目录手动拷贝到streamingAssetsPath目录中
        /// 第一次运行游戏，增量更新模式，把资源从streamingAssetsPath目录拷贝到Application.persistentDataPath目录
        /// 首先要把src.zip拷贝到persistentDataPath/Temp目录下
        /// 再把src.zip解压到Application.persistentDataPath下
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeltaExtractResource()
        {
            this.m_updatPhase = UpdatePhase.DeltaExtractFile;
            //拷贝files.txt到persistentDataPath目录
            string srcPath = KTPathHelper.AppContentPath();//游戏包资源释放和加载目录
            string dstPath = KTPathHelper.DataPath;//发布后的游戏数据存储目录
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath, true);

            Directory.CreateDirectory(dstPath);

            string infile = srcPath + "files.txt";
            string outfile = dstPath + "files.txt";
            yield return CopyFile(infile, outfile);

            this.m_updatPhase = UpdatePhase.DeltaExtractSrc;
            if (!Directory.Exists(dstPath + "Temp"))
                Directory.CreateDirectory(dstPath + "Temp");

            //拷贝src.zip到persistentDataPath目录
            infile = srcPath +KTConfigs.kDeltaName;
            outfile = dstPath + "Temp/"+ KTConfigs.kDeltaName;
            yield return CopyFile(infile, outfile);

            Debug.Log("解压"+ KTConfigs.kDeltaName);
            ZipTool.unZipFile(outfile, dstPath);//同步操作 需要时间
            //if (ZipTool.inputStream.Available > 0)
            //    yield return new WaitForEndOfFrame();
            Debug.Log("解包完成!!!");
            yield return DeltaUpdateResource();
        }

        /// <summary>
        /// 拷贝文件
        /// todo:有可能错误，待处理
        /// </summary>
        /// <param name="infile">输入目录</param>
        /// <param name="outfile">输出目录</param>
        /// <returns></returns>
        private IEnumerator CopyFile(string infile,string outfile)
        {
            if (File.Exists(outfile))
                File.Delete(outfile);

            var sb = KTStringBuilderCache.Acquire()
            .Append("正在解包文件:>")
            .Append(":")
            .Append(infile)
            .Append(":")
            .Append(outfile);
            Debug.Log(KTStringBuilderCache.GetStringAndRelease(sb));

            if (Application.platform == RuntimePlatform.Android)
            {
                yield return KTDownloadHelper.WWWDownloadRequest(infile,
                (www) =>
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                ,(www) =>
                {
                    Debug.LogError("拷贝释放"+Path.GetFileName(infile) + "文件错误");
                });
            }
            else
            {
                if (File.Exists(outfile))
                    File.Delete(outfile);
        
                File.Copy(infile, outfile, true);
            }

            yield return new WaitForEndOfFrame();//等一帧
        }

        /// <summary>
        /// 增量更新
        /// 下载files文件到Application.persistentDataPath下,对比本地files文件判断是否需要更新，如果有
        /// 下载delta文件到persistentDataPath/Temp目录下
        /// 组合delta文件和src.zip文件到persistentDataPath目录下，名称为src.zip
        /// 把新的src.zip文件拷贝到persistentDataPath/Temp目录下，覆盖掉原来的src.zip
        /// 解压新的Temp/src.zip到persistentDataPath目录下
        /// 把下载的新files文件覆盖掉原来的files
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeltaUpdateResource()
        {
            this.m_updatPhase = UpdatePhase.DeltaUpdate;
            InitUpdate();
            //下载新file
            string dataPath = KTPathHelper.DataPath;
            var sb = KTStringBuilderCache.Acquire()
            .Append("file://")
            .Append(KTPathHelper.AppContentPath())
            .Append("files.txt");

            var sb2 = KTStringBuilderCache.Acquire()
            .Append(KTConfigs.kWebUrl)
            .Append("files.txt?v=")
            .Append(DateTime.Now.ToString("yyyymmddhhmmss"));

            string fileUrl = KTConfigs.kDebugMode ? KTStringBuilderCache.GetStringAndRelease(sb) : KTStringBuilderCache.GetStringAndRelease(sb2);
            this.m_updatPhase = UpdatePhase.DeltaDownloadNewFile;
            Debug.Log("LoadUpdate---->>>" + fileUrl);

            yield return KTDownloadHelper.WWWDownloadRequest(fileUrl,
            (www) =>
            {
                www.bytes.CopyTo(newFileData, 0);

                var newFileLines = www.text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var oldFileLines = File.ReadAllLines(dataPath + "files.txt");
                ExecuteDownloadTask(newFileLines, oldFileLines);

                if (m_fileToCreate.Count == 0)
                {
                    Debug.Log("没有可更新资源");
                    if (hotUpdateComplete != null)
                        hotUpdateComplete();

                    return;
                }

                //创建加载任务
                string deltaFullPath = dataPath + "Temp/" + KTConfigs.kDeltaName;
                if (File.Exists(deltaFullPath))
                    File.Delete(deltaFullPath);

                sb = KTStringBuilderCache.Acquire()
                .Append("file://")
                .Append(KTPathHelper.AppContentPath())
                .Append(KTConfigs.kDeltaName);

                sb2 = KTStringBuilderCache.Acquire()//此处因为暂定只需要下载一个zip文件，所以不需要从m_fileToCreate拿到具体文件名，但当多zip分包时就必须遍历m_fileToCreate下载了
                .Append(KTConfigs.kWebUrl)
                .Append(KTConfigs.kDeltaName)
                .Append("?v=")
                .Append(DateTime.Now.ToString("yyyymmddhhmmss"));

                string deltaUrl = KTConfigs.kDebugMode ? KTStringBuilderCache.GetStringAndRelease(sb) : KTStringBuilderCache.GetStringAndRelease(sb2);
                this.m_updatPhase = UpdatePhase.DownloadDelta;
                Debug.Log("LoadUpdate---->>>" + deltaUrl);

                StartCoroutine(KTDownloadHelper.WWWDownloadRequest(deltaUrl, (www2) =>
                 {
                     File.WriteAllBytes(deltaFullPath, www2.bytes);
                     StartCoroutine(PitchFile());
                 }
                 ,(www2) =>
                 {
                     StartCoroutine(UpdateFail("下载新src.zip.delta文件失败"));
                 }));
            }
            , (www) =>
            {
                StartCoroutine(UpdateFail("下载新file.txt文件失败"));
            });
        }

        /// <summary>
        /// 组合打包文件
        /// </summary>
        /// <returns></returns>
        private IEnumerator PitchFile()
        {
            this.m_updatPhase = UpdatePhase.PitchFile;

            string fileName = "Temp/"+KTConfigs.kSrcName;
            string deltaName = "Temp/"+KTConfigs.kDeltaName;

            Debug.Log("组合补丁文件");
            Directory.SetCurrentDirectory(KTPathHelper.DataPath);
            OctodiffUtil.ExecutePitch(fileName, deltaName, KTConfigs.kSrcName);

            //把新的src.zip文件拷贝到persistentDataPath/Temp目录下，覆盖掉原来的src.zip
            if (File.Exists(fileName))
                File.Delete(fileName);

            if (File.Exists(KTConfigs.kSrcName))
                File.Move(KTConfigs.kSrcName, fileName);

            //删掉除Temp目录之外的所有旧资源，目录直接删掉，文件不需要删，会直接覆盖
            Directory.Delete(KTPathHelper.DataPath + "BuildRes", true);
            Directory.Delete(KTPathHelper.DataPath + "lua", true);
            Directory.SetCurrentDirectory(Application.dataPath.Replace("/Assets", ""));

            Debug.Log("解压src.zip");
            this.m_updatPhase = UpdatePhase.UnZipFile;
            ZipTool.unZipFile(KTPathHelper.DataPath + fileName, KTPathHelper.DataPath);//需要时间
            yield return UpdateComplete();
        }

        /// <summary>
        /// 非增量热更新，第一次运行释放文件
        /// 第一次运行，把Application.streamingAssetsPath内的文件拷贝到Application.persistentDataPath目录
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExtractResource()
        {
            this.m_updatPhase = UpdatePhase.ExtractFile;
            //拷贝files.txt到persistentDataPath目录
            string srcPath = KTPathHelper.AppContentPath();//游戏包资源释放和加载目录
            string dstPath = KTPathHelper.DataPath;//发布后的游戏数据存储目录
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath, true);

            Directory.CreateDirectory(dstPath);

            string infile = srcPath + "files.txt";
            string outfile = dstPath + "files.txt";
            yield return CopyFile(infile, outfile);

            this.m_updatPhase = UpdatePhase.ExtractSrc;
            //拷贝所有资源到persistentDataPath目录,释放所有文件到数据目录
            string[] files = File.ReadAllLines(outfile);
            foreach (var file in files)
            {
                string[] fs = file.Split('|');
                infile = srcPath + fs[0];
                outfile = dstPath + fs[0];
                Debug.Log("正在解包文件:>" + fs[0]);

                string dir = Path.GetDirectoryName(outfile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                yield return CopyFile(infile, outfile);
            }

            Debug.Log("解包完成!!!");
            yield return UpdateResource();
        }

        /// <summary>
        /// 普通更新
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateResource()
        {
            this.m_updatPhase = UpdatePhase.NormalUpdate;
            InitUpdate();
            //下载新file
            string dataPath = KTPathHelper.DataPath;
            var sb = KTStringBuilderCache.Acquire()
            .Append("file://")
            .Append(KTPathHelper.AppContentPath())
            .Append("files.txt");

            var sb2 = KTStringBuilderCache.Acquire()
            .Append(KTConfigs.kWebUrl)
            .Append("files.txt?v=")
            .Append(DateTime.Now.ToString("yyyymmddhhmmss"));

            string fileUrl = KTConfigs.kDebugMode ? KTStringBuilderCache.GetStringAndRelease(sb) : KTStringBuilderCache.GetStringAndRelease(sb2);
            this.m_updatPhase = UpdatePhase.DeltaDownloadNewFile;
            Debug.Log("LoadUpdate---->>>" + fileUrl);

            //下载file
            this.m_updatPhase = UpdatePhase.DownloadNewFile;
            yield return KTDownloadHelper.WWWDownloadRequest(fileUrl,
            (www) =>
            {
                www.bytes.CopyTo(newFileData, 0);

                string[] newFileLines = www.text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                string[] oldFileLines = File.ReadAllLines(dataPath + "files.txt");

                ExecuteDownloadTask(newFileLines, oldFileLines);

                if (m_fileToCreate.Count == 0)
                {
                    Debug.Log("没有可更新资源");
                    if (hotUpdateComplete != null)
                        hotUpdateComplete();

                    return;
                }

                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);

                ExecuteDownloadTask(newFileLines, oldFileLines);
                //删掉多余的旧资源
                for (int i = 0, j = m_fileToDelete.Count; i < j; i++)
                {
                    string localfile = (dataPath + m_fileToDelete[i]).Trim();
                    if (File.Exists(localfile))
                        File.Delete(localfile);
                }
                //递归删除BuildRes下的所有空文件夹
                DeleteDir(dataPath + "BuildRes");
                //创建加载任务
                for (int i = 0, j = m_fileToCreate.Count; i < j; i++)
                {
                    string fileFullPath = (dataPath + m_fileToCreate[i]).Trim();
                    if (File.Exists(fileFullPath))
                        File.Delete(fileFullPath);

                    string path = Path.GetDirectoryName(fileFullPath);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    sb = KTStringBuilderCache.Acquire()
                    .Append("file://")
                    .Append(KTPathHelper.AppContentPath())
                    .Append(m_fileToCreate[i]);

                    sb2 = KTStringBuilderCache.Acquire()
                    .Append(KTConfigs.kWebUrl)
                    .Append(m_fileToCreate[i])
                    .Append("?v=")
                    .Append(DateTime.Now.ToString("yyyymmddhhmmss"));

                    string srcUrl = KTConfigs.kDebugMode ? KTStringBuilderCache.GetStringAndRelease(sb) : KTStringBuilderCache.GetStringAndRelease(sb2);
                    KTUpdateLoaderManager.It.Add(srcUrl, fileFullPath);

                    //StartCoroutine(KTDownloadHelper.WWWDownloadRequest(srcUrl, (www2) =>
                    //{
                    //    File.WriteAllBytes(fileFullPath, www2.bytes);
                    //}
                    //,(www2) =>
                    //{
                    //    StartCoroutine(UpdateFail("下载新"+ m_fileToCreate[i] + "文件失败"));
                    //}));

                    //StartCoroutine(UpdateComplete());
                }
            }
            ,(www) =>
            {
                StartCoroutine(UpdateFail("下载新file.txt文件失败"));
            });

            this.m_updatPhase = UpdatePhase.DownloadSrc;
            yield return KTUpdateLoaderManager.It.StartLoadAsync((data) =>
            {
                StartCoroutine(UpdateComplete());
            },null,(msg)=>
            {
                StartCoroutine(UpdateFail("下载新" + msg + "文件失败"));
            });
        }

        /// <summary>
        /// 所有文件下载解压完比
        /// </summary>
        private IEnumerator UpdateComplete()
        {
            //覆盖旧的files文件,所有文件都更新成功才覆盖files文件，以便在更新失败后重新进入游戏可以重新更新
            Debug.Log("覆盖files");
            File.WriteAllBytes(KTPathHelper.DataPath + "files.txt", newFileData);
            newFileData = null;
            Debug.Log("更新完成!!");
            if (hotUpdateComplete != null)
                hotUpdateComplete();

            this.m_updatPhase = UpdatePhase.UpdateOver;
            yield break;
        }

        private IEnumerator UpdateFail(string file)
        {
            Debug.Log("更新失败!>" + file);
            if (hotUpdateFail != null)
                hotUpdateFail();

            yield break;
        }

        /// <summary>
        /// 筛选出要创建的文件和要删除的文件
        /// </summary>
        /// <param name="fileLines"></param>
        /// <param name="oldFileLines"></param>
        private void ExecuteDownloadTask(string[] fileLines, string[] oldFileLines)
        {
            var fileMap = new Dictionary<string, string>();
            var oldFileMap = new Dictionary<string, string>();
            for (int i = 0, j = fileLines.Length; i < j; i++)
            {
                string key = fileLines[i].Split('|')[0];
                fileMap.Add(key, fileLines[i]);
            }

            for (int i = 0, j = oldFileLines.Length; i < j; i++)
            {
                string[] kv = oldFileLines[i].Split('|');
                oldFileMap.Add(kv[0], kv[1]);
            }

            //筛选要删除的文件
            for (int i = 0, j = oldFileLines.Length; i < j; i++)
            {
                string key = oldFileLines[i].Split('|')[0];

                if (!fileMap.ContainsKey(key))
                    m_fileToDelete.Add(key);
            }

            //筛选要创建的文件
            for (int i = 0, j = fileLines.Length; i < j; i++)
            {
                string[] kv = fileLines[i].Split('|');

                if (oldFileMap.ContainsKey(kv[0]))
                {
                    if (!oldFileMap[kv[0]].Equals(kv[1]))
                    {
                        m_fileToDelete.Add(kv[0]);
                        m_fileToCreate.Add(kv[0]);
                    }
                }
                else
                {
                    m_fileToCreate.Add(kv[0]);
                }
            }
        }

        /// <summary>
        /// 递归删除BuildRes下的所有空文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteDir(string dir)
        {
            //判断是否需要删除所在文件夹
            var dirs = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
            for (int i = 0, j = dirs.Length; i < j; i++)
            {
                if (Directory.GetFiles(dirs[0]).Length == 0)
                    Directory.Delete(dirs[0]);
            }
        }

        public void Release(bool destroy = false)
        {
            if (m_fileToDelete != null)
            {
                m_fileToDelete.Clear();
                m_fileToDelete = null;
            }
            if (m_fileToCreate != null)
            {
                m_fileToCreate.Clear();
                m_fileToCreate = null;
            }

            hotUpdateComplete = null;
            hotUpdateFail = null;
            newFileData = null;
        }
    }
}