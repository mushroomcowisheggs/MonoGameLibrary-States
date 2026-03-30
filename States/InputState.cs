using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.States {
    /// <summary>
    /// Captures and provides access to keyboard, gamepad, and mouse input states across frames.
    /// </summary>
    public class InputState {
        
        /// <summary>
        /// The current keyboard state.
        /// </summary>
        public KeyboardState CurrentKeyboardState;
        
        /// <summary>
        /// The previous frame's keyboard state.
        /// </summary>
        public KeyboardState PreviousKeyboardState;
        
        /// <summary>
        /// The current gamepad state (PlayerIndex.One).
        /// </summary>
        public GamePadState CurrentGamePadState;
        
        /// <summary>
        /// The previous frame's gamepad state.
        /// </summary>
        public GamePadState PreviousGamePadState;
        
        /// <summary>
        /// The current mouse state.
        /// </summary>
        public MouseState CurrentMouseState;
        
        /// <summary>
        /// The previous frame's mouse state.
        /// </summary>
        public MouseState PreviousMouseState;
        
        /// <summary>
        /// Updates the input states by capturing the latest hardware state and storing the previous states.
        /// </summary>
        public void Update() {
            PreviousKeyboardState = CurrentKeyboardState;
            PreviousGamePadState = CurrentGamePadState;
            PreviousMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();
        }
        
        /// <summary>
        /// Determines whether a specific key was pressed this frame (down now, up last frame).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is currently down and was up in the previous frame; otherwise false.</returns>
        public bool IsKeyPressed(Keys key) {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
        }
        // ...
    }
}