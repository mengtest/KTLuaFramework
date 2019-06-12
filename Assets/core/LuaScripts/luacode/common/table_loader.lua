local TABLES_DIR = "tables"
local TableLoader = {}

function TableLoader.getTableList()
	return require(TABLES_DIR..".list")
end

function TableLoader.loadAll()
	local tables = {}
	for _, v in ipairs(TableLoader.getTableList()) do
		assert(type(v) == "string")
		tables[v] = TableLoader.loadOne(v)
	end
	return tables
end

function TableLoader.loadOne(name)
	local tab = require(TABLES_DIR.."."..name)
	assert(type(tab) == "table")
	return tab
end

return TableLoader