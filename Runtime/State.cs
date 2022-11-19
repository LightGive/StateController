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
		public virtual void OnStateEnter() { }
		public virtual void OnStateUpdate() { }
		public virtual void OnStateExit() { }
	}
}