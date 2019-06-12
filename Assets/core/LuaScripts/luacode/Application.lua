local Application = class("Application")
local World=require "World"
local Time=require("UnityEngine.Time")

--管理整个游戏
function Application:initialize(...)
	--提供各种管理器的访问，比如ui，通信，等等
	--提供world的管理
	self.m_world=World:new()
end

function Application:Update()
	if self.m_world then
		self.m_world:Update(deltaTime)
	end
end

function Application:FixedUpdate()

end

function Application:LateUpdate()

end

return Application