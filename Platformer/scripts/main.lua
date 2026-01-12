local startEnt
local newBullet = Entity.physics()
local elevator = Entity.platform()

function init()
	startEnt = Entity.base();
	startEnt.sprite.color = color(Enum.Color.Lime);
	startEnt.position = Vector(200, 200);
	startEnt.size = Vector(15, 15);
	addEntity(startEnt);
	addEntity(newBullet);
	elevator.size = Vector(100, 20);
	addEntity(elevator);
end

function update(dt)
	--print(player.size, player.position)
	if player.isTouching(startEnt) then
		startEnt.sprite.color = color(Enum.Color.Red);
		else
		startEnt.sprite.color = color(Enum.Color.Lime);
	end
	elevator.center = Vector(250, 250 - math.sin(os.clock() * 0.5) * 200)
end

function onInput(inputInfo)
	--print(inputInfo.type, inputInfo.state, inputInfo.button, inputInfo.position)
	if (inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down and inputInfo.button == Enum.MouseButton.Left) then
		if input.isKeyDown(Enum.Key.E) then
			player.center = inputInfo.position
			player.velocity = Vector(0, 0)
			return
		end

		--player.size = player.size * 1.1;

		local shootDirection = inputInfo.position - player.position;

        newBullet.position = player.center - Vector(5, 10);
        newBullet.acceleration = player.acceleration;
        newBullet.velocity = player.velocity + (shootDirection.normalize() * 50);
        --player.velocity += (shootDirection.normalize() * 50);

		newBullet.size = Vector(10, 10);
	end

	if (inputInfo.key == Enum.Key.T and inputInfo.state == Enum.InputState.Down) then
		--removeEntity(elevator)
		for i, v in pairs(Entities.collision()) do
			print(i, v)
			removeEntity(v)
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