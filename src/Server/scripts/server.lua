function init()
	print("Lua started!")

	server.listen("test", function(clientId, data)
		print(clientId, data)
	end)

	server.listen("shoot", function(clientId, data)
		local shootData = clientId .. "+" .. data
		server.fireAllClients("shoot", shootData)
	end)

	server.listen("place", function(clientId, data)
		server.fireAllClients("place", data)
	end)

	server.listen("message", function(clientId, data)
        local split = string.split(clientId, "-")[2];
		if data == "!test" then
			server:fireAllClients("message", split.." is testin boi!")
		else
            server:fireAllClients("message", split..": "..data);
		end
	end)
end

function onNewClient(clientId)
	print(clientId)
end

function preUpdateBroadcast()
	print("preUpdateBroadcast")
end