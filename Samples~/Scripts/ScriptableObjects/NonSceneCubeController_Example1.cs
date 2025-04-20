namespace CocodriloDog.Core.Examples {

	using UnityEngine;


	#region Small Types

	/// <summary>
	/// Use this interface to be implemented in <see cref="ScriptableNonSceneCubeController"/> which points to this 
	/// controller. That way buttons in the scene can add event callbacks on the <see cref="ScriptableNonSceneCubeController"/>.
	/// </summary>
	public interface INonSceneCubeController_Example1 {
		void RotateCubeButton_onClick();
	}

	#endregion

	/// <summary>
	/// This example illustrates how a non-scene controller can subscribe to objects in scenes.
	/// </summary>
	/// 
	/// <remarks>
	/// Non-scene objects refers to objects in other scenes, objects instantiated at runtime, or objects
	/// that live in the DonDestroyOnLoad space.
	///
	/// This controller handles the RotateCubeButton click event. In a normal scenario, a button in the
	/// scene could be hooked to this event, but this cube will be instantiated with code, so we need another solution.
	/// In this case we can create the interface above and implement it in the <see cref="ScriptableNonSceneCubeController"/>
	/// so that it can be used as a wrapper of this controller.
	/// 
	/// This is the simplest pattern (so far) to achieve this. There is another approach implemented in
	/// <see cref="NonSceneCubeController_Example2"/>.
	/// </remarks>
	public class NonSceneCubeController_Example1 : MonoBehaviour, INonSceneCubeController_Example1 {


		#region Event Handlers

		public void RotateCubeButton_onClick() {
			Debug.Log("Rotate");
			transform.rotation = Random.rotationUniform;
		}

		#endregion


	}

}