local World=class("world")
local U = require 'common.util.underscore'

function World:initialize( ... )
	self.m_systems={} --list
	self.m_entities={} --字典
	--self.m_componentsCache={} --组件池，用于产生和回收所有组件
end

function World:Update(deltaTime)
	U.each(self.m_systems,function(system)
		system:Update(self,deltaTime)
	end)
end

function World:Destroy()

end

--[[entity-----------------------------]]
function World:AddEntity(entity)
	self.m_entities[entity.name]=entity
end

function World:DelEntity(entityName)
	self.m_entities[entityName]=nil
end

function World:GetEntities()
	return self.m_entities
end

function World:ClearEntities()
	local keys=U.keys(self.m_entities)
	U.each(keys,function(key)
		self.m_entities[key]=nil
	end)
end

--[[system-----------------------------]]
function World:AddSystem(system)
	if U.include(system) then
		return
	end

	table.insert(self.m_systems,entity)
end

function World:DelSystem(systemName)
	utility.table:remove_by_val(self.m_systems,systemName)
end

function World:ClearSystems()
	for i = #self.m_systems, 1, -1 do
		table.remove(self.m_systems, i)
	end
end

--[[component-----------------------------]]

--根据组件名称列表，返回结果和满足所有名称的第一个实体的组件Tuple
function World:Fectch(...)
	if not arg or #arg==0 then
		return false,nil
	end

	local entites=U.values(self:GetEntities())
	local entity=U.detect(entites,function(entity)
		U.all(arg,function(name)
			return entity:HasComponent(name)
		end)
	end)

	if entity then
		return true,entity:GetComponentTuple(arg)
	else
		return false,nil
	end
end

--根据组件名称列表，返回满足所有名称的实体的组件Tuple列表
--参数为组件名称列表
function World:FectchAll(...)
	if not arg or #arg==0 then
		return nil
	end

	local entites=U.values(self:GetEntities())
	local selected_entites=U.select(entites,function(entity)
		return U.all(arg,function(name)
			return entity:HasComponent(name)
		end)
	end)
	local componentTuples=U.map(selected_entites,function(entity)
		return entity:GetComponentTuple(arg)
	end)

	return componentTuples
end

--备份数据
function World:Copy()
	
end

--回滚
function World:Rollback()

end

-- function World:PoolFrom(component)
-- end

-- function World:PoolTo(componentName)
-- 	return 
-- end

return World