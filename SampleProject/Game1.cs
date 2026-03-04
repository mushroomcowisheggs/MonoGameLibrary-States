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

    // A public static property that allows other classes within the project (such as State) to safely access the state stack
    public static MonoGameLibrary.States.StateStack StateStack { get; private set; }
    
    public Game1() : base("Dungeon Slime - States Demo", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        // Initialize StateStack and InputState manager
        _stateStack = new MonoGameLibrary.States.StateStack();
        _inputState = new MonoGameLibrary.States.InputState();
        StateStack = _stateStack;

        // Push the initial state into the stack
        _stateStack.Push(new States.InitialState()); // SampleProject.States

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        // 1. Update input state(record current frame and last frame)
        _inputState.Update();
        
        // 2. Update state stack. The stack is responsible for invoking the Update and HandleInput of the current active state.
        _stateStack.Update(gameTime, _inputState);

        // Hand over the return function to the InitialState HandleInput() management
        /*
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }
        */
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // The state stack is responsible for invoking the draw method of the current activity state.
        // It follows the IsTransparent and IsBlocking rules to handle the cascading of multiple states.
        _stateStack.Draw(gameTime, Core.SpriteBatch);

        base.Draw(gameTime);
    }
}