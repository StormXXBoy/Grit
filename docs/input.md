# Input

Grit exposes a unified input system through the `InputHandler` and the `InputInfo` object, enabling Lua scripts to respond to both keyboard and mouse events.

## Input Object

The `InputInfo` object is passed to the Lua callback `onInput(inputInfo)` and contains the following properties:

* `inputInfo.type : Enum.InputType` – The type of input (`Key` or `Mouse`).
* `inputInfo.state : Enum.InputState` – The state of the input (`Down`, `Pressed`, `Up`).
* `inputInfo.key : Enum.Key` – The keyboard key pressed (valid if `type == Key`).
* `inputInfo.button : Enum.MouseButton` – The mouse button pressed (valid if `type == Mouse`).
* `inputInfo.position : Vector` – The position of the mouse in world coordinates (valid for mouse input).

## Input Enums

### Enum.InputType

* `Key` – Keyboard input.
* `Mouse` – Mouse input.

### Enum.InputState

* `Down` – Button or key was pressed this frame.
* `Pressed` – Button or key is currently being held down.
* `Up` – Button or key was released this frame.

### Enum.Key

* Standard keyboard keys as defined by the Windows `Keys` enum (e.g., `A`, `Space`, `Escape`).

### Enum.MouseButton

* `Left` – Left mouse button.
* `Right` – Right mouse button.
* `Middle` – Middle mouse button (wheel click).

## Example Usage

```lua
function onInput(inputInfo)
    if inputInfo.type == Enum.InputType.Mouse and inputInfo.state == Enum.InputState.Down then
        if inputInfo.button == Enum.MouseButton.Left then
            print("Left mouse clicked at: " .. inputInfo.position.X .. ", " .. inputInfo.position.Y)
        end
    end

    if inputInfo.type == Enum.InputType.Key and inputInfo.state == Enum.InputState.Down then
        if inputInfo.key == Enum.Key.Space then
            print("Space key pressed")
        end
    end
end
```

This system allows scripts to handle all user input events in a consistent and flexible way, enabling custom controls, player interactions, and tools within the Grit engine.
