using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States
{
    public abstract class State
    {
        /// <summary>
        /// Used for requesting state stack operations
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChangeRequested;
        
        protected void RequestPush(MonoGameLibrary.States.State newState) {
            if (StateChangeRequested != null) {
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Push, newState));
            }
        }

        protected void RequestPop() {
            if (StateChangeRequested != null){
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Pop, null));
            }
        }
        
        protected void RequestChange(State newState) {
            if (StateChangeRequested != null){
                StateChangeRequested.Invoke(this, new StateChangeEventArgs(StateChangeType.Change, newState));
            }
        }
        
        /// <summary>
        /// Input Handling (optional rewrite)
        /// </summary>
        public virtual void HandleInput(GameTime gameTime, InputState inputState) { }

        /// <summary>
        /// IsTransparent: When it is true, the lower state is still drawn
        /// </summary>
        public virtual bool IsTransparent { get { return false; } }

        /// <summary>
        /// IsBlocking: When it is true, the lower layer stop to be updated
        /// </summary>
        public virtual bool IsBlocking { get { return false; } }

        /// <summary>
        /// Called when entering the state
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called when exiting the state
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Updating logic
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Drawing logic
        /// </summary>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        
        /// <summary>
        /// Read-only identification attribute, defaultly return type name
        /// </summary>
        public virtual object TypeId { get { return GetType(); } }
    }
}

public class StateChangeEventArgs : EventArgs {
    public StateChangeType ChangeType { get; }
    public MonoGameLibrary.States.State NewState { get; }
    public StateChangeEventArgs(StateChangeType changeType, MonoGameLibrary.States.State newState)
    {
        ChangeType = changeType;
        NewState = newState;
    }
}

public enum StateChangeType {
    Push,
    Pop,
    Change
}