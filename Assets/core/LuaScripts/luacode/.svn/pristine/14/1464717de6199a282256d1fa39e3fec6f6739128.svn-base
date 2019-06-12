
local Parser = {}



function Parser:list_parser(text)
    local list, other = self:creatList(text)
    if other ~= nil then
        other = self:clearHeadSpace(other)
        if other ~= "" then
            Debug.LogError("你确定此list类型数据末尾没有带上一些神奇的东西咩？")
            return
        end
    end
    return list
end

function Parser:dict_parser(text)
    local dict, other = self:creatDict(text)
    if other ~= nil then
        other = self:clearHeadSpace(other)
        if other ~= "" then
            Debug.LogError("你确定此dict类型数据末尾没有带上一些神奇的东西咩？")
            return
        end
    end
    return dict
end





--创建List，返回此List以及List终止符"]"之后的所有字符串
function Parser:creatList(text)
	
	local s, e = string.find(text, "^[%s]*[%[]");--匹配列表标识符
	if e == nil then
		-- error("未能匹配到有效的List标识符: " .. text);
		return {};
	end
	
	local list = {};
	text = string.sub(text, e + 1);
	text = self:insertVal(list, text, "]");
	
	return list, text;
end

--创建Dict，返回此Dict以及Dict终止符"}"之后的所有字符串
function Parser:creatDict(text)
	
	local s, e = string.find(text, "^[%s]*[{]");--匹配字典标识符
	if e == nil then
		error("未能匹配到有效的Dict起始标识符" .. text);
		return;
	end
	local s2, e2 = string.find(text, "^[%s]*[{][%s]*[}][%s]*")
	if e2 then
		return {}, string.sub(text, e2 + 1)
	end
	
	text = string.sub(text, e + 1);
	
	local dict = {};
	local index = 1;
			
	while true do
		--dict[index] = {};
		
		text = self:clearHeadSpace(text);
		local tempKey = {};
		text = self:insertVal(tempKey, text, ":");
		if #tempKey > 1 then
			error("未匹配到有效的字典分割符 \":\"");
		end
		--dict[index].key = tempKey[1];
		local firstByte, other = self:peekFirstByte(text);
		text = other;
		if firstByte == nil then
			error("表中某处的Dict数据,有处value部分书写有误");
			return;
		else
			local tempVal = {};
			text = self:insertVal(tempVal, text, ",");
			--dict[index].value = tempVal[1];
            dict[tempKey[1]] = tempVal[1]
			local firstChar = string.sub(text, 0, 1);
			if firstChar == "}" then
				text = string.sub(text, 2);
				return dict, text;
			else
				index = index + 1;
                if index >= 100 then
                    return {}, ""
                end
			end
		end
	end
end

function Parser:insertVal(tbl, text, term)
	local endCharByte = string.byte(term);
	if not endCharByte then
		error("未传入有效的类型终止符");
		return;
	end
	local isEmpty = true;
	while isEmpty do
		local firstByte, other = self:peekFirstByte(text);
		text = other;
		if firstByte == nil then
			error("表中某处的数据少了有效的终止符");
			break;
		end
		
		if firstByte == 45 or (firstByte > 47 and firstByte < 58) then--Num
			local rst, other = self:popNum(text);
			table.insert(tbl, rst);
			text = other;
		elseif firstByte == 91 then--"[" creat new list
			local rst, other = self:creatList(text);
			table.insert(tbl, rst);
			text = other;
		elseif firstByte == 123 then--"{" creat new dict
			local rst, other = self:creatDict(text);
			table.insert(tbl, rst);
			text = other;
		elseif firstByte == 116 then--true
			local rst, other = self:popTrue(text);
			table.insert(tbl, rst);
			text = other;
		elseif firstByte == 102 then--false
			local rst, other = self:popFalse(text);
			table.insert(tbl, rst);
			text = other;
		--elseif firstByte == 39 then--string
        elseif firstByte == 34 then--string
			local rst, other = self:popStr(text);
			table.insert(tbl, rst);
			text = other;
		elseif firstByte == endCharByte then--终止符 break
			text = string.sub(text, 2);
			return text;
		--elseif firstByte == 124 then--"|" continue
        elseif firstByte == 44 then--"|" continue
			text = string.sub(text, 2);
		else
			return text;
		end
	end
	
end

--清除字符串头部的空格符
function Parser:clearHeadSpace(text)
	local s, e = string.find(text, "^[%s]*");
	return string.sub(text, e + 1);
end

--清除字符串头部的第一个 冒号
function Parser:clearFirstColon(text)
	local s, e = string.find(text, "^[%s]*[:]");
	return string.sub(text, e + 1);
end

function Parser:peekFirstByte(text)
	text = self:clearHeadSpace(text);
	local firstByte = string.byte(text, 1);
	return firstByte, text;
end

function Parser:popStr(text)
	local s, e = string.find(text, '^["][^"]*["]');
	if e == nil then
		error("锅锅，你确定表里某处list或dict类型中的的string型数据没有写错吗？")
	end
	local str = string.sub(text, s + 1, e - 1);
	local other = string.sub(text, e + 1);
	return str, other;
end

function Parser:popNum(text)
	local s, e = string.find(text, "^[%-]?[%s]*[%d]*[%.]?[%d]*");
	local num = tonumber(string.sub(text, s, e));
	if num == nil then
		error("表中的数字 字符格式有误");
	end
	local other = string.sub(text, e + 1);
	return num, other;
end

function Parser:popTrue(text)
	local isSucceed = string.match(text, "^true");
	if isSucceed then
		return true, string.sub(text, 5);
	else
		error("锅锅，你确定表里某处的bool型数据没有写错吗？")
	end
end

function Parser:popFalse(text)
	local isSucceed = string.match(text, "^false");
	if isSucceed then
		return false, string.sub(text, 6);
	else
		error("锅锅，你确定表里某处的bool型数据没有写错吗？")
	end
end

return Parser