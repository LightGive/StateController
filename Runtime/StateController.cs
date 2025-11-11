using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LightGive.StateController.Runtime
{
	/// <summary>
	/// ステートマシンを管理するコントローラークラス
	/// IStateインターフェースを実装したステートオブジェクトの登録、切り替え、更新を行います
	/// </summary>
	public class StateController : MonoBehaviour
	{
		/// <summary>
		/// 現在アクティブなステートオブジェクト
		/// </summary>
		[SerializeReference] IState _currentState = null;

		/// <summary>
		/// StateControllerが初期化済みかどうかのフラグ
		/// </summary>
		bool _isInitialized = false;

		/// <summary>
		/// ステートのリスト
		/// </summary>
		ReadOnlyCollection<IState> _stateList;

		/// <summary>
		/// 現在のステートを取得します
		/// </summary>
		/// <returns>現在アクティブなIStateオブジェクト。初期化前やステートが設定されていない場合はnull</returns>
		public IState CurrentState => _currentState;

		/// <summary>
		/// 登録されている全てのステートを取得します
		/// </summary>
		/// <returns>登録されているIStateのコレクション。初期化前の場合はnull</returns>
		public IEnumerable<IState> AllStates => _stateList;

		public UnityEvent<IState, IState> OnStateChangedEvt { get; private set; } = new();

		/// <summary>
		/// MonoBehaviourの初期化処理
		/// StateControllerの内部状態をリセットします
		/// </summary>
		void Awake()
		{
			_isInitialized = false;
			_currentState = null;
		}

		/// <summary>
		/// 毎フレーム実行される処理
		/// 現在のステートのStateUpdateメソッドを呼び出します
		/// </summary>
		void Update()
		{
			if (_isInitialized)
			{
				CurrentState?.StateUpdate();
			}
		}

		protected virtual void OnDestroy()
		{
			if (!_isInitialized)
			{
				return;
			}

			foreach (var state in AllStates)
			{
				state.Dispose();
			}
		}

		/// <summary>
		/// StateControllerを初期化し、ステートオブジェクトを登録します
		/// </summary>
		/// <param name="states">登録するステートオブジェクトの配列</param>
		/// <param name="initialState">初期ステートとして設定するオブジェクト</param>
		public void Initialize(IState[] states, IState initialState)
		{
			if (states == null || states.Length == 0)
			{
				Debug.LogError("ステート配列がnullまたは空です");
				return;
			}

			if (states.Any(x => x == null))
			{
				Debug.LogError("Nullのステートを登録する事は出来ません");
				return;
			}

			if (states.Length != states.Distinct().Count())
			{
				Debug.LogError("重複してステートを登録することは出来ません");
				return;
			}

			if (!states.Contains(initialState))
			{
				Debug.LogError("初期ステートが登録されたステートの中に存在しません");
				return;
			}

			_stateList = new ReadOnlyCollection<IState>(states.ToList());
			foreach (IState state in _stateList)
			{
				state.Initialize();
			}

			_isInitialized = true;
			SetStateImmediately(initialState);
		}

		/// <summary>
		/// ステート遷移を実行します
		/// 現在のステートのStateExitを呼び出し、新しいステートのStateEnterを呼び出します
		/// </summary>
		/// <param name="state">遷移先のステートオブジェクト</param>
		public bool SetState(IState state)
		{
			if(state == _currentState)
			{
				return false;
			}

			if (state == null)
			{
				return false;
			}

			if (!_isInitialized)
			{
				Debug.LogWarning("StateControllerが初期化されていません");
				return false;
			}

			if (AllStates.Contains(state))
			{
				SetStateImmediately(state);
				return true;
			}
			else
			{
				Debug.LogWarning($"{state}のステートが登録されていません");
				return false;
			}
		}

		void SetStateImmediately(IState state)
		{
			if (_currentState != null)
			{
				_currentState.StateExit();
			}

			var preState = _currentState;
			_currentState = state;
			_currentState.StateEnter();
			OnStateChangedEvt.Invoke(preState, _currentState);
		}

		/// <summary>
		/// 現在のステートが指定した型かどうかを判定します
		/// </summary>
		/// <typeparam name="T">判定したいステート型（IStateを継承している必要があります）</typeparam>
		/// <returns>現在のステートが指定した型の場合はtrue、それ以外はfalse</returns>
		public bool IsCurrentState<T>() where T : IState
		{
			return _currentState is T;
		}
	}
}