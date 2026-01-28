local particleService = {}

function particleService.init(delayService)
    particleService._delayService = delayService
end

function particleService.new(delayService)
    local self = setmetatable({}, {__index = particleService})

    self._entities = {}
    self.position = Vector(0, 0)
    self.size = Vector(3, 3)
    self.color = color(Enum.Color.White)
    self.velocity = 1
    self.lifetime = 1

    return self
end

function particleService:emit(count)
    if count > 100 then
        print("Emitting a large number of particles may impact performance.")
    end
    for i = 1, count do
        local newEntity = Entity.physics()
        newEntity.position = Vector(self.position.x, self.position.y)
        newEntity.size = self.size
        newEntity.sprite.color = self.color
        newEntity.velocity = Vector(
            math.random(-self.velocity*100, self.velocity*100) / 100,
            math.random(-self.velocity*200, self.velocity*100) / 100
        )

        particleService._delayService.addDelay(function()
            removeEntity(newEntity)
        end, self.lifetime + math.random() * 0.1)

        table.insert(self._entities, newEntity)
        addEntity(newEntity)
    end
end

return particleService