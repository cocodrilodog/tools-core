namespace CocodriloDog.Core.Examples {

	using UnityEngine;

	[CreateAssetMenu(fileName = "ScriptableNonSceneCubeController", menuName = "Cocodrilo Dog/Core/Examples/ScriptableNonSceneCubeController")]
	public class ScriptableNonSceneCubeController : ScriptableReference, INonSceneCubeController_Example1 {


		#region Event Handlers

		public void RotateCubeButton_onClick() => (Value as NonSceneCubeController_Example1).RotateCubeButton_onClick();

		#endregion


	}

}