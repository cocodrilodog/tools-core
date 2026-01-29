namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// This is an example of how to extend the <see cref="StateMachineBase{T_State, T_Machine}"/>
	/// class.
	/// </summary>
	[AddComponentMenu("")]
	public class RotationMachine_Example : MonoCompositeStateMachine<RotationMachine_Example.State, RotationMachine_Example> {


		#region Public Properties

		public float Speed {
			get => m_Speed;
			set => m_Speed = value;
		} 

		#endregion


		#region Unity Methods

		protected override void Update() {
			base.Update();
			var eulerAngles = m_BallContainer.localEulerAngles;
			eulerAngles.z += Time.deltaTime * m_Speed;
			m_BallContainer.localEulerAngles = eulerAngles;
		}

		protected override void OnValidate() {

			base.OnValidate(); 

			// Create the states programatically
			CreateStateIfNull(typeof(Red), 0);
			CreateStateIfNull(typeof(Green), 1);
			CreateStateIfNull(typeof(Blue), 2);

			// don't allow to change the states in the inspector
			CanAddRemoveStates = false;
			CanReorderStates = false;

			// Since the internal functioning of this state machine relies on the name 
			// of the states, here we disable the ability of the Unity user to change their names.
			ForEachState(s => s.CanEditName = false);

		}

		#endregion


		#region Private Fields

		[SerializeField]
		private RectTransform m_BallContainer;

		[SerializeField]
		private float m_Speed = 3;

		#endregion


		#region States

		[Serializable]
		public abstract class State : MonoCompositeState<State, RotationMachine_Example> {

			[SerializeField]
			protected Color m_Color;

		}

		[Serializable]
		public class Red : State {

			public Red() => m_Color = Color.red;

			public override void Enter() {
				base.Enter();
				Machine.m_BallContainer.GetComponentInChildren<Image>().color = m_Color;
				Debug.Log("Red");
			}

			public override void Update() {
				base.Update();
				if(Machine.m_BallContainer.localEulerAngles.z >= 120) {
					TransitionToState("Green");
				} else if (Machine.m_BallContainer.localEulerAngles.z >= 240) {
					TransitionToState("Blue");
				}
			}

		}

		[Serializable]
		public class Green : State {

			public Green() => m_Color = Color.green;

			public override void Enter() {
				base.Enter();
				Machine.m_BallContainer.GetComponentInChildren<Image>().color = m_Color;
				Debug.Log("Green");
			}

			public override void Update() {
				base.Update();
				if (Machine.m_BallContainer.localEulerAngles.z >= 240) {
					TransitionToState("Blue");
				} else if (Machine.m_BallContainer.localEulerAngles.z < 120) {
					TransitionToState("Red");
				}
			}

		}

		[Serializable]
		public class Blue : State {

			public Blue() => m_Color = Color.blue;

			public override void Enter() {
				base.Enter();
				Machine.m_BallContainer.GetComponentInChildren<Image>().color = m_Color;
				Debug.Log("Blue");
			}

			public override void Update() {
				base.Update();
				if (Machine.m_BallContainer.localEulerAngles.z >= 0 &&
					Machine.m_BallContainer.localEulerAngles.z < 120) {
					TransitionToState("Red");
				} else if (Machine.m_BallContainer.localEulerAngles.z < 240) {
					TransitionToState("Green");
				}
			}

		}

		#endregion


	}

}
