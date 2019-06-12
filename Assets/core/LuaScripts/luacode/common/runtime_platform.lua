local RuntimePlatform = {}

-- 运行时平台 
function RuntimePlatform.is_in_touchable_platform()
    local platform = Application.platform
    return platform == RuntimePlatform.Android or 
        platform == RuntimePlatform.IPhonePlayer or
        platform == RuntimePlatform.tvOS or
        platform == RuntimePlatform.BlackBerryPlayer
end

return RuntimePlatform