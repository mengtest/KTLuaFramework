using LuaFramework;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class KTCreateECSClassWindow : OdinEditorWindow
{
	[MenuItem("LuaFramework/ECSTools")]
	private static void ShowScriptObjectEditor()
	{
		EditorWindow.GetWindow<KTCreateECSClassWindow>("KTCreateECSClassWindow").autoRepaintOnSceneChange = true;
	}

	private static readonly ValueDropdownList<string> kBaseClassNames = new ValueDropdownList<string>()
	{
		"",
		"BaseSystem",
		"ECSEntity"
	};

	private static readonly ValueDropdownList<string> kInterfaceNames = new ValueDropdownList<string>()
	{
		"",
		"IComponent",
		"IComponentData<T>",
		"ITick",
		"IFixedTick",
		"ILateTick"
	};

	private static readonly ValueDropdownList<string> kNamespaces = new ValueDropdownList<string>()
	{
		"",
		"System",
		"System.Collections",
		"System.Collections.Generic",
		"UnityEngine",
		"Kernel.core"
	};

	[LabelText("类名")]
	public string className;

	[LabelText("继承父类"), ValueDropdown("kBaseClassNames")]
	public string baseClassName;

	[LabelText("实现接口"), ValueDropdown("kInterfaceNames")]
	public List<string> interfaceNames=new List<string>();

	[LabelText("包含的命名空间"), ValueDropdown("kNamespaces")]
	public List<string> usingNamespaceList = new List<string>()
	{
		"System",
		"System.Collections",
		"System.Collections.Generic",
		"UnityEngine",
		"Kernel.core"
	};

	[LabelText("命名空间")]
	public string selfNamespace = "Kernel.game";

	[Button("generate", ButtonSizes.Medium)]
	[GUIColor(0.0f, 1.0f, 0.0f)]
	public void New()
	{
		//选择文件夹，只对Project右侧子面板的选择有效
		var obj = Selection.activeObject;
		if (obj == null)
			return;

		if(string.IsNullOrEmpty(className))
		{
			Debug.LogError("Class Name Empty");
			return;
		}

		var sb = KTStringBuilderCache.Acquire()
		.Append(AssetDatabase.GetAssetPath(obj))
		.Append("/")
		.Append(className)
		.Append(".cs");
		var fullpath = KTStringBuilderCache.GetStringAndRelease(sb);

		using (var fs = new FileStream(fullpath, FileMode.Create))
		{
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				CollectNameSpaces();
				foreach(var item in usingNamespaceList)
				{
					WriteUsingNamespace(sw, item);
				}

				sw.WriteLine();

				if (string.IsNullOrEmpty(selfNamespace))
				{
					OneTab = "";
					TwoTab = "\t";
				}
				else
				{
					OneTab = "\t";
					TwoTab = "\t\t";
				}

				WriteSelfNamespace(sw, selfNamespace, () =>
				{
					WriteClass(sw, className, baseClassName, interfaceNames);
				});

				sw.Flush();
				sw.Close();
			}

			fs.Close();
		};

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private string OneTab;
	private string TwoTab;
	private Assembly assembly;

	private void CollectNameSpaces()
	{
		var baselist = new List<string>(interfaceNames);
		if (!string.IsNullOrEmpty(baseClassName))
			baselist.Insert(0, baseClassName);

		if (assembly == null)
			assembly = Assembly.Load("Assembly-CSharp");

		for (int i = 0; i < baselist.Count; ++i)
		{
			var baseType = assembly.GetTypes().First(t => t.Name.ToUpper() == baselist[i].ToUpper());
			if(!usingNamespaceList.Contains(baseType.Namespace))
				usingNamespaceList.Add(baseType.Namespace);
		}
	}

	private void WriteUsingNamespace(StreamWriter sw, string usingNamespace)
	{
		sw.Write(string.Format("using {0};\r\n", usingNamespace));
	}

	private void WriteSelfNamespace(StreamWriter sw, string selfNamespace,Action bodyAction = null)
	{
		if(!string.IsNullOrEmpty(selfNamespace))
		{
			sw.Write(string.Format("namespace {0}\r\n{1}\r\n", selfNamespace,"{"));
			bodyAction?.Invoke();
			sw.Write("}\r\n");
		}
		else
		{
			bodyAction?.Invoke();
		}
	}

	private void WriteClass(StreamWriter sw, string className,string baseClassName,List<string> interfaceNames, Action bodyAction=null)
	{
		var baselist = new List<string>(interfaceNames);
		if(!string.IsNullOrEmpty(baseClassName))
			baselist.Insert(0, baseClassName);

		var extendSign = baselist.Count > 0 ? ":" : string.Empty;
		sw.Write(string.Format("{0}public class {1}{2}", OneTab,className,extendSign));

		for(int i=0;i< baselist.Count;++i)
		{
			if(i==0)
			{
				sw.Write(baselist[i]);
			}
			else
			{
				sw.Write(","+baselist[i]);
			}
		}

		sw.Write("\r\n"+ OneTab + "{\r\n");

		if (!string.IsNullOrEmpty(baseClassName))
		{
			var baseClassType = assembly.GetTypes().First(t => t.Name.ToUpper() == baseClassName.ToUpper());
			if (baseClassType == null)
			{
				Debug.LogErrorFormat("Base Class {0} Not Found", baseClassName);
				return;
			}

			//构造函数
			var constructorInfo = baseClassType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
			constructorInfo.ToList().ForEach(c =>
			{
				if (!c.IsPublic)
					return;

				var parametersInfo = c.GetParameters();
				var paramsStrs = c.GetParameters().Select(p =>
				{
					string str = "";
					str += CovertType(p.ParameterType);
					str += " ";
					str += p.Name;
					return str;
				});
				var paramsStr = string.Join(",", paramsStrs);

				var baseParamsStrs = parametersInfo.Select(p => p.Name);
				var baseParamsStr = string.Join(",", baseParamsStrs);

				var head = string.Format("{0}public {1}({2}):base({3})", TwoTab, className, paramsStr, baseParamsStr);
				var body = string.Format("\r\n{0}{1}\r\n{2}{3}\r\n", TwoTab, "{", TwoTab, "}");
				sw.Write(head + body);
			});

			sw.WriteLine();
		}

		//实现接口
		interfaceNames.ForEach(interfaceName =>
		{
			var match=Regex.Match(interfaceName, ".*<(.*)>.*");
			if(match.Groups.Count>1)
			{
				Debug.Log("Generics are not supported");
				return;
			}

			var interfaceType = assembly.GetTypes().First(t => t.Name.ToUpper() == interfaceName.ToUpper());
			if (interfaceType == null)
			{
				Debug.LogErrorFormat("Interface {0} Not Found", interfaceName);
				return;
			}

			var methodsInfo=interfaceType.GetMethods();
			methodsInfo.ToList().ForEach(m =>
			{
				var parametersInfo = m.GetParameters();
				var paramsStrs = m.GetParameters().Select(p =>
				{
					string str = "";
					str += CovertType(p.ParameterType);
					str += " ";
					str += p.Name;
					return str;
				});
				var paramsStr = string.Join(",", paramsStrs);

				var head = string.Format("{0}public {1} {2}({3})", TwoTab, CovertType(m.ReturnType), m.Name, paramsStr);
				var body = string.Format("\r\n{0}{1}\r\n{2}{3}\r\n", TwoTab, "{", TwoTab, "}");
				sw.Write(head + body);
			});
		});

		bodyAction?.Invoke();
		sw.Write(OneTab+"}\r\n");
	}

	private string CovertType(Type type)
	{
		var typeName = type.Name;
		switch (typeName)
		{
			case "Void":
				return "void";
			case "Object":
				return "object";
			case "Boolean":
				return "bool";
			case "String":
				return "string";
			case "Char":
				return "char";

			case "Decimal":
				return "decimal";
			case "Single":
				return "float";
			case "Double":
				return "double";

			case "Byte":
				return "byte";
			case "Int16":
				return "short";
			case "Int32":
				return "int";
			case "Int64":
				return "long";

			case "SByte":
				return "sbyte";
			case "UInt16":
				return "ushort";
			case "UInt32":
				return "uint";
			case "UInt64":
				return "ulong";

			default:
				return typeName;
		}
	}
}