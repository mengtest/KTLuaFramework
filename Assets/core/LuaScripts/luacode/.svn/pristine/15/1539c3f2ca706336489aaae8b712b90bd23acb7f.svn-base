---
--- Created by xjl.
--- DateTime: 2017/11/30 16:22
---
local lua_queue = class("lua_queue")

function lua_queue:initialize(capacity)
    self.capacity = capacity
    self.queue = {}
    self.size = 0
    self.head = -1
    self.rear = -1
end

function lua_queue:pushQueue(element)
    if self.size == 0 then
        self.head = 1
        self.rear = 1
        self.size = 1
        self.queue[self.rear] = element
    else
        local temp = (self.rear + 1) % self.capacity
        if temp == self.head then
            self:local_log("Error: capacity is full.")
            return
        else
            self.rear = temp
        end

        self.queue[self.rear] = element
        self.size = self.size + 1
    end

end

function lua_queue:popQueue()
    if self:isEmpty() then
        self:local_log("Error: The Queue is empty.")
        return
    end

    local value = self.queue[self.head]
    self.size = self.size - 1
    self.head = (self.head + 1) % self.capacity

    return value
end

function lua_queue:peekQueue()
    if self:isEmpty() then
        self:local_log("Error: The Queue is empty.")
        return
    end

    local value = self.queue[self.head]
    return value
end

function lua_queue:clear()
    self.queue = nil
    self.queue = {}
    self.size = 0
    self.head = -1
    self.rear = -1
end

function lua_queue:isEmpty()
    if self:get_size() == 0 then
        return true
    end
    return false
end

function lua_queue:get_size()
    return self.size
end

function lua_queue:printElement()
    local h = self.head
    local r = self.rear
    local str = nil
    local first_flag = true
    while h ~= r do
        if first_flag == true then
            str = "{" .. self.queue[h]
            h = (h + 1) % self.capacity
            first_flag = false
        else
            str = str .. "," .. self.queue[h]
            h = (h + 1) % self.capacity
        end
    end
    str = str .. "," .. self.queue[r] .. "}"
    print(str)
end

function lua_queue:local_log(...)
    app:logError(string.format(...))
end

function lua_queue:pop_average_vector3()
    local total_x = 0
    local total_y = 0
    local total_z = 0
    
    for i, v in ipairs(self.queue) do
        total_x = total_x + v.x
        total_y = total_y + v.y
        total_z = total_z + v.z
    end

    local count = #self.queue
    local average_x = total_x / count
    local average_y = total_y / count
    local average_z = total_z / count

    return Vector3(average_x, average_y, average_z)
end

function lua_queue:pushQueue_replace(element)
    if self.size == 0 then
        self.head = 1
        self.rear = 1
        self.size = 1
        self.queue[self.rear] = element
    else
        local temp = (self.rear + 1) % self.capacity
        if temp == self.head then
            self:popQueue()
            self.rear = temp
            self.queue[self.rear] = element
        else
            self.rear = temp
            self.queue[self.rear] = element
            self.size = self.size + 1
        end
    end
end

return lua_queue