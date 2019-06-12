local ObserverHost = {}

function ObserverHost:subscribe(observable, func)
	self.m_subscription = self.m_subscription or {}
	local subscription = observable:subscribe(func, function(err)
		app:logError(err)
	end)
	assert(subscription)
	table.insert(self.m_subscription, subscription)
	return subscription
end

function ObserverHost:unsubscribe(conn)
	for i,v in ipairs(self.m_subscription) do
		if v == conn then
			table.remove(self.m_subscription, i)
			conn:unsubscribe()
			return true
		end
	end
end

function ObserverHost:unsubscribeAll()
	if self.m_subscription == nil then return end
	for _,v in ipairs(self.m_subscription) do
		v:unsubscribe()
	end
	self.m_subscription = nil
end

return ObserverHost