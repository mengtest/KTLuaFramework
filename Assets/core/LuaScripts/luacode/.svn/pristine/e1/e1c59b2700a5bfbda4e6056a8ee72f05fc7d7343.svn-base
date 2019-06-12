
local _bind_mt ={};
_bind_mt.__eq = function(a , b)
		if a._obj == b._obj and a._func == b._func then
			return true
		else
			return false
		end
		return true
	end;
	
_bind_mt.__call = function(tb , ...)
	if tb._func == nil then
		return 
	end
	if tb._obj ~= nil then
		return tb._func(tb._obj , ...)
	else
		return tb._func(...)
	end
end;

--bind function
function event_binder(obj , func)
	local set = {_obj = obj , _func = func}
	setmetatable(set,_bind_mt)
	return set
end
