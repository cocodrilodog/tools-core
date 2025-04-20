namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This instantiates a cube.
	/// </summary>
	/// 
	/// <remarks>
	/// The goal of this example is to show how a non-scene controller can observe and subscribe to other objects 
	/// from different scenes.
	/// 
	/// Non-scene objects refers to objects in other scenes, objects instantiated at runtime, or objects
	/// that live in the DonDestroyOnLoad space.
	/// 
	/// Please see <see cref="NonSceneCubeController_Example1"/> and <see cref="NonSceneCubeController_Example2"/>
	/// to see two different patters that achieve this.
	/// </remarks>
	public class NonSceneObjectController_Example : MonoBehaviour {


		#region Private Fields

		[SerializeField]
		private GameObject m_NonSceneCubePrefab;

		#endregion


		#region private Unity Methods

		private void Awake() => Instantiate(m_NonSceneCubePrefab);

		#endregion


	}

}