function init()
	print("Test")
	local v = Vector(100, 200)
	print(v)
	--player.size = v
	print(tostring(player.size.X))
	print(player.size, player.position)
	--player.size.X = 40;
	--while true do
		print("testin!!")
	--end
end

function  update(dt)
	--print(player.size, player.position)
end

function onInput(inputInfo)
	print(inputInfo.type, inputInfo.state, inputInfo.button, inputInfo.position)
	if (inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down and inputInfo.button == Enum.MouseButton.Left) then
		if input.isKeyDown(Enum.Key.E) then
			player.center = inputInfo.position
			player.velocity = Vector(0, 0)
			return
		end

		local shootDirection = inputInfo.position - player.position;

        local newBullet = Entity.physics();

        newBullet.position = player.center - Vector(5, 10);
        newBullet.acceleration = player.acceleration;
        newBullet.velocity = player.velocity + (shootDirection.normalize() * 50);
        --player.velocity += (shootDirection.normalize() * 50);

		newBullet.size = Vector(10, 10);

		addEntity(newBullet);
	end
	
	if (inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down and inputInfo.button == Enum.MouseButton.Right) then
		--player.size = player.size * 0.5
	end
end

function loopT(table)
	for i, v in pairs(table) do
		if type(v) == "table" then
			loopT(v)
		end
		print(i, v)
	end
end

--loopT(Enum)

print("main.lua loaded")