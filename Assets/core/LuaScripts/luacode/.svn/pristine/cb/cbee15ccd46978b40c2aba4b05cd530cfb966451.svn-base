
local HFSM = require("common.util.HFSM.HFSM")
local State_with_event_map = require("common.util.HFSM.State_with_event_map")
local HFSM_event = require("common.util.HFSM.HFSM_event")


HFSM_test = class("HFSM_test")

function HFSM_test:initialize()
	self.m_hfsm = HFSM:new()
	
	self:start()
end

function HFSM_test:start()
	self.m_hfsm:add_state("state0", self:state0())
	self.m_hfsm:add_state_in("state1", self:state1(), "state0")
	self.m_hfsm:add_state_in("state2", self:state2(), "state0")
	self.m_hfsm:add_state_in("state3", self:state3(), "state1")
	self.m_hfsm:add_state_in("state4", self:state4(), "state1")
	self.m_hfsm:add_state_in("state5", self:state5(), "state2")
	self.m_hfsm:add_state_in("state6", self:state6(), "state2")
	self.m_hfsm:add_state_in("state7", self:state7(), "state0")
	self.m_hfsm:add_state_in("state8", self:state8(), "state7")

	
	self.m_hfsm:init("state1")
end

function HFSM_test:update()
	
	--if Input.GetKeyDown(KeyCode.F)  then uerror("= - =!!!!"); end

	if Input.GetKeyDown(KeyCode.Alpha0) then
		self.m_hfsm:post("state0")
	elseif Input.GetKeyDown(KeyCode.Alpha1) then
		self.m_hfsm:post("state1")
	elseif Input.GetKeyDown(KeyCode.Alpha2) then
		self.m_hfsm:post("state2")
	elseif Input.GetKeyDown(KeyCode.Alpha3) then
		self.m_hfsm:post("state3")
	elseif Input.GetKeyDown(KeyCode.Alpha4) then
		self.m_hfsm:post("state4")
	elseif Input.GetKeyDown(KeyCode.Alpha5) then
		self.m_hfsm:post("state5")
	elseif Input.GetKeyDown(KeyCode.Alpha6) then
		self.m_hfsm:post("state6")
	elseif Input.GetKeyDown(KeyCode.Z) then
		self.m_hfsm:post("show1")
	elseif Input.GetKeyDown(KeyCode.X) then
		--self.m_hfsm:post("show2")
		local evt = HFSM_event:new()
		evt.msg = "show2"
		evt.obj = "this is obj"
		self.m_hfsm:post_event(evt)
	elseif Input.GetKeyDown(KeyCode.Alpha7) then
		self.m_hfsm:post("state7")
	elseif Input.GetKeyDown(KeyCode.Alpha8) then
		self.m_hfsm:post("state8")
	end
end

function HFSM_test:state0()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state0.on_start")
	end)
	state.on_over:add(function ()
		print("state0.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function ()
		print("state0.show1")
	end)
	state:add_event_action("show2", function ()
		print("state0.show2")
	end)
	return state
end

function HFSM_test:state1()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state1.on_start")
	end)
	state.on_over:add(function ()
		print("state1.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_state_action("show1", function ()
		print("state1.show1   StateAction")
		return "state6"
	end)
	state:add_event_action("show2", function (evt)
		print("state1.show2   EventAction" .. "\nevt.msg:" .. tostring(evt.msg) .. "\nevt.obj:" .. tostring(evt.obj))
	end)
	return state
end

function HFSM_test:state2()
	local state = State_with_event_map:new()
    state.m_test = 369
    function state:test_func()
        print("self.m_test2 = " .. self.m_test)
    end
    state.test_func2 = function (self)
        self.m_test = self.m_test - 69
        print("state:test_func2, rst =" .. self.m_test)
    end
	state.on_start:add(function ()
		print("state2.on_start")
	end)
	state.on_over:add(function ()
		print("state2.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function (self)
		print("state2.show1")
        print("state.m_test = " .. self.m_test)
        self:test_func()
	end)
	state:add_event_action("show2", function (self)
		print("state2.show2")
        self:test_func2()
	end)
	return state
end

function HFSM_test:state3()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state3.on_start")
	end)
	state.on_over:add(function ()
		print("state3.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function ()
		print("state3.show1")
	end)
	state:add_event_action("show2", function ()
		print("state3.show2")
	end)
	return state
end

function HFSM_test:state4()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state4.on_start")
	end)
	state.on_over:add(function ()
		print("state4.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function ()
		print("state4.show1")
	end)
	state:add_event_action("show2", function ()
		print("state4.show2")
	end)
	return state
end

function HFSM_test:state5()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state5.on_start")
	end)
	state.on_over:add(function ()
		print("state5.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function ()
		print("state5.show1")
	end)
	state:add_event_action("show2", function ()
		print("state5.show2")
	end)
	return state
end

function HFSM_test:state6()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state6.on_start")
	end)
	state.on_over:add(function ()
		print("state6.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")
	state:add_event_action("show1", function ()
		print("state6.show1")
	end)
	state:add_event_action("show2", function ()
		print("state6.show2")
	end)
	return state
end

function HFSM_test:state7()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state7.on_start")
	end)
	state.on_over:add(function ()
		print("state7.on_over")
	end)
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")	
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")

	return state
end

function HFSM_test:state8()
	local state = State_with_event_map:new()
	state.on_start:add(function ()
		print("state8.on_start")
	end)
	
	state.on_over:add(function ()
		print("state8.on_over")
	end)
	
	state:add_event("state0", "state0")
	state:add_event("state1", "state1")
	state:add_event("state2", "state2")
	state:add_event("state3", "state3")
	state:add_event("state4", "state4")
	state:add_event("state5", "state5")
	state:add_event("state6", "state6")
	state:add_event("state7", "state7")
	state:add_event("state8", "state8")

	return state
end

return HFSM_test