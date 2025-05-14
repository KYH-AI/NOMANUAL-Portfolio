using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.StateMachine
{
    /// <summary>
    /// 재너릭 상태패턴 CRUD
    /// </summary>
    public class StateMachine<TState, TBaseState> where TBaseState : BaseState
    {
        public TBaseState CurrentState { get; private set; }
        private readonly Dictionary<TState, TBaseState> _states = new Dictionary<TState, TBaseState>();

        public StateMachine(TState initialState, TBaseState initialBaseState)
        {
            AddState(initialState, initialBaseState);
            CurrentState = GetState(initialState);
        }
        

        public void AddState(TState specialState, TBaseState newState)
        {
            if (!_states.ContainsKey(specialState))
            {
                _states.Add(specialState, newState);
            }
        }

        public void DeleteState(TState specialState)
        {
            if (_states.ContainsKey(specialState))
            {
                _states.Remove(specialState);
            }
        }

        public TBaseState GetState(TState specialState)
        {
            if (_states.TryGetValue(specialState, out TBaseState getState))
            {
                return getState;
            }
            
            //TODO : 오류 출력
            return null;
        }

        public void ChangeState(TState specialState)
        {
            CurrentState?.OnExitState();

            if (_states.TryGetValue(specialState, out TBaseState newState))
            {
                CurrentState = newState;
            }
            
            CurrentState?.OnEnterState();
        }

        public void UpdateState()
        {
            CurrentState?.OnUpdateState();
        }

        public void FixedUpdateState()
        {
            CurrentState?.OnFixedUpdateState();
        }
    }
}
