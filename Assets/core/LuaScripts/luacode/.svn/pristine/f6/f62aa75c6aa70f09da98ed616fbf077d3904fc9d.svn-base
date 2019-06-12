local function event_interface(cls)
	function cls:on_event(name)
		local event = self.__event_obj
			:filter(function(_name) return _name == name end)
			:map(function(_name, ...) return ... end)
		return event
	end

	function cls:emit_event(name, ...)
		if self.__event_call[name] == nil then
			self.__event_call[name] = arg
			self.__event_obj:onNext(name, ...)
			self.__event_call[name] = nil
        else
            --TODO:��ʱ���޸ģ����Ƴ�
            --Ŀǰֻ����С�ڵ�������ͬ���¼�ͬʱ����ʱ����©����
            --������˵Ӧ���Ѿ��㹻�ˣ�����Ȼ�����ȫ������
            local aim = self.__event_call[name]
            local cur = arg
            local is_same = true
            for k,v in pairs(aim) do
                if v ~= cur[k] then
                    is_same = false
                    return
                end
            end

            if is_same then
                self.__event_obj:onNext(name, ...)
                self.__event_call[name] = nil
            end
		end
	end
end

local function event_decl(cls, name)
	event_interface(cls)

	cls["on_event_"..name] = function(self)
		return self:on_event(name)
	end

	cls["emit_event_"..name] = function(self, ...)
		return self:emit_event(name, ...)
	end
end

local function event_init(self)
	self.__event_meta = self.__event_meta or {}
	self.__event_obj  = self.__event_obj  or rx.Subject.create()
	self.__event_call = self.__event_call or {}
end

return {
	decl = event_decl,
	init = event_init,
	interface = event_interface,
}