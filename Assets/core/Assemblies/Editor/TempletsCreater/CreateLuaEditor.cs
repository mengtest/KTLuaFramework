#region 模块信息
// **********************************************************************
// Copyright (C) CHANGIT(厦门千骥)
//
// 文件名(File Name):             CreateLuaEditor.cs
// 作者(Author):                  吴肖牧
// 创建时间(CreateTime):           2017/5/2 10:31:49
// 修改者列表(modifier):
// 模块描述(Module description):  创建lua脚本，选中Panel的预设，右键创建对应的lua脚本，已将ui层移植到Ctrl层，ui层脚本暂时保留着
// **********************************************************************
#endregion

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class CreateLuaEditor : Editor
{

    static string Str_Panel = @"local transform;
local gameObject;

Panel = {};
local this = Panel;

--==============================--
--desc:绑定对象组件
--==============================--
function Panel.Awake(obj)
	gameObject = obj;
    transform = obj.transform;
	this.Binding();
    this.Init();
end

--==============================--
--desc:Awake之后会自己调用，初始化写在这里
--==============================--
function Panel.Start()
	-- body
end

--==============================--
--desc:删除界面，自己会调用
--==============================--
function Panel.OnDestroy()
	logWarn('OnDestroy---->>>');
end

--==============================--
--desc:初始化绑定控件
--==============================--
function Panel.Binding()
	this.transform = transform;
    --TODO
    --this.test = transform:FindChild('test/test').gameObject:GetComponent('Text');
end

--==============================--
--desc:申明自定义变量和初始化
--==============================--
function Panel.Init()
	-- TODO
    this.test = nil  --申明自定义全局变量
end

--============================== 以上方法由代码生成，请根据需要填充 ==============================--

--============================== 逻辑代码 Start ==============================--


--============================== 逻辑代码 End ==============================--

"
;

    static string Str_Ctrl = @"thisCtrl = {};
local this = thisCtrl;

local crtl;
local transform;
local gameObject;

--==============================--
--desc:构建函数
--==============================--
function thisCtrl.New()
	return this;
end

--==============================--
--desc:创建界面
--==============================--
function thisCtrl.Awake()
	UITool:CreatePanel('parent','panel', PanelType.MainPanel, this.OnCreate);
end

--==============================--
--desc:绑定对象组件，注册事件，初始化
--==============================--
function thisCtrl.OnCreate(obj)
    gameObject = obj;
	transform = obj.transform;
	crtl = transform:GetComponent('LuaBehaviour');
    this.BindComponent();            --绑定组件
    this.RegisterClickEvent();       --注册点击事件
    this.RegisterListenerEvent();    --注册监听事件
    this.Init();                     --初始化
end

--==============================--
--desc:绑定组件
--==============================--
function thisCtrl.BindComponent()
    --this.test = transform:FindChild('test/test').gameObject:GetComponent('Text');
end

--==============================--
--desc:注册点击事件
--==============================--
function thisCtrl.RegisterClickEvent()
    -- crtl:AddClick(panel.CloseBtn,{}, this.Close);
end

--==============================--
--desc:注册监听事件
--==============================--
function thisCtrl.RegisterListenerEvent()
    -- Event.AddListener(Protocal.Connect, this.OnConnect); 
end

--==============================--
--desc:注销监听事件
--==============================--
function thisCtrl.RemoveListenerEvent()
    -- Event.RemoveListener(Protocal.Connect); 
end

--==============================--
--desc:打开界面
--==============================--
function thisCtrl.Show()
    UITool:GetCtrl(thisCtrl);
end

--==============================--
--desc:关闭界面
--==============================--
function thisCtrl.Close()
    -- TODO
    this.Release();
    this.RemoveListenerEvent()
	UITool:ClosePanel(gameObject)
    UITool:RemoveCtrl(thisCtrl);
end

--==============================--
--desc:释放
--==============================--
function LoginCtrl.Release()
    -- TODO
end

--==============================--
--desc:自定义变量和初始化
--==============================--
function thisCtrl.Init()
	-- TODO
    this.test = nil  --自定义全局变量
end

--============================== 以上方法由代码生成，请根据需要填充 ==============================--

--============================== 逻辑代码 Start ==============================--


--============================== 逻辑代码 End ==============================--

"
;

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="file_path">路径</param>
    /// <param name="file_name">脚本名字</param>
    /// <param name="str_info">lua模版的字符串</param>
    static void WriteFileByLine(string file_path, string file_name, string str_info)
    {
        StreamWriter sw = null;
        if (!File.Exists(file_path + "//" + file_name))
        {
            sw = File.CreateText(file_path + "//" + file_name);//创建一个用于写入 UTF-8 编码的文本  
            Debug.Log(file_name + "文件创建成功！");
        }
        else
        {
            sw = File.AppendText(file_path + "//" + file_name);//打开现有 UTF-8 编码文本文件以进行读取  
            Debug.Log(file_name + "文件重写成功！");
        }
        Debug.Log(file_path + "/" + file_name);
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close();
        sw.Dispose();//文件流释放 
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 创建LuaPanel脚本
    /// </summary>
    [MenuItem("Assets/Create Lua Panel")]
    static public void CreateLuaPanel()
    {
        if (Selection.gameObjects.Length != 1)
        {
            return;
        }
        string name = Selection.gameObjects[0].name;
        string Info = Str_Panel.Replace("Panel", name);
        WriteFileByLine(Application.dataPath, name + ".lua", Info);
    }

    /// <summary>
    /// 创建LuaCtrl脚本
    /// </summary>
    [MenuItem("Assets/Create Lua Ctrl")]
    static public void CreateLuaCtrl()
    {
        if (Selection.gameObjects.Length != 1)
        {
            return;
        }
        string name = Selection.gameObjects[0].name.Replace("Panel", "");
        string path = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);
        string parent = path.Substring(path.IndexOf("Panel"), path.Length - path.LastIndexOf("/")).Replace("Panel", "").Replace("/", "").Replace(name, "");
        string Info = Str_Ctrl.Replace("thisCtrl", name+"Ctrl").Replace("crtl", name.ToLower()).Replace("panel", Selection.gameObjects[0].name).Replace("parent", parent);
        WriteFileByLine(Application.dataPath, name + "Ctrl" + ".lua", Info);
    }
}