
namespace LightGive.StateController.Runtime
{
	/// <summary>
	/// IStateインターフェースの基底実装クラス
	/// コールバック機能を提供し、継承クラスが簡単にステート遷移を行えるようにします
	/// </summary>
	public abstract class StateBase : IState
	{
		/// <summary>
		/// 破棄
		/// </summary>
		public virtual void Dispose() { }

		/// <inheritdoc/>
		public virtual void Initialize() { }

		/// <inheritdoc/>
		public virtual void StateEnter() { }

		/// <inheritdoc/>
		public virtual void StateUpdate() { }

		/// <inheritdoc/>
		public virtual void StateExit() { }
	}
}