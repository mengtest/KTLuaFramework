
local State = require("common.util.HFSM.State")
local HFSM_event =require("common.util.HFSM.HFSM_event")

local HFSM = class("HFSM")

function HFSM:initialize()
	self.m_state_dic = {}
	self.m_cur_state_path = {}
	self.m_root_state = State:new()
	
	self.m_root_state:set_name("root")
	self.m_state_dic["root"] = self.m_root_state
	table.insert(self.m_cur_state_path, self.m_root_state)
end

--为状态机添加“一级状态”（挂载于根节点下的状态）
--state_name(string):状态名
--state(State):状态
function HFSM:add_state(state_name, state)
	self:add_state_in(state_name, state, "")
end

--在父状态下挂载“二级状态”
--state_name(string):二级状态名
--state(State)二级状态
--father_name(string)父状态名
function HFSM:add_state_in(state_name, state, father_name)
	if state_name == "root" then
		print("自定义状态不可以\"root\"命名。")
		return
	end
	state:set_name(state_name)
	if not father_name or father_name == "" then
		state:set_father_name("root")
	else
		state:set_father_name(father_name)
	end
	self.m_state_dic[state_name] = state
end

--初始化状态机至指定状态
--state_name(string):初始状态的状态名
function HFSM:init(state_name)
	self:transform(state_name)
end

--不带参数的发报器
--msg(string):特征码
function HFSM:post(msg)
	local evt = HFSM_event:new()
	evt.msg = msg
	self:post_event(evt)
end

--携带参数的发报器
--目前按当前状态的目录结构，由根及叶按层发送特征码
--故可能出现消息还未广播完就遇到了状态跳转指令，
--如果出现此状况则不再继续对原路径中其余状态进行广播。
--MARK:目前尚未发现如此处理可能导致的致命性漏洞，但仍觉得这样的逻辑似乎并不算完美。
--evt.msg(string):特征码
--evt.obj(anything):携带的参数
function HFSM:post_event(evt)
	for i = 1, #self.m_cur_state_path do
		local state = self.m_cur_state_path[i]
		local state_name = state:post_event(evt)
		if state_name ~= "" then
			self:transform(state_name)
			break
		end
	end
end

function HFSM:transform(state_name)
	local same_path_list = {}
	local cur_path_list = {}
	for i = 1, #self.m_cur_state_path do
		table.insert(cur_path_list, self.m_cur_state_path[i])
	end
	local target_path_list = self:find_state_path(state_name)
	self:slipt_same_path(same_path_list, cur_path_list, target_path_list)
	if #same_path_list == 0 then
		print("如果逻辑正确应该是不会走到这里的，如果进入此处，则说明之前的逻辑有误。")
		return
	end
	for i = #cur_path_list, 1, -1 do
		cur_path_list[i]:over()
        table.remove(self.m_cur_state_path, #self.m_cur_state_path)
	end
	for i = 1, #target_path_list do
		target_path_list[i]:start()
        table.insert(self.m_cur_state_path, target_path_list[i])
	end
end

function HFSM:find_state_path(state_name)
	local path_list = {}
	local target_state = self:get_state_by_name(state_name)
	if target_state then
		table.insert(path_list, 1, target_state)
		local father_state = self:get_state_by_name(target_state:get_father_name())
		self:push_father_state_forward(path_list, father_state)
	end
	return path_list
end

function HFSM:get_state_by_name(state_name)
	local state = self.m_state_dic[state_name]
	if state then
		return state
	else
		--MARK:: Needed to modify 添加无Key指向的State的日志记录
		return nil
	end
end

function HFSM:push_father_state_forward(state_list, state)
	if state then
		table.insert(state_list, 1, state)
		local father_state = self:get_state_by_name(state:get_father_name())
		self:push_father_state_forward(state_list, father_state)
	end
end

function HFSM:slipt_same_path(same_path_list, path_list1, path_list2)
	local rst1 = #path_list1 > 0 and #path_list2 > 0
	local rst2 = rst1
	if rst1 then
		rst2 = path_list1[1] == path_list2[1]
	end
	if rst2 then
		table.insert(same_path_list, path_list1[1])
		table.remove(path_list1, 1)
		table.remove(path_list2, 1)
		self:slipt_same_path(same_path_list, path_list1, path_list2)
	end
end

return HFSM