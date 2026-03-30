using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States;

/// <summary>
/// Provides extension methods for the StateStack class.
/// </summary>
public static class StateStackExtensions {
    /// <summary>
    /// Safely invokes the Update method, handling null stack references.
    /// </summary>
    /// <param name="stack">The state stack to update.</param>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="inputState">The current input state.</param>
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
    /// Checks whether the state stack is empty.
    /// </summary>
    /// <param name="stack">The state stack to check.</param>
    /// <returns>True if the stack is null or has no current state; otherwise false.</returns>
    public static bool IsEmpty(this StateStack stack) {
        return stack == null || stack.CurrentState == null;
    }
    
    /// <summary>
    /// Gets the type of the current state.
    /// </summary>
    /// <param name="stack">The state stack to query.</param>
    /// <returns>The type of the current state, or null if the stack is empty or null.</returns>
    public static Type GetCurrentStateType(this StateStack stack) {
        if (stack != null && stack.CurrentState != null) {
            return stack.CurrentState.GetType();
        }
        else {
            return null;
        }
    }
    
    /// <summary>
    /// Checks whether the current state is of the specified type.
    /// </summary>
    /// <typeparam name="T">The state type to check against.</typeparam>
    /// <param name="stack">The state stack to query.</param>
    /// <returns>True if the current state is of type T; otherwise false.</returns>
    public static bool IsInState<T>(this StateStack stack) where T : State {
        if (stack != null) {
            return stack.CurrentState is T;
        }
        else {
            return false;
        }
    }
}