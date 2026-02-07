function loopT(table)
	for i, v in pairs(table) do
		if type(v) == "table" then
			loopT(v)
		end
		print(i, v)
	end
end

local startEnt
local newBullet = Entity.physics()
local elevator = Entity.platform()
local npc = Entity.player()
local networkClient = nil
local bullets = {}
local task = require("taskService")
local tagService = require("tagService")
local particleService = require("particleService")
local shartParticles = particleService.new()

function init()
	particleService.init(delayService)
	shartParticles.color = color(Enum.Color.Brown)
	shartParticles.velocity = 3

	startEnt = Entity.base();
	startEnt.sprite.color = color(Enum.Color.Lime);
	startEnt.position = Vector(200, 200);
	startEnt.size = Vector(15, 15);
	addEntity(startEnt);
	addEntity(newBullet);
	elevator.size = Vector(100, 20);
	addEntity(elevator);
	addEntity(npc)
	tagService.addTag(npc, "npc")
	local remBut
	remBut = addButton("remove npc", function()
		for _, ent in pairs(tagService.getAllObjectsWithTag("npc")) do
			removeEntity(ent)
			tagService.removeTag(ent)
		end
		remBut()
	end)
	addButton("zoomies", function()
		player.speed = player.speed * 2
	end)
end

function createBlock(pos, size)
	local newBlock = Entity.platform()
	newBlock.position = pos
	newBlock.size = size
	addEntity(newBlock);
end

function serverConnected(client)
	networkClient = client
	removeEntity(newBullet)
	removeEntity(elevator)
	removeEntity(npc)
	client.listen("test", function(data)
		print(data)
	end)
	client.listen("shoot", function(data)
		local parts = string.split(data, "+")
		local netBullet = Net.entity()
		netBullet.UpdateFromString(parts[2])
		local bullet = Entity.physics()

		bullet.position = Vector(netBullet.position.X, netBullet.position.Y)
		bullet.velocity = Vector(netBullet.velocity.X, netBullet.velocity.Y)
		bullet.size = Vector(10, 10)

		bullets[bullet] = os.clock()
		task.delay(5, function()
			removeEntity(bullet)
			bullets[bullet] = nil
		end)

		addEntity(bullet)
	end)
	client.listen("place", function(data)
		local parts = Net.split("+",data)
		local pos = Vector(parts[1].X, parts[1].Y)
		local size = Vector(parts[2].X, parts[2].Y)
		createBlock(pos, size)
	end)
end

function update(dt)
	--print(player.size, player.position)
	if player.isTouching(startEnt) then
		startEnt.sprite.color = color(Enum.Color.Red);
	else
		startEnt.sprite.color = color(Enum.Color.Lime);
	end
	elevator.center = Vector(250, 250 - math.sin(os.clock() * 0.5) * 200)
	if math.abs(npc.center.x - player.center.x) > 20 then
		npc.MoveHorizontal(dt, player.center.x - npc.center.x)
		entityJump(npc)
	end

	task.update(dt)
end

local bufferPoint = nil
function onInput(inputInfo)
	--print(inputInfo.type, inputInfo.state, inputInfo.button, inputInfo.position)
	if (inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down) then
		if inputInfo.button == Enum.MouseButton.Left then
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

			if networkClient ~= nil then
				local netBullet = Net.entity()
				netBullet.position = Net.vector(newBullet.position.X, newBullet.position.Y)
				netBullet.velocity = Net.vector(newBullet.velocity.X, newBullet.velocity.Y)
				networkClient:fire("shoot", netBullet:ToString())
			end

			--player.velocity += (shootDirection.normalize() * 50);

			newBullet.size = Vector(10, 10);
		elseif inputInfo.button == Enum.MouseButton.Right then
            if not bufferPoint then
                bufferPoint = inputInfo.position;
            else
                local start = bufferPoint;
                local endPoint = inputInfo.position;

                local x = math.min(start.X, endPoint.X);
                local y = math.min(start.Y, endPoint.Y);
                local w = math.abs(endPoint.X - start.X);
                local h = math.abs(endPoint.Y - start.Y);

				if networkClient then
					local netpos = Net.vector(x, y)
					local netsize = Net.vector(w, h)
					networkClient.fire("place", Net.join("+", netpos, netsize))
				else
					createBlock(Vector(x, y), Vector(w, h))
				end
                bufferPoint = nil;
			end
		end
	end

	if (inputInfo.key == Enum.Key.T and inputInfo.state == Enum.InputState.Down) then
		--removeEntity(elevator)
		for i, v in pairs(Entities.collision()) do
			print(i, v)
			removeEntity(v)
		end
	end

	if (inputInfo.key == Enum.Key.F and inputInfo.state == Enum.InputState.Down) then
		shartParticles.position = player.center
		shartParticles:emit(10)
		if networkClient then
			networkClient:fire("message", "*farts*")
		end
	end
end

function onNewMessage(message)
	if message == "!info" then
		task.spawn(function()
			task.wait(1)
			addMessage("Grit Project is a 2d platformer sandbox engine/game with multiplayer support writen in C# with WinForms where you can script with lua!")
		end)
	elseif message:sub(1,6) == "!exec " then
		loadstring(string.sub(message, 7, #message))()
	end
end

--loopT(Enum)