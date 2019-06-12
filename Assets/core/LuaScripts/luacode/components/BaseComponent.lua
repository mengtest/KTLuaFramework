local BaseComponent=class("BaseComponent")

-- Attribute.decl(BaseComponent, "attr1")
-- Attribute.decl(BaseComponent, "attr12")

function BaseComponent:initialize( ... )
	Attribute.init(self)
end

return BaseComponent