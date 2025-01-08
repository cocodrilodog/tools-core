namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class ScriptableFlowStateMachine_Example : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() {
			m_ScriptableFlowStateMachine.GetState(0).OnEnter += () => Debug.Log("C# Event: State1 Enter");
			m_ScriptableFlowStateMachine.GetState(1).OnEnter += () => Debug.Log("C# Event: State2 Enter");
			m_ScriptableFlowStateMachine.GetState(2).OnEnter += () => Debug.Log("C# Event: State3 Enter");
		}

		private void Start() {
			m_ScriptableFlowStateMachine.SetDefaultState();
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private ScriptableFlowStateMachine m_ScriptableFlowStateMachine;

		#endregion


	}

}