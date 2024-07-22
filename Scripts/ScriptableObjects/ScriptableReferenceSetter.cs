namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// A component that will assign the <see cref="m_ObjectToReference"/> object to the <see cref="m_Destination"/>
	/// <see cref="ScriptableReference"/>.
	/// </summary>
	/// <remarks>
	/// This allows to choose either the game object itself or the components attached to it as the source.
	/// </remarks>
    public class ScriptableReferenceSetter : MonoBehaviour {


		#region Unity Methods

		private void Awake() => m_Destination.Value = m_ObjectToReference;

		#endregion


		#region Private Fields

		[Tooltip("The game object or component that will be assigned to the ScriptableReference asset on Awake")]
		[ComponentOptions(nameof(ScriptableReferenceSetter))]
		[SerializeField]
		private Object m_ObjectToReference;

		[Tooltip(
			"A ScriptableReference asset that will receive a value of a this game object or any of its " +
			"components at runtime, so that other objects can read it from the asset."
		)]
		[SerializeField]
        private ScriptableReference m_Destination;

		#endregion


	}

}