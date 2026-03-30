using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States;

/// <summary>
/// Manages a stack of game states, handling updates, drawing, and state transitions.
/// </summary>
public class StateStack {
    private readonly List<State> _states = new List<State>();
    private bool _isProcessing = false;
    private readonly Queue<Action> _operationQueue = new Queue<Action>();

    /// <summary>
    /// Gets the current state at the top of the stack.
    /// </summary>
    /// <returns>The top-most state, or null if the stack is empty.</returns>
    public State CurrentState { get { return _states.Count > 0 ? _states[_states.Count - 1] : null; } }

    /// <summary>
    /// Subscribes to the state's StateChangeRequested event.
    /// </summary>
    /// <param name="state">The state to subscribe.</param>
    private void SubscribeState(State state) {
        state.StateChangeRequested += OnStateChangeRequested;
    }

    /// <summary>
    /// Unsubscribes from the state's StateChangeRequested event.
    /// </summary>
    /// <param name="state">The state to unsubscribe.</param>
    private void UnsubscribeState(State state) {
        state.StateChangeRequested -= OnStateChangeRequested;
    }

    /// <summary>
    /// Handles state change requests from a state.
    /// </summary>
    /// <param name="sender">The state that raised the event.</param>
    /// <param name="e">Event arguments containing the requested change.</param>
    private void OnStateChangeRequested(object sender, StateChangeEventArgs e) {
        // Perform corresponding operations based on the event type (use queues to ensure thread safety)
        switch (e.ChangeType) {
            case StateChangeType.Push:
                Push(e.NewState);
                break;
            case StateChangeType.Pop:
                Pop();
                break;
            case StateChangeType.Change:
                Change(e.NewState);
                break;
        }
    }
    
    /// <summary>
    /// Pushes a new state onto the stack.
    /// </summary>
    /// <param name="state">The state to push.</param>
    public void Push(State state) {
        Action operation = delegate() {
            if (_states.Count > 0) {
                _states[_states.Count - 1].Exit();
            }
            SubscribeState(state);
            _states.Add(state);
            state.Enter();
        };
        QueueOrExecute(operation);
    }
    
    /// <summary>
    /// Pops the current state from the stack.
    /// </summary>
    public void Pop() {
        Action operation = delegate() {
            if (_states.Count > 0) {
                var topState = _states[_states.Count - 1];
                UnsubscribeState(topState);
                topState.Exit();
                _states.RemoveAt(_states.Count - 1);
            }

            if (_states.Count > 0) {
                _states[_states.Count - 1].Enter();
            }
        };
        QueueOrExecute(operation);
    }
    
    /// <summary>
    /// Replaces the entire stack with a single new state.
    /// </summary>
    /// <param name="state">The state to set as the new stack content.</param>
    public void Change(State state) {
        Action operation = delegate() {
            // Remove all states
            while (_states.Count > 0) {
                var topState = _states[_states.Count - 1];
                UnsubscribeState(topState);
                topState.Exit();
                _states.RemoveAt(_states.Count - 1);
            }

            SubscribeState(state);
            _states.Add(state);
            state.Enter();
        };
        QueueOrExecute(operation);
    }
    
    /// <summary>
    /// Queues an operation for later execution if currently processing updates; otherwise executes immediately.
    /// </summary>
    /// <param name="operation">The operation to queue or execute.</param>
    private void QueueOrExecute(Action operation) {
        if (_isProcessing) {
            _operationQueue.Enqueue(operation);
        }
        else {
            operation();
        }
    }
    
    /// <summary>
    /// Updates all active states from top to bottom, respecting blocking flags.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="inputState">The current input state.</param>
    public void Update(GameTime gameTime, InputState inputState)
    {
        _isProcessing = true;
        while (_operationQueue.Count > 0) {
            var operation = _operationQueue.Dequeue();
            if (operation != null) {
                operation.Invoke();
            }
        }
        
        if (_states.Count == 0) {
            return;
        }

        // Update from the top of the stack downward until a blocking state (flag == true) is encountered.
        for (int i = _states.Count - 1; i >= 0; i -= 1) {
            _states[i].HandleInput(gameTime, inputState);
            _states[i].Update(gameTime);
            if (_states[i].IsBlocking) {
                break;
            }
        }
        _isProcessing = false; // Ensure that the flag is true during the whole update process
    }
    
    /// <summary>
    /// Draws all states from bottom to top, stopping when a non-transparent state is encountered.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="spriteBatch">The SpriteBatch instance used for drawing.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        // Draw current state. Start drawing from the bottom of the stack upwards. Stop when encountering a non-transparent state (flag == false), which ensures that the non-transparent state (such as a full-screen menu) will cover all the content below it.
        bool _foundDrawOpaque = false;
        for (int i = 0; i < _states.Count; i += 1) {
            var state = _states[i];
            
            // If a non-transparent state has been found, the state below it will no longer be drawn (already has been completely covered), but the transparent state allows for further superimposition above it
            if (!_foundDrawOpaque || state.IsTransparent) {
                state.Draw(gameTime, spriteBatch);
            }
            
            if (!state.IsTransparent) {
                _foundDrawOpaque = true;
            }
        }
    }
}