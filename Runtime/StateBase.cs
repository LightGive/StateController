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
        /// ステート遷移要求のコールバックを設定します
        /// StateControllerのInitialize時に自動的に呼び出されます
        /// </summary>
        /// <param name="onStateChange">他のステートへの遷移要求を処理するコールバック</param>
        public virtual void SetStateChangeCallback(UnityAction<Type> onStateChange)
        {
            _onStateChange = onStateChange;
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

        /// <summary>
        /// ステートが開始されるときに呼び出されます
        /// 継承クラスで必要に応じてオーバーライドしてください
        /// </summary>
        public virtual void StateEnter() { }

        /// <summary>
        /// ステートが有効な間、毎フレーム呼び出されます
        /// 継承クラスで必要に応じてオーバーライドしてください
        /// </summary>
        public virtual void StateUpdate() { }

        /// <summary>
        /// ステートが終了するときに呼び出されます
        /// 継承クラスで必要に応じてオーバーライドしてください
        /// </summary>
        public virtual void StateExit() { }
    }
}