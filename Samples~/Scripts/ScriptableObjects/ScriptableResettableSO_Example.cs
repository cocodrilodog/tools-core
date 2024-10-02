namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/Examples/Scriptable Edit State SO Example")]
	public class ScriptableResettableSO_Example : ScriptableResettable {


		#region Private Fields

		[SerializeField]
		public string SomeString;

		[SerializeField]
		public float SomeFloat;

		[SerializeField]
		public Vector3 SomePosition;

		[SerializeField]
		public ScriptableResettableSO_Example Another;

		#endregion


	}

}