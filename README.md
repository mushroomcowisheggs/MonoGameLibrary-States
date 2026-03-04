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

## Example
Let's look at a concrete example to understand how to use the MonoGameLibrary.States​ module to build a simple application flow. This example will have two screens (InitialStateand SecondState) and demonstrate state transitions and overlay effects. When your game `Game1` inherits from `MonoGameLibrary.Core`, like the condition at the end of [MonoGame.Samples/Tutorials/learn-monogame-2d/src/04-Creating-A-Class-Library](https://github.com/MonoGame/MonoGame.Samples/tree/3.8.4/Tutorials/learn-monogame-2d/src/04-Creating-A-Class-Library). 
### 1. Setting up the Game Class (Game1.cs)
Your main game class inherits from `MonoGameLibrary.Core`. It is responsible for initializing the `StateStack` and `InputState`, and delegates the update and draw calls to the stack.

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace SampleProject;

public class Game1 : Core
{
    // Declare StateStack and InputState manager
    private MonoGameLibrary.States.StateStack _stateStack;
    private MonoGameLibrary.States.InputState _inputState;

    // A public static property for safe access to the state stack from other classes (e.g., States)
    public static MonoGameLibrary.States.StateStack StateStack { get; private set; }

    public Game1() : base("Dungeon Slime - States Demo", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        // Initialize StateStack and InputState manager
        _stateStack = new MonoGameLibrary.States.StateStack();
        _inputState = new MonoGameLibrary.States.InputState();
        StateStack = _stateStack; // Make it accessible globally

        // Push the initial state (e.g., a splash screen or main menu)
        _stateStack.Push(new States.InitialState());

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        // 1. Update the input state (records current and previous frame)
        _inputState.Update();
        
        // 2. Delegate all game logic updating to the state stack.
        // The stack will call Update/HandleInput on the active states.
        _stateStack.Update(gameTime, _inputState);
        
        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        // Delegate all rendering to the state stack.
        // It follows the IsTransparent and IsBlocking rules for layered drawing.
        _stateStack.Draw(gameTime, Core.SpriteBatch);

        base.Draw(gameTime);
    }
}

```

### 2. Defining the Initial State (InitialState.cs)
The first state loaded. It sets a blue background and waits for player input to push a new state onto the stack.

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace SampleProject.States;

public class InitialState : MonoGameLibrary.States.State {
    public override void Update(GameTime gameTime) {
        // Empty update logic in this sample
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        // Set the screen to CornflowerBlue
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);
    }

    public override void HandleInput(GameTime gameTime, MonoGameLibrary.States.InputState inputState) {
        // When SPACE is pressed, push the SecondState onto the stack (if it's not already on top).
        if (inputState.IsKeyPressed(Keys.Space)) {
            if (!MonoGameLibrary.States.StateStackExtensions.IsInState<SecondState>(Game1.StateStack))
            {
                this.RequestPush(new SecondState());
            }
        }
        // ESC or Back button exits the game
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }
    }
}

```

### 3. Defining the Second State (SecondState.cs)
A state that is pushed on top of the `InitialState`. It is **transparent**, meaning the state below it (`InitialState`) will still be drawn, but will be covered by this state's new clear color. Pressing `ENTER` will pop this state, returning to the initial one.

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace SampleProject.States;

public class SecondState : MonoGameLibrary.States.State {
    // This state is transparent. The Draw() of the state below (InitialState) will be called first.
    public override bool IsTransparent => true;
    public override void Update(GameTime gameTime) {
        // Empty update logic in this sample
    }
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Overwrites the screen with DarkSlateGray, creating an overlay effect.
        Core.GraphicsDevice.Clear(Color.DarkSlateGray);
    }
    public override void HandleInput(GameTime gameTime, MonoGameLibrary.States.InputState inputState)
    {
        // When ENTER is pressed, pop the current state (SecondState) from the stack, returning to InitialState.
        if (inputState.IsKeyPressed(Keys.Enter))
        {
            this.RequestPop();
        }
    }
}

```

### Execution Flow Explanation
1.  **Startup**: The game runs, `Game1` initializes and pushes `InitialState` onto the state stack. The screen displays `CornflowerBlue`.
2.  **Pushing a New State**: Pressing the `SPACE` key in `InitialState` triggers `this.RequestPush(new SecondState())`.
    *   The `StateStack` pushes `SecondState` onto the top of the stack, making it the active state.
    *   Since `SecondState` has its `IsTransparent` property set to `true`, `StateStack.Draw()` will first draw the underlying `InitialState` (blue), then draw the top `SecondState` (DarkSlateGray). The latter clears the entire screen, so you will see DarkSlateGray.
3.  **Popping a State**: Pressing the `ENTER` key in `SecondState` triggers `this.RequestPop()`.
    *   The `StateStack` removes `SecondState` from the stack.
    *   `InitialState` becomes the top active state again, and the screen reverts to `CornflowerBlue`.

This simple example clearly demonstrates how the state stack manages screens, how event-driven transitions (`RequestPush`/`RequestPop`) enable safe state changes, and how the `IsTransparent` property affects the final rendering outcome. You can easily build complex interface flows like main menus, pause screens, and game levels on top of this foundation.

## More Than Management, It's Empowerment
Beyond core state management, this module includes a practical `InputState` class. It encapsulates MonoGame's input while also recording the state of the current and previous frames. This makes implementing edge-detection logic like "key was just pressed" (`WasKeyJustPressed`) extremely easy, as seen in `SplashState.HandleInput`.

The module's design also considers synergy with the main **MonoGame Library**. You can directly access singleton instances like `Core.Graphics`, `Core.Input`, `Core.Audio`, etc., from within any `State`, easily using advanced features provided by the main library such as texture atlases, camera systems, and audio control, achieving a `1+1 > 2` development experience.

## Conclusion: Keep Your Game Architecture Clear and Maintainable
The **MonoGameLibrary.States** module, by introducing the state stack pattern, clearly divides control of the game flow into separate, independent `State`s. It enforces a good code organization, making different parts of the game loosely coupled and highly cohesive. Whether it's simple scene transitions or complex interface overlays (like in-game menus + dialogue bubbles), this system handles them elegantly.

Its event-driven transition mechanism ensures safety, while the `IsTransparent` and `IsBlocking` properties provide pixel-perfect control over presentation. For developers using MonoGame who wish to improve their project structure, integrating this lightweight yet powerful state management module is a choice worth considering.
