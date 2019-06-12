using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace LuaFramework
{
    public class KTPbTools
    {
        /// <summary>
        /// 选择.proto文件，导出对应的c++,lua文件到当前目录,针对pb2.0
        /// </summary>
        [MenuItem("LuaFramework/PbTools/BuildSelectedPb2FileToLua")]
        private static void BuildSelectedPb2FileToLua()
        {
            Object obj = Selection.activeObject;
            if (obj == null)
                return;

            string path = AssetDatabase.GetAssetPath(obj).Replace("Assets/", "");
            string fullPath =Path.Combine(Application.dataPath,path);

            string protoc = @"E:\UnityWorkspace\protobuf-2.4.1\vsprojects\Debug\protoc.exe";
            string protoc_gen_dir = "\"E:/UnityWorkspace/protoc-gen-lua-master/plugin/build.bat\"";

            string ext = Path.GetExtension(fullPath);
            if (!ext.Equals(".proto"))
                return;

            string name = Path.GetFileName(fullPath);
            string dir = fullPath.Replace(name, "");

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = protoc;
            info.Arguments = "--cpp_out=./ -o "+name.Replace(".proto",".pb")+" "+name;
            //info.Arguments = "--cpp_out=./ --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;
            info.ErrorDialog = true;
            Util.Log(info.FileName + " " + info.Arguments);

            Process pro = Process.Start(info);
            pro.WaitForExit();

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 选择所有.proto文件所在的目录，导出所有对应的c++,lua文件到当前目录,针对pb2.0
        /// </summary>
        [MenuItem("LuaFramework/PbTools/BuildSelectedPb2FilesToLua")]
        private static void BuildSelectedPb2FilesToLua()
        {
            Object obj = Selection.activeObject;
            if (obj == null) return;

            string path = AssetDatabase.GetAssetPath(obj).Replace("Assets/", "");
            string selectedDir = Path.Combine(Application.dataPath, path);
            List<string> files = new List<string>();
            Recursive(selectedDir, files);

            string protoc = @"E:\UnityWorkspace\protobuf-2.4.1\vsprojects\Debug\protoc.exe";
            string protoc_gen_dir = "\"E:/UnityWorkspace/protoc-gen-lua-master/plugin/build.bat\"";
            files.ForEach(file =>
            {
                string ext = Path.GetExtension(file);
                if (!ext.Equals(".proto"))
                    return;

                string name = Path.GetFileName(file);
                string dir = file.Replace(name, "");

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = protoc;
                info.Arguments = "--cpp_out=./ -o " + name.Replace(".proto", ".pb") + " " + name;
                //info.Arguments = "--cpp_out=./ --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.UseShellExecute = true;
                info.WorkingDirectory = dir;
                info.ErrorDialog = true;
                UnityEngine.Debug.Log(info.FileName + " " + info.Arguments);

                Process pro = Process.Start(info);
                pro.WaitForExit();
            });
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        private static void Recursive(string path, List<string> result)
        {
            var names = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            var files = names.ToList().Where(filename =>
            {
                return !filename.EndsWith(".meta");
            })
            .Select(filename =>
            {
                return filename.Replace('\\', '/');
            });

            result.AddRange(files);
            dirs.ToList().ForEach(dir => Recursive(path, result));
        }
    }
}
