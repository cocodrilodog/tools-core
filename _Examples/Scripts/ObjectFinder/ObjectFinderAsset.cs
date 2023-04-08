namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/Examples/Object Finder Asset")]
	public class ObjectFinderAsset : ScriptableObject {


		#region Private Fields

		[SerializeField]
		private ObjectFinder m_ObjectFinder;

		#endregion


	}

}