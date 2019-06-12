
local State = require("common.util.HFSM.State")
local Delegate2 = require("common.util.HFSM.Delegate2")

local State_with_event_map = class("State_with_event_map", State)

function State_with_event_map:initialize()
	self.m_action_map = {}
	
	self.on_start = Delegate2:new()
	self.on_over = Delegate2:new()
end

--���״̬ת���¼����������յ�ָ����Ϣ���л�״̬,����֮�ⲻ��ִ�ж��������
--msg(string):�����¼�����Ϣ
--next_state(string):��Ҫ�л�����״̬������
function State_with_event_map:add_event(msg, next_state)
	self.m_action_map[msg] = function ()
		return next_state
	end
end

--���״̬ת���¼���״̬��������ִ����state_action֮���л����䷵��ֵ��ָ���״̬��
--MARK:[���鲻Ҫʹ��]�˺�������ְ��һ�ĺ������൱��add_event_action��add_event�Ľ���壬
--��ˣ���ʹ�ò������ܵ��¹��������״̬���ṹ���ҡ�
--msg(string):�����¼�����Ϣ
--state_action(function):����ֵΪstring����״̬���ĺ���
function State_with_event_map:add_state_action(msg, state_action)
    if self.m_action_map[msg] then
        print("The message you want to listen is already existed. Message name is: " .. msg)
        return
    end
    self.m_action_map[msg] = state_action
end

--��Ӵ����¼������������¼�����ִ���κ�״̬�л�������߼���
--msg(string):�����¼�����Ϣ
--event_action(function):������Ҫִ�еı���������
function State_with_event_map:add_event_action(msg, event_action)
    if self.m_action_map[msg] then
        print("The message you want to listen is already existed. Message name is: " .. msg)
        return
    end
	self.m_action_map[msg] = function (self, hfsm_event)
		event_action(self, hfsm_event)
		return ""
	end
end

--�Ƴ�ָ����Ϣ�ļ���
--msg(string):�����¼�����Ϣ
function State_with_event_map:del_action(msg)
    self.m_action_map[msg] = nil
end

function State_with_event_map:on_post_event(hfsm_event)
	local rst = ""
	local func = self.m_action_map[hfsm_event.msg]
	if func then
		rst = func(self, hfsm_event)
	end
	return rst
end

function State_with_event_map:do_start()
	self.on_start:call()
end

function State_with_event_map:do_over()
	self.on_over:call()
end

return State_with_event_map