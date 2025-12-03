local startEnt
local newBullet = Entity.physics()

function init()
	startEnt = Entity.base();
	startEnt.sprite.color = color(Enum.Color.Lime);
	startEnt.position = Vector(200, 100);
	startEnt.size = Vector(15, 15);
	addEntity(startEnt);
	addEntity(newBullet);
end

function  update(dt)
	--print(player.size, player.position)
	if (newBullet.center - startEnt.center).magnitude() < 15 then
		player.position = Vector(0, 500)
		player.velocity = Vector(0, 0)
	end
end

function onInput(inputInfo)
	--print(inputInfo.type, inputInfo.state, inputInfo.button, inputInfo.position)
	if (inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down and inputInfo.button == Enum.MouseButton.Left) then
		if input.isKeyDown(Enum.Key.E) then
			player.center = inputInfo.position
			player.velocity = Vector(0, 0)
			return
		end

		local shootDirection = inputInfo.position - player.position;

        newBullet.position = player.center - Vector(5, 10);
        newBullet.acceleration = player.acceleration;
        newBullet.velocity = player.velocity + (shootDirection.normalize() * 50);
        --player.velocity += (shootDirection.normalize() * 50);

		newBullet.size = Vector(10, 10);
	end

	if (inputInfo.key == Enum.Key.T and inputInfo.state == Enum.InputState.Down) then
		for i, v in pairs(entities()) do
			print(i, v)
		end
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