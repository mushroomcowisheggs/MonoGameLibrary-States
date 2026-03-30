using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States {
    
    /// <summary>
    /// Represents a game state that can be managed by a state stack.
    /// </summary>
    public abstract class State {
        
        /// <summary>
        /// Occurs when a state change (push, pop, or change) is requested.
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChangeRequested;
        
        /// <summary>
        /// Requests a new state to be pushed onto the state stack.
        /// </summary>
        /// <param name="newState">The state to push onto the stack.</param>
        protected void RequestPush(MonoGameLibrary.States.State newState) {
            if (StateChangeRequested != null) {
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Push, newState));
            }
        }
        
        /// <summary>
        /// Requests the current state to be popped from the state stack.
        /// </summary>
        protected void RequestPop() {
            if (StateChangeRequested != null) {
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Pop, null));
            }
        }
        
        /// <summary>
        /// Requests the entire state stack to be replaced with a new state.
        /// </summary>
        /// <param name="newState">The state to replace the stack with.</param>
        protected void RequestChange(State newState) {
            if (StateChangeRequested != null) {
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Change, newState));
            }
        }
        
        /// <summary>
        /// Handles input for this state. Can be overridden by derived classes.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="inputState">The current input state containing keyboard, gamepad, and mouse data.</param>
        public virtual void HandleInput(GameTime gameTime, InputState inputState) { }

        /// <summary>
        /// Gets a value indicating whether the state below this one should still be drawn.
        /// </summary>
        /// <remarks>
        /// When true, the lower state remains visible; when false, lower states are hidden.
        /// Default value is false.
        /// </remarks>
        public virtual bool IsTransparent { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether lower states should stop updating when this state is active.
        /// </summary>
        /// <remarks>
        /// When true, states below this one are not updated; when false, all states update.
        /// Default value is false.
        /// </remarks>
        public virtual bool IsBlocking { get { return false; } }

        /// <summary>
        /// Called when the state becomes active (entered onto the stack).
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called when the state becomes inactive (exited from the stack).
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Updates the state's logic.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draws the state using the provided SpriteBatch.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The SpriteBatch instance used for drawing.</param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        
        /// <summary>
        /// Gets a read-only identifier for this state.
        /// </summary>
        /// <remarks>
        /// By default, returns the type of the state instance.
        /// </remarks>
        public virtual object TypeId { get { return GetType(); } }
    }
}

/// <summary>
/// Provides data for state change events.
/// </summary>
public class StateChangeEventArgs : EventArgs {
    
    /// <summary>
    /// Gets the type of state change requested.
    /// </summary>
    public StateChangeType ChangeType { get; }
    
    /// <summary>
    /// Gets the new state to be used (if applicable).
    /// </summary>
    public MonoGameLibrary.States.State NewState { get; }
    
    /// <summary>
    /// Initializes a new instance of the StateChangeEventArgs class.
    /// </summary>
    /// <param name="changeType">The type of state change.</param>
    /// <param name="newState">The new state (if any).</param>
    public StateChangeEventArgs(StateChangeType changeType, MonoGameLibrary.States.State newState) {
        ChangeType = changeType;
        NewState = newState;
    }
}

/// <summary>
/// Specifies the type of operation to perform on the state stack.
/// </summary>
public enum StateChangeType {
    
    /// <summary>Push a new state onto the stack.</summary>
    Push,
    
    /// <summary>Pop the current state from the stack.</summary>
    Pop,
    
    /// <summary>Replace the entire stack with a new state.</summary>
    Change
}