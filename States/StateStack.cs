using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.States;
public class StateStack
{
    private readonly List<State> _states = new List<State>();
    private bool _isProcessing = false;
    private readonly Queue<Action> _operationQueue = new Queue<Action>();

    public State CurrentState { get { return _states.Count > 0 ? _states[_states.Count - 1] : null; } }

    private void SubscribeState(State state)
    {
        state.StateChangeRequested += OnStateChangeRequested;
    }

    private void UnsubscribeState(State state)
    {
        state.StateChangeRequested -= OnStateChangeRequested;
    }

    private void OnStateChangeRequested(object sender, StateChangeEventArgs e)
    {
        // Perform corresponding operations based on the event type (use queues to ensure thread safety)
        switch (e.ChangeType)
        {
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

    public void Push(State state)
    {
        Action operation = delegate()
        {
            if (_states.Count > 0)
                _states[_states.Count - 1].Exit();

            SubscribeState(state);
            _states.Add(state);
            state.Enter();
        };
        QueueOrExecute(operation);
    }

    public void Pop()
    {
        Action operation = delegate()
        {
            if (_states.Count > 0)
            {
                var topState = _states[_states.Count - 1];
                UnsubscribeState(topState);
                topState.Exit();
                _states.RemoveAt(_states.Count - 1);
            }

            if (_states.Count > 0)
                _states[_states.Count - 1].Enter();
        };
        QueueOrExecute(operation);
    }

    public void Change(State state)
    {
        Action operation = delegate()
        {
            // Remove all states
            while (_states.Count > 0)
            {
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

    private void QueueOrExecute(Action operation)
    {
        if (_isProcessing)
            _operationQueue.Enqueue(operation);
        else
            operation();
    }

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
            if (_states[i].IsBlocking)
                break;
        }
        _isProcessing = false; // Ensure that the flag is true during the whole update process
    }

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