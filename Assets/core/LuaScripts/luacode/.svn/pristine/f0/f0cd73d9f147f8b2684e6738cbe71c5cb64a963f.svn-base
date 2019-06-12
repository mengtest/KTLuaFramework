
--local parser = require("common.util.parser")
local text_parser = require "common.util.parser"

local UtilString = {}

--从前切割str(返回str[count:len], str[0:count])
--str(string):待切割的字符串
--count(int):欲切割去的字符串字数
function UtilString:discard_front(str, count)
	local len = string.len(str)
	count = math.clamp(count, 0, len)
	local front = string.sub(str, 0, count)
	local behind = string.sub(str, count, len)
	return behind, front
end

-- 以定界符delimiter为标记拆分文字源str，返回拆分后的string数组
function  UtilString:split_to_arr(str, delimiter)
	local strArr = {};
	for match in string.gmatch(str, "[^"..delimiter.."]+") do
		table.insert(strArr, match);
	end
	return strArr;
end

function UtilString:split(str, delimiter)
    local sub_str_tab = {}
    while true do          
        local pos_begin, pos_end = string.find(str, delimiter) 
        if not pos_begin then
            table.insert(sub_str_tab, str)
            break
        end  
        local sub_str = string.sub(str, 1, pos_begin - 1)              
        table.insert(sub_str_tab, sub_str)
        str = string.sub(str, pos_end + 1, string.len(str))
    end

    return sub_str_tab
end

-- 以定界符delimiter为标记拆分文字源str，返回拆分后的Int数组
-- 此函数只做拆分处理，不执行str的类型判断,因此请确保传入的str文字源格式正确
function  UtilString:split_to_num_arr(str, delimiter)
	local intArr = {};
	for match in string.gmatch(str, "[^"..delimiter.."]+") do
		table.insert(intArr, tonumber(match));
	end
	return intArr;
end

-- 将table中的key和value格式化为字符串
-- tbl(table):待格式化的table
-- indent_symbol:无需填写
function UtilString:get_string_by_tbl(tbl, indent_symbol)
	tbl = tbl or "nil"
    indent_symbol = indent_symbol or ""
    if type(tbl) ~= "table" then
        return tostring(tbl)
    end
    local text = ""
    for k,v in pairs(tbl) do
        local str_v = ""
        if type(v) == "table" then
            str_v = "table\n"
            str_v = str_v .. self:get_string_by_tbl(v, indent_symbol .. "  ")
        else
            str_v = tostring(v) .. "\n"
        end
        text = text .. indent_symbol .. "k = " .. tostring(k) .. ",\tv = " .. str_v
    end
    return text
end

-- 将list格式的字符串转化为list(支持多重嵌套)
-- 采用json格式（支持存储 string, number, bool, list, dict 类型）
-- 示例1：    [1, "second", 3]
-- 示例2：    [2.3, {"key":"value"}, [2,3,4]]
function UtilString:parse_to_list(text)
    return text_parser:list_parser(text)
end

-- 将dict格式的字符串转化为dict(支持多重嵌套)
-- 采用json格式（支持以 string, int, bool 作为key）
function UtilString:parse_to_dict(text)
    return text_parser:dict_parser(text)
end


return UtilString