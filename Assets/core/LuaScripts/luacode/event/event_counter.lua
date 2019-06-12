
local Event_counter = {}

Event_counter.m_cur_event_value = 0;

function Event_counter:get_event_value()
	self.m_cur_event_value = self.m_cur_event_value + 1;
	return self.m_cur_event_value
end

return Event_counter