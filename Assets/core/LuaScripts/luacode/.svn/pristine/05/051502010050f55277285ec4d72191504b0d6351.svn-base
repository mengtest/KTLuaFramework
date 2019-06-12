
local UtilLog = {}

-- 将个人的mask标签添加于此处，并设值为false上传至服务器。
UtilLog.Mask = {
    Global = true,
    SJG_Temp = false,
    SJG_Quest = false,
    SJG_Map = false,
    SJG_Look = false,
    SJG_Quiz = false,
}

function UtilLog:log(mask, format_str, ...)
    if not mask then return end
    Debug.Log(string.format(format_str, ...))
end

function UtilLog:logWarning(mask, format_str, ...)
    if not mask then return end
    Debug.LogWarning(string.format(format_str, ...))
end

function UtilLog:logError(mask, format_str, ...)
    if not mask then return end
    Debug.LogError(string.format(format_str, ...))
end

return UtilLog