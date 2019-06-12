local BaseComponent=require "components.BaseComponent"
local SingletonComponent=class("Singleton",BaseComponent)

function SingletonComponent:initialize(...)
	SingletonComponent.super.initialize(arg)
end

return SingletonComponent