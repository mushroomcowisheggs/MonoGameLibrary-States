# MonoGameLibrary-States
A States extension of MonoGame extension module MonoGameLibrary for managing game screens and modes using a stack-based state system, enabling clean transitions, overlays, and structured game flow.

## Build a Clear and Smooth Flow for Your MonoGame Game: The MonoGameLibrary.States Extension Module
Managing transitions and overlays between different screens and modes (like the main menu, game levels, pause interface, and end screen) is a common but easily cluttered challenge in game development. This **MonoGameLibrary.States** extension module is designed specifically to solve this problem. It provides a lightweight, extensible architecture based on a State Stack, allowing you to combine and manage the various "states" in your game as if building with blocks.

## Core Idea: Everything is a State
This module abstracts each game interface or mode as a `State`. A `State` is a complete, self-contained logic unit that knows how to initialize (`Enter`), update (`Update`), draw (`Draw`), and clean up (`Exit`). Whether it's a splash animation lasting a few seconds, a complex main game loop, or a simple pause menu, it can be defined as a `State`.

The `State` base class provides several key lifecycle methods:
- `Enter()` / `Exit()`: Called when the state is activated or destroyed, used for resource loading/unloading.
- `Update(GameTime)`: Handles per-frame game logic.
- `Draw(GameTime, SpriteBatch)`: Handles per-frame rendering.
- `HandleInput(GameTime, InputState)`: (Optional) Handles input specific to that state.

Usefully, each state also has two properties to control its behavior:
- `IsTransparent`: If `true`, the lower states are also drawn during rendering. This makes it easy to achieve effects like "a translucent pause menu covering the game screen."
- `IsBlocking`: If `true`, it prevents the `Update` and `HandleInput` methods of lower states from being called. This means that when a modal dialog pops up, the background game world will automatically "pause."

## The State Stack: An Intelligent Layer Manager
A single state has limited power, and the `StateStack` class is where they are organized. It maintains a stack of states (Last-In-First-Out), with the top state being the currently most "active" one. The `StateStack` is responsible for calling the update methods of states from top to bottom and the draw methods from bottom to top, following the rules of `IsTransparent` and `IsBlocking`. This automatically handles complex layered rendering and logic freezing.

Transitions between states are event-driven, safe, and flexible. Inside any state, you can call `RequestChange(newState)` to clear the stack and switch to a completely new state (e.g., from the main menu to the game), or call `RequestPush(menuState)` to overlay a new state on the current one (e.g., opening an inventory), or call `RequestPop()` to close the current state (e.g., closing the menu to return to the game). All transition requests are queued and processed at safe times, avoiding errors that could arise from directly modifying the stack structure within the update loop.

## Example: From Splash Screen to Main Game Loop
Let's see how it works with a practical example. Assume your game `Game1` inherits from `MonoGameLibrary.Core`. 

## More Than Management, It's Empowerment
Beyond core state management, this module includes a practical `InputState` class. It encapsulates MonoGame's input while also recording the state of the current and previous frames. This makes implementing edge-detection logic like "key was just pressed" (`WasKeyJustPressed`) extremely easy, as seen in `SplashState.HandleInput`.

The module's design also considers synergy with the main **MonoGame Library**. You can directly access singleton instances like `Core.Graphics`, `Core.Input`, `Core.Audio`, etc., from within any `State`, easily using advanced features provided by the main library such as texture atlases, camera systems, and audio control, achieving a `1+1 > 2` development experience.

## Conclusion: Keep Your Game Architecture Clear and Maintainable
The **MonoGameLibrary.States** module, by introducing the state stack pattern, clearly divides control of the game flow into separate, independent `State`s. It enforces a good code organization, making different parts of the game loosely coupled and highly cohesive. Whether it's simple scene transitions or complex interface overlays (like in-game menus + dialogue bubbles), this system handles them elegantly.

Its event-driven transition mechanism ensures safety, while the `IsTransparent` and `IsBlocking` properties provide pixel-perfect control over presentation. For developers using MonoGame who wish to improve their project structure, integrating this lightweight yet powerful state management module is a choice worth considering.
