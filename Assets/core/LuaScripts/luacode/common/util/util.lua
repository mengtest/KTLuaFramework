local util = {}

util.lua_number_max = 3.402823466e+38
util.lua_number_min = 1.175494351e-38

function util.loadstring(source, env)
	if (_VERSION == "Lua 5.2") then
		local f, err = load(source, source, "t", env)
		return f, err
	else
		local f, err = loadstring(source)
		if (f and env) then
			setfenv(f, env)
		end
		return f, err
	end
end

function util.utf8len(input,outlimit)
    local len  = string.len(input)
    local left = len
    local cnt  = 0
    local bl_have_out_there = false
    local arr  = {0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc}
    while left ~= 0 do
        local tmp = string.byte(input, -left)
        local i   = #arr
        while arr[i] do
            if tmp >= arr[i] then
                left = left - i
                break
            end
            i = i - 1
        end
        if i > (outlimit or 3) then
        	bl_have_out_there = true
        end
        cnt = cnt + 1
    end
    return cnt,bl_have_out_there
end

function util.utf8split(input)
	local ret = {}
    local len  = string.len(input)
    local left = len
    local cnt  = 0
    local arr  = {0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc}
    while left ~= 0 do
        local tmp = string.byte(input, -left)
        local i   = #arr
        while arr[i] do
            if tmp >= arr[i] then
            	table.insert(ret, string.sub(input, -left, -left + i - 1))
                left = left - i
                break
            end
            i = i - 1
        end
        cnt = cnt + 1
    end
    -- return cnt
    return ret
end

function util.pack(...)
	return {...}
end

function util.min(a, b)
	assert(type(a) == "number")
	assert(type(b) == "number")
	return a < b and a or b
end

function util.max(a, b)
	assert(type(a) == "number")
	assert(type(b) == "number")
	return a > b and a or b
end

function util.enum(tab, start)
	assert(type(tab) == "table")
	local index = start or 1
	local ret = {}
	for _,v in ipairs(tab) do
		assert(type(v) == "string")
		assert(ret[v] == nil)
		ret[v] = index
		index = index + 1
	end
	return ret
end

local function swap(tab, a, b)
	if (a == b) then return end
	local tmp = tab[a]
	tab[a] = tab[b]
	tab[b] = tmp
end

function util.array_sample(arr, n)
	assert(type(arr) == "table")
	local n = n or 1
	assert(n >= 1)
	
	local ret = {}
	for i,v in ipairs(arr) do
		ret[i] = v
	end

	local size = #ret
	local num = math.min(n, size)
	for i = 1, num do
		local index = math.random(i, size)
		swap(ret, i, index)
	end
	for i = num+1, size do
		ret[i] = nil
	end
	return ret
end

function util.version_cmp(a, b)
	local as = util.split(a, ".")
	local bs = util.split(b, ".")
	for i = 1, util.max(#as, #bs) do
		local an = tonumber(as[i]) or 0
		local bn = tonumber(bs[i]) or 0
		if (an < bn) then
			return -1
		elseif (an > bn) then
			return 1
		end
	end
	return 0
end

function util.version_gte(a, b)
	return util.version_cmp(a, b) >= 0
end

function util.version_gt(a, b)
	return util.version_cmp(a, b) > 0
end

function util.version_lte(a, b)
	return util.version_cmp(a, b) <= 0
end

function util.version_lt(a, b)
	return util.version_cmp(a, b) < 0
end

function util.version_eq(a, b)
	return util.version_cmp(a, b) == 0
end

function util.clamp(val, min, max)
	return util.min(util.max(val, min), max)
end

function util.ro_table(t, ro)
	if (not ro) then return t end
	
	local keys = nil
	return setmetatable({
		__keys = function()
			if (keys == nil) then
				keys = {}
				for k,_ in pairs(t) do
					table.insert(keys, k)
				end
				table.sort(keys)
			end
			return keys
		end
	}, {
		__index = t,
		__newindex = function(t, k, v)
			error("attempt to modify read-only table")
		end,
		__metatable = false
	})
end

function util.clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

function util.lock_global()
	assert(getmetatable(_G) == nil)
	setmetatable(_G, {
		__newindex = function(t, k, v)
			error("try to create global variable >>>"..k.."<<<\n"..debug.traceback())
		end
		})
end

function util.unlock_global()
	assert(getmetatable(_G) ~= nil)
	setmetatable(_G, nil)
end

function util.tab_keys(tab)
	local keys = {}
	for k,_ in pairs(tab) do
		table.insert(keys, k)
	end
	return keys
end

function util.tab_values(tab)
	local vals = {}
	for _,v in pairs(tab) do
		table.insert(vals, v)
	end
	return vals
end

function util.tab_is_empty(tab)
	return next(tab) == nil
end

function util.dump(obj)
	local ret = ""
	if (type(obj) == "table") then
		local keys = {}
		for k,_ in pairs(obj) do
			table.insert(keys, k)
		end
		table.sort(keys, function(a,b) 
				if (type(a) == type(b)) then
					return a < b
				end
				return type(a) == "number"
			end)
		ret  = ret .. "{"
		for _,k in ipairs(keys) do
			local v = obj[k]
			ret = ret .. "[" .. util.dump(k) .. "] = "
			ret = ret .. util.dump(v) .. ","
		end
		ret  = ret .. "}"
	elseif (obj == nil or type(obj) == "number" or type(obj) == "boolean") then
		ret = tostring(obj)
	else
		ret = string.format("%q", tostring(obj))
	end
	return ret
end

function util.split(str, sep)
	assert(type(str) == "string")
	assert(type(sep) == "string")
	assert(string.len(sep) > 0)

	local ret = {}
	for i in string.gmatch(str, "[^"..sep.."]+") do
		table.insert(ret, i)
	end
	return ret
end

function util.read_obj_from_string(content)
	if (content == nil) then
		return false, "empty string"
	end

	local func,err = util.loadstring("return "..content, {})
	if (not func) then
		return false, err
	end

	return pcall(func)
end

function util.save_obj_to_string(obj)
	return util.dump(obj)
end

function util.read_table_from_string(content)
	if (content == nil) then
		return false, "empty string"
	end

	local func,err = util.loadstring(content, {})
	if (not func) then
		return false, err
	end

	return pcall(func)
end

function util.save_table_to_string(obj)
	return "return " .. util.dump(obj)
end


function util.read_table_from_file(filename)
	local file,err = io.open(filename, "rb")
	if (not file) then
		return false, err
	end

	local content = file:read("*a") or ""
	file:close()

	return util.read_table_from_string(content)
end

function util.save_table_to_file(filename, obj)
	local content = util.save_table_to_string(obj)

	local file,err = io.open(filename, "wb")
	if (not file) then
		return false, err
	end

	file:write(content)
	file:close()
	return true
end

function util.add_delay_property(t, propertys)
	assert(type(t) == "table")
	assert(getmetatable(t) == nil)
	assert(type(propertys) == "table")

	for k,v in pairs(propertys) do
		assert(t[k] == nil)
		assert(type(v) == "function")
	end

	return setmetatable(t, {
			__index = function(t,k)
				local func = propertys[k]
				if (func == nil) then
					return nil
				end

				local value = func(t)
				t[k] = value
				return value
			end
		})
end

function util.all(t, proc)
	assert(type(t) == "table")
	assert(type(proc) == "function")

	for k,v in pairs(t) do
		if (not proc(k,v)) then
			return false
		end
	end

	return true
end

function util.any(t, proc)
	assert(type(t) == "table")
	assert(type(proc) == "function")

	for k,v in pairs(t) do
		if (proc(k,v)) then
			return true
		end
	end

	return false
end

function util.contain(t, value)
	for k,v in pairs(t) do
		if (v == value) then
			return true
		end
	end

	return false
end

function util.map(t, proc)
	assert(type(t) == "table")
	assert(type(proc) == "function")

	local ret = {}
	for k,v in pairs(t) do
		ret[k] = proc(k, v)
	end

	return ret
end

function util.group_by(t, proc)
	assert(type(t) == "table")
	assert(type(proc) == "function")

	local ret = {}
	for k,v in pairs(t) do
		local group_key = proc(k, v)
		assert(group_key ~= nil)

		local group = ret[group_key]
		if (group == nil) then
			group = {}
			ret[group_key] = group
		end
		
		table.insert(group, v)
	end

	return ret
end

function util.schedule(node, callback, delay)
    local delay = CCDelayTime:create(delay)
    local callfunc = CCCallFunc:create(callback)
    local sequence = CCSequence:createWithTwoActions(delay, callfunc)
    local action = CCRepeatForever:create(sequence)
    node:runAction(action)
    return action
end

function util.performWithDelay(node, callback, delay)
    local delay = CCDelayTime:create(delay)
    local callfunc = CCCallFunc:create(callback)
    local sequence = CCSequence:createWithTwoActions(delay, callfunc)
    node:runAction(sequence)
    return sequence
end

local gss = {}
function util.globalSchedule(callback, delay)
	local id = CCDirector:getInstance():getScheduler():scheduleScriptFunc(callback, delay, false)
	gss[id] = true
	return id
end

function util.globalUnschedule(id)
	local ret = CCDirector:getInstance():getScheduler():unscheduleScriptEntry(id)
	gss[id] = nil
	return ret
end

function util.removeAllGlobalUnschedule()
	for k,_ in pairs(gss) do
		util.globalUnschedule(k)
	end
end

function util.getPatch(from, to)
	assert(type(from) == "table")
	assert(type(to) == "table")
	local patch = nil

	local fields = {}
	for k,_ in pairs(from) do
		fields[k] = true
	end
	for k,_ in pairs(to) do
		fields[k] = true
	end
	for k,_ in pairs(fields) do
		if (type(from[k]) == "table" and type(to[k]) == "table") then
			local sub = util.getPatch(from[k], to[k])
			if (sub) then
				patch = patch or {}
				patch.sub = patch.sub or {}
				patch.sub[k] = sub
			end
		else
			if (not(type(from[k]) == type(to[k]) and from[k] == to[k])) then
				patch = patch or {}
				if (to[k] ~= nil) then
					patch.set = patch.set or {}
					patch.set[k] = to[k]
				else
					patch.del = patch.del or {}
					patch.del[k] = true
				end
			end
		end
	end

	return patch
end

function util.applyPatch(obj, patch)
	assert(type(obj) == "table")
	assert(type(patch) == "table")

	if (patch.set) then
		for k,v in pairs(patch.set) do
			obj[k] = v
		end
	end
	if (patch.del) then
		for k,_ in pairs(patch.del) do
			obj[k] = nil
		end
	end
	if (patch.sub) then
		for k,v in pairs(patch.sub) do
			util.applyPatch(obj[k], v)
		end
	end
	return true
end

local proxy_meta = {}

local function is_proxy_object(obj)
	if (type(obj) == "table") then
		return rawget(obj, "__proxy")
	end
	return false
end

local function bind_proxy(proxy, origin, parent, key, patch)
	util.proxyHook()

	assert(type(origin) == "table")
	assert(not is_proxy_object(origin))
	assert(type(proxy) == "table")
	assert(next(proxy, nil) == nil)

	proxy.__proxy = true
	proxy.__origin = origin
	proxy.__parent = parent
	proxy.__key = key
	proxy.__patch = patch

	return setmetatable(proxy, proxy_meta)
end

local function new_proxy(origin, parent, key, patch)
	return bind_proxy({}, origin, parent, key, patch)
end

local function make_proxy(tab, parent, key)
	assert(type(tab) == "table")
	assert(not is_proxy_object(tab))

	local new = {}
	for k,v in pairs(tab) do
		if (type(v) == "table") then
			new[k] = make_proxy(v, tab, k)
		else
			new[k] = v
		end
		tab[k] = nil
	end

	return new, bind_proxy(tab, new, parent, key)
end

local function my_size(t)
	local origin = rawget(t, "__origin")
	return #origin
end

local function my_maxn(t)
	local origin = rawget(t, "__origin")
	return table.maxn(origin)
end

local function my_next(t, index)
	local origin = rawget(t, "__origin")
	local k,_ = next(origin, index)
	return k, t[k]
end

local function my_inext(t, index)
	local k = index + 1
	local v = t[k]
	if (v) then
		return k, v
	end
end

local function my_patch(t, args, val)
	local parent = rawget(t, "__parent")
	local key = rawget(t, "__key")
	local patch = rawget(t, "__patch")

	if (patch) then
		patch(args, val)
	end

	if (parent) then
		assert(key)
		table.insert(args, 1, key)
		my_patch(parent, args, val)
	end
end

function proxy_meta:__index(k)
	local origin = rawget(self, "__origin")
	local child = rawget(self, "__child")
	local parent = rawget(self, "__parent")
	local key = rawget(self, "__key")

	if (child) then
		local v = rawget(child, k)
		if (v) then return v end
	end

	local v = rawget(origin, k)
	if (type(v) == "table") then
		if (not child) then
			child = {}
			rawset(self, "__child", child)
		end
		local p = new_proxy(v, self, k)
		rawset(child, k, p)
		return p
	end

	return v
end
function proxy_meta:__newindex(k, v)
	local origin = rawget(self, "__origin")
	local child = rawget(self, "__child")
	local parent = rawget(self, "__parent")
	local key = rawget(self, "__key")

	local old_v = self[k]
	if (type(v) == type(old_v) and v == old_v) then
		return
	end
	if (type(v) == "table") then
		if (not child) then
			child = {}
			rawset(self, "__child", child)
		end
		local d, p = make_proxy(v, self, k)
		rawset(child, k, p)
		rawset(origin, k, d)
		my_patch(self, {k}, d)
	else
		if (child) then
			rawset(child, k, nil)
		end
		rawset(origin, k, v)
		my_patch(self, {k}, v)
	end
end
function proxy_meta:__pairs()
	return my_next, self, nil
end
function proxy_meta:__ipairs()
	return my_inext, self, 0
end
function proxy_meta:__len()
	return my_size(self)
end
function proxy_meta:__maxn()
	return my_maxn(self)
end
function proxy_meta:__insert(pos, val)
	assert(not is_proxy_object(val))
	local size = my_size(self)
	if (val == nil) then
		val = pos
		pos = size + 1
	else
		assert(pos >= 1 and pos <= size)
	end
	if (val == nil) then 
		return 
	end

	local origin = rawget(self, "__origin")
	for i = size, pos, -1 do
		self[i+1] = origin[i]
	end
	self[pos] = val
end
function proxy_meta:__remove(pos)
	local size = my_size(self)
	if (pos == nil) then
		pos = size
	else
		assert(pos >= 1 and pos <= size)
	end

	local origin = rawget(self, "__origin")
	local val = origin[i]
	for i = pos, size do
		self[i] = origin[i+1]
	end
	return val
end

function util.cascadePatchFunction(f_patch, key)
	if (type(f_patch) ~= "function") then
		return
	end

	return function(args, val) 
		table.insert(args, 1, key)
		f_patch(args, val)
	end
end

function util.makeProxy(tab, f_patch, key)
	assert(type(tab) == "table")
	assert(not util.isProxyObject(tab))

	local f_patch_next = util.cascadePatchFunction(f_patch, key)
	local new = {}

	for k,v in pairs(tab) do
		if (type(v) == "table") then
			new[k] = util.makeProxy(v, f_patch_next, k)
		else
			new[k] = v
		end
		tab[k] = nil
	end

	util.bindProxy(new, tab, f_patch_next)
	return new, tab
end

local hooked = false
function util.proxyHook()
	if (hooked) then return end
	hooked = true

	local tab_insert = table.insert
	local tab_remove = table.remove
	local tab_maxn = table.maxn
	local tab_pairs = pairs
	local tab_ipairs = ipairs

	table.insert = function(t, ...)
		local meta = getmetatable(t)
		local f = meta and meta.__insert or tab_insert
		return f(t, ...)
	end

	table.remove = function(t, pos)
		local meta = getmetatable(t)
		local f = meta and meta.__remove or tab_remove
		return f(t, pos)
	end

	table.maxn = function(t)
		local meta = getmetatable(t)
		local f = meta and meta.__maxn or tab_maxn
		return f(t)
	end

	pairs = function(t)
		local meta = getmetatable(t)
		local f = meta and meta.__pairs or tab_pairs
		return f(t)
	end

	ipairs = function(t)
		local meta = getmetatable(t)
		local f = meta and meta.__ipairs or tab_ipairs
		return f(t)
	end
end

function util.isProxyObject(obj)
	return is_proxy_object(obj)
end

function util.getProxy(origin, f_patch)
	return new_proxy(origin, nil, nil, f_patch)
end

local function test_branch(a, b)
	local max = math.min(#a, #b)
	for i = 1, max do
		local a = a[i]
		local b = b[i]
		if (type(a) ~= type(b)) then
			return false
		end
		if (a ~= b) then
			return false
		end
	end

	local a = a[max +1]
	local b = b[max +1]

	if (a ~= nil) then
		return true, 1
	end
	if (b ~= nil) then
		return true, -1
	end
	return true, 0
end

function util.proxyPatchBuilder(patch)
	assert(type(patch) == "table")
	return function(args, val)
		local rm_index
		for i,v in ipairs(patch) do
			local t,c = test_branch(args, v.args)
			if (t) then
				if (c <= 0) then
					rm_index = rm_index or {}
					table.insert(rm_index, i)
				else
					assert(rm_index == nil)
					assert(type(v.val) =="table")
					local tmp = util.clone(args)
					for i = 1, #v.args do table.remove(tmp, 1) end
					util.tabSetVal(v.val, tmp, val)
					return
				end
			end
		end
		if (rm_index) then
			for i = #rm_index, 1, -1 do
				table.remove(patch, rm_index[i])
			end
		end
		table.insert(patch, {
			args = args,
			val = val,
			})
	end
end

function util.tabSetVal(tab, keys, val)
	assert(type(tab) == "table")
	assert(type(keys) == "table")

	local key = table.remove(keys, 1)
	assert(key)

	if (#keys > 0) then
		util.tabSetVal(tab[key], keys, val)
	else
		tab[key] = val
	end
end

function util.tabClear(tab)
	for k,v in pairs(tab) do
		tab[k] = nil
	end
end

function util.applyPatch2(obj, patch)
	assert(type(obj) == "table")
	assert(type(patch) == "table")

	for _,v in ipairs(patch) do
		util.tabSetVal(obj, util.clone(v.args), v.val)
	end
end

function util.isEqual(a, b)
	if (type(a) ~= type(b)) then
		return false
	end

	if (type(a) == "table") then
		for k,_ in pairs(a) do
			if (not util.isEqual(a[k], b[k])) then
				return false
			end
		end
		for k,_ in pairs(b) do
			if (not util.isEqual(a[k], b[k])) then
				return false
			end
		end
		return true
	end

	return a == b
end

function util.fixNumberKey(val)
	if (type(val) ~= "table") then 
		return val 
	end

	local ret = {}
	for k,v in pairs(val) do
		local n = tonumber(k)
		if (n) then
			ret[n] = util.fixNumberKey(v)
		else
			ret[k] = util.fixNumberKey(v)
		end
	end
	return ret
end

function util.forceStringKey(val)
	if (type(val) ~= "table") then 
		return val
	end

	local ret = {}
	for k,v in pairs(val) do
		ret[tostring(k)] = util.forceStringKey(v)
	end
	return ret
end

function util.modifyAttValue(att, field, op, val)
	assert(type(att) == "table")
	assert(type(field) == "string")
	assert(field ~= "")
	assert(op == "add" or op == "set")
	assert(type(val) == "string")
	assert(field ~= "")

	local code = "assert(att."..field.."~=nil);"
	code = code.."att."..field.."="
	if (op == "add") then
		code =code .."att."..field.."+"
	end
	code = code..val

	local f = util.loadstring(code, {att = att, assert = assert})
	if (f) then
		f()
	end
end 

function util.hookSave(name, func)
	assert(type(name) == "string")
	assert(type(func) == "function")

	local tab = _G["__hook"]
	if (tab == nil) then
		tab = {}
		_G["__hook"] = tab
	end
	if (tab[name] == nil) then
		tab[name] = func
	end
end

function util.hookGet(name)
	assert(type(name) == "string")
	
	local tab = _G["__hook"]
	return tab and tab[name]
end

return util