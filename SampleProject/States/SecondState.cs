using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace SampleProject.States;

public class SecondState : MonoGameLibrary.States.State {
    public override bool IsTransparent => true;
    public override void Update(GameTime gameTime) {
        // Empty update logic in this sample
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Use DarkSlateGray color to replace the last one
        Core.GraphicsDevice.Clear(Color.DarkSlateGray);
    }

    public override void HandleInput(GameTime gameTime, MonoGameLibrary.States.InputState inputState)
    {
        // When the Escape key is pressed, the request pops up the current state and returns to the previous state (possibly SecondState, in which case execution will continue until InitialState is returned).
        if (inputState.IsKeyPressed(Keys.Enter))
        {
            this.RequestPop();
        }
        
    }
}