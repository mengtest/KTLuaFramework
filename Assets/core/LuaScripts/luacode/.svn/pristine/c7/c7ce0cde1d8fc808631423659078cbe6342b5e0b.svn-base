local M = { }

--[[ 如何使用：
 1.添加单个事件 - Unity端回调 on_animation_event
 local event_anima = require "common.util.event_anima"  
 event_anima.addSingleAnimationEvent(val,function()
       print("lua log")
 end)
 2.添加多个事件 - Unity端回调 on_animation_events(序号)
 local event_anima = require "common.util.event_anima"
 event_anima.addMutiAnimationEvent(val, 
 function()
     print("lua log 1")
 end ,
 function()
     print("lua log 2")
 end )
]]--

function M.addSingleAnimationEvent(go, func)
    local m_animation_event = go:GetComponent(KTAnimationEvent)
    m_animation_event.onEventCallBack = KTAnimationEvent_OnAnimationEvent()
    m_animation_event.onEventCallBack:AddListener(func)
end

function M.addMutiAnimationEvent(go, ...)
    local m_animation_event = go:GetComponent(KTAnimationEvent)
    local m_animation_event_list = m_animation_event.onEventCallBacks
    local fucs = { ...}
    for i, v in ipairs(fucs) do
        local l_event = KTAnimationEvent_OnAnimationEvent()
        l_event:AddListener( function()
            v()
        end )
        m_animation_event_list:Add(l_event)
    end
end




return M

