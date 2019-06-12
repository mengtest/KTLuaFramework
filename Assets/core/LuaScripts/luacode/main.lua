-- mobdebug = require("mobdebug")
-- mobdebug.start()
-- mobdebug.off()

import 'UnityEngine'
--import 'HedgehogTeam.EasyTouch'
--import 'DG.Tweening'
require "common.comm_init"

-- foreach C# ienumerable
function foreach(csharp_ienumerable)
    return Slua.iter(csharp_ienumerable)
end

-- dbc = TableLoader.loadAll()

local Application = require "Application"
app = Application:new()

local UtilityFunction=require"UtilityFunction"
utilityFunc=UtilityFunction:new()

local pb = require "pb"
local protoc = require "network.protoc"

print(1111111111)
assert(pb.loadfile "Assets/BuildRes/PbProtocols/addressbook.pb") -- 载入刚才编译的pb文件

local person = { -- 我们定义一个addressbook里的 Person 消息
   name = "Alice",
   id = 12345,
   phone = {
      { number = "1301234567" },
      { number = "87654321", type = "WORK" },
   }
}

-- 序列化成二进制数据
local data = assert(pb.encode("tutorial.Person", person))

-- 从二进制数据解析出实际消息
local msg = assert(pb.decode("tutorial.Person", data))
print(222222222)

print(msg.name)
-- 打印消息内容（使用了serpent开源库）
print(require "common.util.serpent".block(msg))
-- load schema from text
-- assert(protoc:load [[
--    message Phone {
--       optional string name        = 1;
--       optional int64  phonenumber = 2;
--    }
--    message Person {
--       optional string name     = 1;
--       optional int32  age      = 2;
--       optional string address  = 3;
--       repeated Phone  contacts = 4;
--    } ]])
-- print(22222222222)
-- -- lua table data
-- local data = {
--    name = "ilse",
--    age  = 18,
--    contacts = {
--       { name = "alice", phonenumber = 12312341234 },
--       { name = "bob",   phonenumber = 45645674567 }
--    }
-- }
-- print(3333333333)
-- -- encode lua table data into binary format in lua string and return
-- local bytes = assert(pb.encode("Person", data))
-- print(pb.tohex(bytes))

-- -- and decode the binary data back into lua table
-- local data2 = assert(pb.decode("Person", bytes))
-- print(require "common.util.serpent".block(data2))
-- --print(data2.name..tostring(data2.age))

print(44444444444)

update = function()
	app:Update()
end

fixed_update = function()
	app:FixedUpdate()
end

late_update = function()
	app:LateUpdate()
end