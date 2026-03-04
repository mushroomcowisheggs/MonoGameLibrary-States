using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace SampleProject.States; // Here should be the namespace of your project

public class InitialState : MonoGameLibrary.States.State {
    public override void Update(GameTime gameTime) {
        // Empty update logic in this sample
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        // Use CornflowerBlue to clear the screen as the view of this state.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);
    }

    public override void HandleInput(GameTime gameTime, MonoGameLibrary.States.InputState inputState) {
        // Request for SecondState when space is pressed and the top state in the stack is not SecondState.
        if (inputState.IsKeyPressed(Keys.Space)) {
            if (!MonoGameLibrary.States.StateStackExtensions.IsInState<SecondState>(Game1.StateStack))
            {
                this.RequestPush(new SecondState());
            }
        }
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }
    }
}