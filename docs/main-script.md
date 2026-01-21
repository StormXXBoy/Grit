# Main Script

In Grit, the `main.lua` script serves as the **entry point** for your client-side Lua logic. This is where your gameplay initialization and scripting logic begins.

## Recommended Practices

* **Use an `init()` function** to set up your script. Avoid running code before this function, as the engine expects initialization to occur inside it.
* The `init()` function is called automatically by the engine once the Lua script is loaded.
* Optional functions you can define for your script:

  * `update(dt)` – runs every frame with `dt` representing delta time.
  * `onInput(inputInfo)` – called whenever input events occur and sends a `InputInfo` object with it..
  * `serverConnected(client)` – called when you connect to a server and sends a `LuaClient` object with it.

## Example Structure

```lua
-- main.lua

function init()
    -- Create base entities, players, or platforms
end

function update(dt)
    -- Handle per-frame updates, movement, or game logic
end

function onInput(inputInfo)
    -- Respond to player input
end

function serverConnected(client)
    -- Setup networking event listeners
end
```

By following this structure, your `main.lua` will act as a clean and maintainable entry point for client-side scripting in Grit.
