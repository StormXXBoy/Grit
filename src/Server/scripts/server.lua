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
end

function onNewClient(clientId)
	print(clientId)
end

function updateRecieved(clientId, data)
	print(clientId, data)
end

function preUpdateBroadcast()
	print("preUpdateBroadcast")
end