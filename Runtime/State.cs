using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightGive.StateController
{
	[System.Serializable]
	public abstract class State<T> where T : StateController<T>
	{
		protected T Controller;
		public State(T controller) { Controller = controller; }
		public abstract void OnStateEnter();
		public abstract void OnStateUpdate();
		public abstract void OnStateExit();
	}
}