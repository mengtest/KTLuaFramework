local UtilTable = {}

-- 以值为索引移除表中的元素
-- tbl(table):待移除制定元素的表
-- val(anything):待移除的制定元素
-- return (bool):是否成功删除val
function UtilTable:remove_by_val(tbl, val)
    for k, v in pairs(tbl) do
        if v == val then
            table.remove(tbl, k)
            return true
        end
    end
    return false
end

--往list里插入元素，不允许重复
--by lijunfeng 2018/3/5
function UtilTable:insert(tbl,val)
    if not self:is_contain(tbl,val) then
        table.insert(tbl,val)
    end
end

-- 将tbl_in中的所有元素尾插入到tbl_out中(tbl_out和tbl_in必须都是有序列表，如果对字典型数据使用将会丢失信息)
-- tbl_out(table)
-- tbl_in(table)
function UtilTable:insert_range(tbl_out, tbl_in)
    if tbl_out and tbl_in and type(tbl_out) == "table" and type(tbl_in) == "table" then
        for k, v in ipairs(tbl_in) do
            table.insert(tbl_out, v)
        end
    else
        print("请传入有效的参数数据")
    end
end

-- 判断字典或列表table中是否存在val
-- tbl(table):
-- val(anything):
function UtilTable:is_contain(tbl, val)
    for k, v in pairs(tbl) do
        if v == val then
            return true,k --by lijunfeng 2018/3/3
        end
    end
    return false
end

-- 获取tbl的元素总个数
function UtilTable:count(tbl)
    local count = 0
    for k, v in pairs(tbl) do
        count = v ~= nil and count + 1 or count
    end
    return count
end

-- becopyed  被复制的表 （type~~list）
-- copy  复制的表
function UtilTable:copy_list(becopyed, copy)
    for i, v in ipairs(becopyed) do
        copy[i] = v
    end
end

-- 判断两个表的元素是否相同
function UtilTable:are_the_tabs_same(tab1, tab2)
    if tab1 == tab2 then
        return true
    end
    if UtilTable:count(tab1) ~= UtilTable:count(tab2) then
        return false
    end

    local tab1_copy = {}
    for i, v in pairs(tab1) do
        tab1_copy[i] = v
    end
    local tab2_copy = {}
    for i, v in pairs(tab2) do
        tab2_copy[i] = v
    end

    local t = {}

    for i, v in pairs(tab1_copy) do
        if UtilTable:is_contain(tab2_copy, v) then
            table.insert(t, v)
        else
            return false
        end
    end

    for i, v in pairs(t) do
        UtilTable:remove_by_val(tab1_copy, v)
        UtilTable:remove_by_val(tab2_copy, v)
    end

    if UtilTable:count(tab1_copy) ~= 0 or UtilTable:count(tab2_copy) ~= 0 then
        return false
    end

    return true
end

-- tab1 是否包含 tab2 中的所有元素
function UtilTable:is_contain_tab(tab1, tab2)
    if UtilTable:count(tab1) == 0 or UtilTable:count(tab2) == 0 then
        return false
    end
    for i, v in pairs(tab2) do
        if not UtilTable:is_contain(tab1, v) then
            return false
        end
    end
    return true
end

-- tab2中是否有元素存在于tab1中
function UtilTable:tab1_is_contain_one_of_tab2s_element(tab1, tab2)
    if UtilTable:count(tab1) == 0 or UtilTable:count(tab2) == 0 then
        return false
    end
    for i, v in pairs(tab2) do
        if UtilTable:is_contain(tab1, v) then
            return true
        end
    end
    return false
end

return UtilTable