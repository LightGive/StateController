
using System;

namespace LightGive.StateController.Runtime
{
	/// <summary>
	/// ステートマシンの各ステートが実装する必要があるインターフェース
	/// ステートの開始、更新、終了処理とコールバック設定機能を提供します
	/// </summary>
	public interface IState : IDisposable
	{
		/// <summary>
		/// <see cref = "StateController" >StateController</see>の<see cref = "StateController.Initialize(IState[], IState)" >Initialize</see>タイミングで呼ばれる初期化処理
		/// </summary>
		void Initialize();

		/// <summary>
		/// ステートが開始されるときに呼び出されます
		/// 初期化処理やステート開始時の処理を記述してください
		/// </summary>
		void StateEnter();

		/// <summary>
		/// ステートが有効な間、毎フレーム呼び出されます
		/// ステートの更新処理を記述してください
		/// </summary>
		void StateUpdate();

		/// <summary>
		/// ステートが終了するときに呼び出されます
		/// 終了処理やリソースの解放を記述してください
		/// </summary>
		void StateExit();
	}
}