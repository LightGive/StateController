using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightGive.StateController
{
	public class StateController<T> : MonoBehaviour where T : StateController<T>
	{
		State<T> _currentState;
		State<T> _nextState;

		public bool ChangeState(State<T> nextState)
		{
			bool bRet = _nextState == null;
			_nextState = nextState;
			return bRet;
		}

		void Update()
		{
			if (_nextState != null)
			{
				if (_currentState != null)
				{
					_currentState.OnStateExit();
				}
				_currentState = _nextState;
				_currentState.OnStateEnter();
				_nextState = null;
			}

			if (_currentState != null)
			{
				_currentState.OnStateUpdate();
			}
		}
	}
}