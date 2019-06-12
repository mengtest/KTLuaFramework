
local State = class("State")

function State:initialize()
	self.m_name = ""
	self.m_father_name = ""
end

function State:get_name()
	return self.m_name
end

function State:set_name(name)
	self.m_name = name
end

function State:get_father_name()
	return self.m_father_name
end

function State:set_father_name(father_name)
	self.m_father_name = father_name
end

function State:start()
	if self.do_start then
		return self:do_start()
	end
end

function State:over()
	if self.do_over then
		return self:do_over()
	end
end

function State:post_event(evt)
	if self.on_post_event then
		return self:on_post_event(evt)
	else
		return ""
	end
end

return State