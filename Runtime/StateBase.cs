using System;
using UnityEngine.Events;

namespace LightGive.StateController.Runtime
{
    /// <summary>
    /// IStateインターフェースの基底実装クラス
    /// コールバック機能を提供し、継承クラスが簡単にステート遷移を行えるようにします
    /// </summary>
    public abstract class StateBase : IState
    {
        /// <summary>
        /// ステート遷移要求を処理するコールバック
        /// StateControllerによって設定されます
        /// </summary>
        protected UnityAction<Type> _onStateChange;

        /// <summary>
        /// 破棄
        /// </summary>
		public virtual void Dispose() { }

		/// <inheritdoc/>
		public virtual void SetStateChangeCallback(UnityAction<Type> onStateChange)
        {
            _onStateChange = onStateChange;
        }

		/// <inheritdoc/>
		public virtual void Initialize() 
		{ 
		}

		/// <inheritdoc/>
		public virtual void StateEnter() 
		{ 
		}

		/// <inheritdoc/>
		public virtual void StateUpdate() 
		{ 
		}

		/// <inheritdoc/>
		public virtual void StateExit() 
		{ 
		}

		/// <summary>
		/// 指定した型のステートへの遷移を要求します
		/// StateControllerに依存することなく他のステートに遷移できます
		/// </summary>
		/// <typeparam name="T">遷移先のステート型（IStateを継承している必要があります）</typeparam>
		protected void ChangeState<T>() where T : IState
		{
			_onStateChange?.Invoke(typeof(T));
		}

	}
}