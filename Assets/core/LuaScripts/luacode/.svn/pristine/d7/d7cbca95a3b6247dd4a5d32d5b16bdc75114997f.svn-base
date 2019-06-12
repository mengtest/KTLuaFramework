
--该事件消息类在lua中可省略（直接new table，然后为表添加项也行），
--仍然将该类写明是为了注明状态机中用以通讯的消息体所包含的字段。
local HFSM_event = class("HFSM_event")

function HFSM_event:initialize()
	self.msg = ""
	self.obj = nil
end

return HFSM_event