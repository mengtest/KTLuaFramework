local BaseSystem=require "system.BaseSystem"
local DelayCommandSystem=class("DelayCommandSystem",BaseSystem)

--延迟执行遍历时会因为删除添加对象导致的操作
--此系统要靠后顺序加入到entityManager中
function DelayCommandSystem:initialize(...)
	DelayCommandSystem.super.initialize(arg)
end

function DelayCommandSystem:Update(entityManager,deltaTime)
	DelayCommandSystem.super.Update(entityManager,deltaTime)

end

function DelayCommandSystem:Destroy()

end

return DelayCommandSystem