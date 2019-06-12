using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using System.IO;

namespace LuaFramework
{
    /// <summary>
    /// 创建配置文件
    /// </summary>
    public class KTCreateScriptObjectWindow : OdinEditorWindow
    {
        [MenuItem("LuaFramework/ScriptableObjectTools/ScriptableObjectTool")]
        public static void ShowScriptObjectEditor()
        {
            EditorWindow.GetWindow<KTCreateScriptObjectWindow>("ScriptableObjectTool").autoRepaintOnSceneChange = true;
        }

        protected override void OnEnable()
        {
            var dir = "Assets/Scripts/ScriptableObjects";
            var files=Directory.GetFiles(dir, "*.cs", SearchOption.TopDirectoryOnly);
            files.ToList<string>().ForEach(file => kClassNames.Add(Path.GetFileNameWithoutExtension(file)));

            if(kClassNames.Count>0)
                scriptName = kClassNames[0].Value;
        }

        private void OnDisable()
        {
            kClassNames.Clear();
        }

        public static ValueDropdownList<string> kClassNames = new ValueDropdownList<string>();

        [LabelText("类名"), ValueDropdown("kClassNames")]
        public string scriptName;

        [Button("New", ButtonSizes.Medium)]
        [GUIColor(0.0f, 1.0f, 0.0f)]
        public void New()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj == null)
                return;

            var sb = KTStringBuilderCache.Acquire()
            .Append(AssetDatabase.GetAssetPath(obj))
            .Append("/")
            .Append(scriptName)
            .Append(KTConfigs.kAssetExt);
            var fullpath = KTStringBuilderCache.GetStringAndRelease(sb);
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                var type = this.GetType().Assembly.GetTypes().First((t) =>
                {
                    return t.Name.ToUpper() == scriptName.ToUpper();
                });

                if (type != null)
                {
                    ScriptableObject scriptObj = ScriptableObject.CreateInstance(type);
                    AssetDatabase.CreateAsset(scriptObj, fullpath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("需要创建的ScriptableObject类型错误");
                }
            }
            catch(Exception e)
            {
                Debug.LogWarning("需要创建的ScriptableObject类型错误");
            }

        }
    }
}