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
		bool isInit = false;

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
		/// MonoBehaviourの初期化処理
		/// StateControllerの内部状態をリセットします
		/// </summary>
		void Awake()
		{
			isInit = false;
			_currentState = null;
		}

		/// <summary>
		/// 毎フレーム実行される処理
		/// 現在のステートのStateUpdateメソッドを呼び出します
		/// </summary>
		void Update()
		{
			if (isInit)
			{
				CurrentState?.StateUpdate();
			}
		}

		/// <summary>
		/// StateControllerを初期化し、ステートオブジェクトを登録します
		/// </summary>
		/// <param name="states">登録するステートオブジェクトの配列</param>
		/// <param name="initialState">初期ステートとして設定するオブジェクト</param>
		public void Initialize(IState[] states, IState initialState)
		{
			if (states == null ||
				states.Length == 0 ||
				initialState == null)
			{
				return;
			}

			var typeStateSets = new Dictionary<Type, IState>(states.Length);
			foreach (var state in states)
			{
				if (state == null)
				{
					Debug.LogWarning("Nullのステートを登録する事は出来ません");
					return;
				}

				var t = state.GetType();
				if (typeStateSets.ContainsKey(t))
				{
					Debug.LogError($"既に登録されているステートクラスです{t.Name}");
					return;
				}
				typeStateSets.Add(t, state);
			}

			if (!typeStateSets.Values.Contains(initialState))
			{
				Debug.LogWarning("初期ステートが存在しません");
				return;
			}

			_typeStateSets = new(typeStateSets);
			foreach (var state in _typeStateSets.Values)
			{
				state.SetStateChangeCallback(OnStateChangeRequested);
			}

			isInit = true;
			SetState(initialState);
		}

		/// <summary>
		/// 指定した型のステートに遷移します
		/// </summary>
		/// <typeparam name="T">遷移先のステート型（IStateを継承している必要があります）</typeparam>
		/// <returns>ステート遷移が成功した場合はtrue、失敗した場合はfalse</returns>
		public bool SetState<T>() where T : IState
		{
			if (!isInit)
			{
				return false;
			}

			if (_typeStateSets.TryGetValue(typeof(T), out var state))
			{
				SetState(state);
				return true;
			}
			else
			{
				Debug.LogWarning($"{typeof(T).Name}のステートが登録されていません");
				return false;
			}
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
			if (!isInit)
			{
				Debug.LogWarning("StateControllerが初期化されていません");
				return;
			}

			if (_typeStateSets.TryGetValue(stateType, out var state))
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