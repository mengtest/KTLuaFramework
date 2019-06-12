local BaseEntity=class("BaseEntity")
local U = require 'common.util.underscore'

function BaseEntity:initialize( ... )
	self.m_componentTuple={}--字典
	U.each(arg,function(component)
		self.m_componentTuple[component.name]=component
	end)
end

--返回符合componentName列表的组件Tuple
--arg:componentName列表
function BaseEntity:GetComponentTuple(...)
	if arg==nil or #arg==0 then
		return self.m_componentTuple
	end

	local componentsTuple={}
	U.each(arg,function(name)
		componentsTuple[name]=self.m_componentTuple[name]
	end)
	return componentsTuple
end

function BaseEntity:GetComponent(componentName)
	return self.m_componentTuple[componentName]
end

function BaseEntity:AddComponent(component)
	self.m_componentTuple[component.name]=component
end

function BaseEntity:DelComponent(componentName)
	self.m_componentTuple[componentName]=nil
end

function BaseEntity:HasComponent(componentName)
	return self.m_componentTuple[componentName]~=nil
end

function BaseEntity:CloneComponent( ... )
	local newComponent={}
	return newComponent
end

function BaseEntity:Clear()
	local keys=U.keys(self.m_componentTuple)
	U.each(keys,function(key)
		self.m_componentTuple[key]=nil
	end)
end

return BaseEntity
