using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.States
{
    public class InputState
    {
        public KeyboardState CurrentKeyboardState;
        public KeyboardState PreviousKeyboardState;
        public GamePadState CurrentGamePadState;
        public GamePadState PreviousGamePadState;
        public MouseState CurrentMouseState;
        public MouseState PreviousMouseState;

        public void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            PreviousGamePadState = CurrentGamePadState;
            PreviousMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
        }
        // ...
    }
}