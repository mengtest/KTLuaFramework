
local Delegate2 = class("Delegate2")

function Delegate2:initialize()
	self.m_action = {}
end

function Delegate2:add(func)
    if utility.table:is_contain(self.m_action, func) then
        return false
    else
        table.insert(self.m_action, func)
        return true
    end
end

function Delegate2:remove(func)
	return utility.table:remove_by_val(self.m_action, func)
end

function Delegate2:count()
	return #self.m_action
end

function Delegate2:call(msg)
	for k, v in pairs(self.m_action) do
		v(msg)
	end
end

return Delegate2