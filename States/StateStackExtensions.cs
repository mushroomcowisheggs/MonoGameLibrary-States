using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States;

/// <summary>
/// The extension methods of StateStack
/// </summary>
public static class StateStackExtensions {
    /// <summary>
    /// Safely invoke the Update method (handling null cases)
    /// </summary>
    /// <param name="stack">The state stack to be updated</param>
    /// <param name="gameTime">Game time</param>
    public static void SafeUpdate(this StateStack stack, GameTime gameTime, InputState inputState) {
        if (stack != null) {
            stack.Update(gameTime, inputState);
        }
    }

    /// <summary>
    /// Safely invoke the Draw method (handling null cases)
    /// </summary>
    /// <param name="stack">The state stack to be drawn</param>
    /// <param name="gameTime">Game time</param>
    /// <param name="spriteBatch">SpriteBatch instance</param>
    public static void SafeDraw(this StateStack stack, GameTime gameTime, SpriteBatch spriteBatch) {
        if (stack != null) {
            stack.Draw(gameTime, spriteBatch);
        }
    }
    
    /// <summary>
    /// Check whether the state stack is empty
    /// </summary>
    public static bool IsEmpty(this StateStack stack) {
        return stack == null || stack.CurrentState == null;
    }
    /// <summary>
    /// Get the current state type
    /// </summary>
    public static Type GetCurrentStateType(this StateStack stack) {
        if(stack != null && stack.CurrentState != null) {
            return stack.CurrentState.GetType();
        }
        else {
            return null;
        }
    }
    
    /// <summary>
    /// Check whether the current state is of the specified type
    /// </summary>
    public static bool IsInState<T>(this StateStack stack) where T : State {
        if (stack != null) {
            return stack.CurrentState is T;
        }
        else {
            return false;
        }
    }
}