-- 属性系统
-- 支持数值属性和表达式属性
-- 通用的属性更新事件
-- 表达式属性默认缓存结果 依赖的属性更新时自动刷新
--
-- TODO: 代码整理和重构

local require_relative = REQUIRE_BASE(...)
-- local Event = require_relative("Event")

local TAG_ARRAY = {"TAG_ARRAY"}
local TAG_TABLE = {"TAG_TABLE"}
local TAG_NIL   = {"TAG_NIL"}
local TAG_CALC  = {"TAG_CALC"}
local TAG_DIRTY = {"TAG_DIRTY"}

local function emit_event(self, name, type, ...)
	self:emit_event(name.."_"..type, ...)
end

local function att_interface(cls)
	Event.interface(cls)
	
	function cls:get_attr(name)
		return self.m_att[name]
	end

	function cls:set_attr(name, val, force)
		local old = self.m_att[name]
		self.m_att[name] = val
		if old ~= val or force == true then
			emit_event(self, name, "set", val, old)
			emit_event(self, name, "set_after", val, old)
		end
	end

    -- pacman 2017-12-07 此功能就相当于set_atrr加个force参数的效果吧
    function cls:set_attr_and_event(name, val)
		self.m_att[name] = val
		emit_event(self, name, "set", val, old)
	end

	function cls:on_event_attr_set(name, init)
		local evt = self:on_event(name.."_set")
		return init and evt:startWith(self.m_att[name]) or evt
	end

	function cls:get_attr_at(name, key)
		return self.m_att[name][key]
	end

	function cls:set_attr_at(name, key, val)
		local old = self.m_att[name][key]
		if old ~= val then
			self.m_att[name][key] = val
			emit_event(self, name, "set_at", key, val, old)
		end
	end

	function cls:on_event_attr_set_at(name, init)
		local evt = self:on_event(name.."_set_at")
		if init then
			for k,v in pairs(self.m_att[name]) do
				evt = evt:startWith(k, v)
			end
		end
		return evt
	end

	function cls:set_attr_insert(name, ...)
		local pos, val
		if select("#", ...) == 1 then
			pos, val = #self.m_att[name] + 1, ...
		else
			pos, val = ...
		end
		table.insert(self.m_att[name], pos, val)
		emit_event(self, name, "insert", pos, val)
	end

	function cls:on_event_attr_insert(name)
		return self:on_event(name.."_insert")
	end

	function cls:set_attr_remove(name, pos)
		local pos = pos or #self.m_att[name]
		local old = table.remove(self.m_att[name], pos)
		emit_event(self, name, "remove", pos, old)
	end

	function cls:on_event_attr_remove(name)
		return self:on_event(name.."_remove")
	end
end

local function att_decl(cls, name, def, tag)
	cls.__att_meta = cls.__att_meta or {}
	assert(cls.__att_meta[name] == nil)
	cls.__att_meta[name] = (def ~= nil and def or TAG_NIL)

	att_interface(cls)
	
	cls["get_"..name] = function(self)
		return self:get_attr(name)
	end

	cls["set_"..name] = function(self, val)
		self:set_attr(name, val)
	end

	cls["on_event_"..name.."_set"] = function(self, init)
		return self:on_event_attr_set(name, init)
	end

	if tag == TAG_ARRAY or tag == TAG_TABLE then
		cls["get_"..name.."_at"] = function(self, key)
			return self:get_attr_at(name, key)
		end

		cls["set_"..name.."_at"] = function(self, key, val)
			self:set_attr_at(name, key, val)
		end

		cls["on_event_"..name.."_set_at"] = function(self, init)
			return self:on_event_attr_set_at(name, init)
		end
	end

	if tag == TAG_ARRAY then
		cls["set_"..name.."_insert"] = function(self, ...)
			self:set_attr_insert(name, ...)
		end

		cls["on_event_"..name.."_insert"] = function(self)
			return self:on_event_attr_insert(name)
		end

		cls["set_"..name.."_remove"] = function(self, pos)
			self:set_attr_remove(name, pos)
		end

		cls["on_event_"..name.."_remove"] = function(self)
			return self:on_event_attr_remove(name)
		end
	end
end

local function att_calc(cls, name, func, ...)
	cls.__att_meta = cls.__att_meta or {}
	assert(cls.__att_meta[name] == nil)
	cls.__att_meta[name] = TAG_CALC

	assert(type(func) == "function")
	cls["get_"..name] = function(self)
		if self.m_att[name] == TAG_DIRTY then
			local val = func(self)
			self.m_att[name] = val
			return val
		else
			return self.m_att[name]
		end
	end

	cls["on_event_"..name.."_set"] = function(self, init)
		local evt = self:on_event(name.."_set")
		return init and evt:startWith(self["get_"..name](self)) or evt
	end

	local depends = {...}
	cls["__init"..name] = function(self)
		if #depends > 0 then
			local events = {}
			for i = 1, #depends do
				events[i] = assert(self:on_event(depends[i].."_set_after"))
			end

			events[1]:merge(table.unpack(events, 2))
				:map(function()
					local old = self.m_att[name]
					self.m_att[name] = TAG_DIRTY
					return cls["get_"..name](self), old
				end)
				:filter(function(new, old)
					return old == TAG_DIRTY or old ~= new
				end)
				:subscribe(function(new, old)
					emit_event(self, name, "set", new, old)
					emit_event(self, name, "set_after", new, old)
				end)
		end
	end
end

local function att_init(self)
	Event.init(self)
	self.__att_meta = self.__att_meta or {}

	assert(self.m_att == nil)
	self.m_att = {}
	for k,v in pairs(self.__att_meta) do
		if type(v) == "function" then
			self.m_att[k] = v()
		elseif v == TAG_TABLE or v == TAG_ARRAY then
			self.m_att[k] = {}
		elseif v == TAG_NIL then
			self.m_att[k] = nil
		elseif v == TAG_CALC then
			self.m_att[k] = TAG_DIRTY
			self["__init"..k](self)
		else
			self.m_att[k] = v
		end
	end
end

local function att_alias(cls, name, getter, setter)
	if getter then
		assert(type(getter) == "string")
		assert(cls[getter] == nil)
		cls[getter] = assert(cls["get_"..name])
	end
	if setter then
		assert(type(setter) == "string")
		assert(cls[setter] == nil)
		cls[setter] = assert(cls["set_"..name])
	end
end

return {
	decl  = att_decl,
	calc  = att_calc,
	alias = att_alias,
	init  = att_init,
	TABLE = TAG_TABLE,
	ARRAY = TAG_ARRAY,
}