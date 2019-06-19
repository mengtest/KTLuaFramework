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
		private static readonly ValueDropdownList<string> kClassNames = new ValueDropdownList<string>();
		private Assembly assembly;

		[MenuItem("LuaFramework/ScriptableObjectTools/ScriptableObjectTool")]
		private static void ShowScriptObjectEditor()
        {
            EditorWindow.GetWindow<KTCreateScriptObjectWindow>("ScriptableObjectTool").autoRepaintOnSceneChange = true;
        }

        protected override void OnEnable()
        {
			if(assembly==null)
				assembly = Assembly.Load("Assembly-CSharp");

			var dir = @"Assets\core\Assemblies\Kernel.core\ScriptableObjects";
            var files=Directory.GetFiles(dir, "*.cs", SearchOption.TopDirectoryOnly);
			files.Select(file=> Path.GetFileNameWithoutExtension(file))
				.Where(file=>
				{
					var type = assembly.GetTypes().First(t => t.Name.ToUpper() == file.ToUpper());
					return type != null && type.IsSubclassOf(typeof(ScriptableObject));
				})
				.ToList()
				.ForEach(file => kClassNames.Add(file));

			if (kClassNames.Count>0)
                scriptName = kClassNames[0].Value;
		}

        private void OnDisable()
        {
            kClassNames.Clear();
        }

        [LabelText("类名"), ValueDropdown("kClassNames")]
        public string scriptName;

        [Button("New", ButtonSizes.Medium)]
        [GUIColor(0.0f, 1.0f, 0.0f)]
        public void New()
        {
			//选择文件夹，只对Project右侧子面板的选择有效
            UnityEngine.Object obj = Selection.activeObject;
            if (obj == null)
                return;

            var sb = KTStringBuilderCache.Acquire()
            .Append(AssetDatabase.GetAssetPath(obj))
            .Append("/")
            .Append(scriptName)
            .Append(KTConfigs.kAssetExt);
            var fullpath = KTStringBuilderCache.GetStringAndRelease(sb);

            try
            {
                var type = assembly.GetTypes().First((t) =>
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