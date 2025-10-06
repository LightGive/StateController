using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

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
		/// 登録されたステートオブジェクトを型別に管理する読み取り専用辞書
		/// </summary>
		ReadOnlyDictionary<Type, IState> _typeStateSets;

		/// <summary>
		/// 現在のステートを取得します
		/// </summary>
		/// <returns>現在アクティブなIStateオブジェクト。初期化前やステートが設定されていない場合はnull</returns>
		public IState CurrentState => _currentState;

		/// <summary>
		/// 登録されている全てのステートを取得します
		/// </summary>
		/// <returns>登録されているIStateのコレクション。初期化前の場合はnull</returns>
		public IEnumerable<IState> AllStates => _typeStateSets?.Values;

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
				Debug.LogWarning("ステート配列がnullまたは空です");
				return;
			}

			if (initialState == null)
			{
				Debug.LogWarning("初期ステートがnullです");
				return;
			}

			Dictionary<Type, IState> typeStateSets = new Dictionary<Type, IState>(states.Length);
			bool initialStateFound = false;

			foreach (IState state in states)
			{
				if (state == null)
				{
					Debug.LogWarning("Nullのステートを登録する事は出来ません");
					return;
				}

				Type stateType = state.GetType();
				if (typeStateSets.ContainsKey(stateType))
				{
					Debug.LogError($"既に登録されているステートクラスです: {stateType.Name}");
					return;
				}

				typeStateSets.Add(stateType, state);

				if (ReferenceEquals(state, initialState))
				{
					initialStateFound = true;
				}

				state.Initialize();
			}

			if (!initialStateFound)
			{
				Debug.LogWarning("初期ステートが登録されたステートの中に存在しません");
				return;
			}

			_typeStateSets = new ReadOnlyDictionary<Type, IState>(typeStateSets);

			foreach (IState state in _typeStateSets.Values)
			{
				state.SetStateChangeCallback(OnStateChangeRequested);
			}

			_isInitialized = true;
			SetState(initialState);
		}

		/// <summary>
		/// 指定した型のステートに遷移します
		/// </summary>
		/// <typeparam name="T">遷移先のステート型（IStateを継承している必要があります）</typeparam>
		/// <returns>ステート遷移が成功した場合はtrue、失敗した場合はfalse</returns>
		public bool SetState<T>() where T : IState
		{
			if (!_isInitialized)
			{
				Debug.LogWarning("StateControllerが初期化されていません");
				return false;
			}

			if (_typeStateSets.TryGetValue(typeof(T), out IState state))
			{
				SetState(state);
				return true;
			}

			Debug.LogWarning($"{typeof(T).Name}のステートが登録されていません");
			return false;
		}

		/// <summary>
		/// ステート遷移を実行します
		/// 現在のステートのStateExitを呼び出し、新しいステートのStateEnterを呼び出します
		/// </summary>
		/// <param name="state">遷移先のステートオブジェクト</param>
		void SetState(IState state)
		{
			if (_currentState != null)
			{
				_currentState.StateExit();
			}

			_currentState = state;
			_currentState.StateEnter();
		}

		/// <summary>
		/// ステートからの遷移要求を処理するコールバックメソッド
		/// StateBaseクラスのChangeStateメソッドから呼び出されます
		/// </summary>
		/// <param name="stateType">遷移先のステート型</param>
		void OnStateChangeRequested(Type stateType)
		{
			if (!_isInitialized)
			{
				Debug.LogWarning("StateControllerが初期化されていません");
				return;
			}

			if (_typeStateSets.TryGetValue(stateType, out IState state))
			{
				SetState(state);
			}
			else
			{
				Debug.LogWarning($"{stateType.Name}のステートが登録されていません");
			}
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