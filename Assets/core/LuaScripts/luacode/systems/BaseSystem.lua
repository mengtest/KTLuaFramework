local BaseSystem=class("BaseSystem")
local ObserverHost = require "common.util.observer_host"
BaseSystem:include(ObserverHost)
local U = require 'common.util.underscore'

function BaseSystem:initialize(...)

end

--子类可以重写具体逻辑
function BaseSystem:Update(entityManager,deltaTime)
	-- local componentTuples=entityManager:FectchAll("name1","name2")
	-- if not componentTuples or #componentTuples==0 then
	-- 	return
	-- end

	-- U.each(componentTuples,function(componentTuple)
	-- local component1=componentTuple["name1"]
	-- local component2=componentTuple["name2"]
	 	--TODO:xxx
	-- end)
end

function BaseSystem:Destroy()

end

return BaseSystem