local UtilFunc = {}

-- 把角度限制在[-180, 180]
function UtilFunc.clamp_angle_180(angle)
	while angle > 180 do
		angle = angle - 360
	end
	while angle < -180 do
		angle = angle + 360
	end

	return angle
end

-- 把角度限制在[0, 360]
function UtilFunc.clamp_angle_0_360(angle)
	while angle > 360 do
		angle = angle - 360
	end
	while angle < 0 do
		angle = angle + 360
	end

	return angle
end

-- 限制角度到[-360, 360]
function UtilFunc.clamp_angle_360(angle)
	while angle > 360 do
		angle = angle - 360
	end
	while angle < -360 do
		angle = angle + 360
	end

	return angle
end	


-- 获取较小的夹角
function UtilFunc.smaller_angle(a, b)
	local angle = math.abs(a - b)
	angle = UtilFunc.clamp_angle_0_360(angle)
	if angle > 180 then
		angle = 360 - angle
	end
	return angle
end



function UtilFunc.is_inf(val)
	return val == math.huge or val == -math.huge
end

function UtilFunc.is_nan(val)
	return val ~= val
end



return UtilFunc
