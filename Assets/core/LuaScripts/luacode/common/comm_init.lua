function REQUIRE_BASE(...)
	local base = (...):match("^(.+)[%./][^%./]+")
	return function(path)
		if path and base then path = base.."/"..path end
		return require(path)
	end
end

local require_relative = REQUIRE_BASE(...)
table.unpack = table.unpack or unpack

--导入工具类
require_relative("util.all")
utility = {}	-- 2018-02-26 pacman 通用工具namespace
utility.log    = require("common.util.util_log")
utility.math	= require("common.util.util_math")
utility.string	= require("common.util.util_string")
utility.table	= require("common.util.util_table")
utility.func        = require("common.util.func")

--主功能模块
Attribute           = require("common.util.attribute") --require_relative("attribute")
Event               = require("common.util.event")
-- Event_system        = require("event.event_system")
-- Event_system:init()
-- TableLoader         = require_relative("table_loader")
