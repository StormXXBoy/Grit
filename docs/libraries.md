# Grit Libraries

Grit exposes two primary Lua-accessible libraries: `Entity` and `Net`. These provide the core functionality for creating game objects, handling physics, and communicating over the network.

---

## Entity Library

The `Entity` library allows you to create and manage game entities. Entities are the building blocks of gameplay objects, platforms, players, and physics-enabled objects.

### Creation Functions

* `Entity.base()` – Creates a basic entity.
* `Entity.collision()` – Creates a static collision entity.
* `Entity.physics()` – Creates a physics-enabled entity.
* `Entity.player()` – Creates a player entity.
* `Entity.platform()` – Creates a platform entity.

### Adding and Removing Entities

* `addEntity(entity)` – Adds the entity to the game world.
* `removeEntity(entity)` – Removes the entity from the game world.

### Properties (common to most entities)

* `position : Vector` – World position.
* `velocity : Vector` – Current velocity.
* `acceleration : Vector` – Current velocity.
* `size : Vector` – Width and height.
* `center : Vector` – Center point of the entity.
* `sprite` – Sprite and color information.

### Methods

* `enity.isTouching(otherEntity)` – Returns whether two entities are colliding.
* `playerEntity.MoveHorizontal(dt, amount)` – Moves an entity horizontally.
* `entityJump(entity)` – Applies a jump force to a physics entity.

---

## Net Library

The `Net` library exposes networking utilities and multiplayer features.
This library can be used on both client and server unlike the Entity library which is only for the client!

### Core Components

* `Net.vector(x, y)` – Creates a network-safe vector.
* `Net.entity()` – Creates a network entity object for serialization.
* `Net.join(separator, ...)` – Joins multiple objects into a single string for transmission.
* `Net.split(separator, string)` – Splits a network string back into components, automatically converting vectors and entities.

### LuaClient

Use `LuaClient` to interact with a network client in Lua scripts.

#### Functions

* `client.listen(eventName, callback)` – Listens for events from the server. `callback` must be a Lua function.
* `client.fire(eventName, data)` – Sends a string payload to the server under the given event name.

### Example

```lua
-- Create a bullet and send it over the network
local netBullet = Net.entity()
netBullet.position = Net.vector(100, 200)
netBullet.velocity = Net.vector(0, -50)
networkClient:fire("shoot", netBullet:ToString())
```

These two libraries form the foundation of Grit Lua scripting, enabling entity management, game logic, and multiplayer networking directly from Lua scripts.
