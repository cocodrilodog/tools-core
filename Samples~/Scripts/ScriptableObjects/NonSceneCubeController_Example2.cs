namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// This example illustrates how a non-scene controller can subscribe to objects in scenes.
	/// </summary>
	/// 
	/// <remarks>
	/// Non-scene objects refers to objects in other scenes, objects instantiated at runtime, or objects
	/// that live in the DonDestroyOnLoad space.
	/// 
	/// The subscription happens by using the events <see cref="ScriptableReference.OnInstanceReady"/>
	/// and <see cref="ScriptableReference.OnInstanceDiscard"/> which makes the process of subscribing when
	/// the value is ready and unsubscribing when the value is about to change to another instance.
	/// 
	/// This is a very complex pattern, and is not the preferred one. The preferred one is the 
	/// <see cref="NonSceneCubeController_Example1"/>.
	/// 
	/// One advantage of this pattern is the ability to replace the instance that invoke the main event
	/// gracefully (in this case, the button click), but most of the real life cases could be handled 
	/// with the pattern described in <see cref="NonSceneCubeController_Example1"/>
	/// </remarks>
	public class NonSceneCubeController_Example2 : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() {

			if(m_RotateCubeButton.Value != null) {
				SubscribeToRotateCubeButton(m_RotateCubeButton.Value);
			}

			m_RotateCubeButton.OnInstanceReady += RotateCubeButton_OnInstanceReady;
			m_RotateCubeButton.OnInstanceDiscard += RotateCubeButton_OnInstanceDiscard;

		}

		private void OnDisable() {

			if (m_RotateCubeButton.Value != null) {
				UnsubscribeFromRotateCubeButton(m_RotateCubeButton.Value);
			}

			m_RotateCubeButton.OnInstanceReady -= RotateCubeButton_OnInstanceReady;
			m_RotateCubeButton.OnInstanceDiscard -= RotateCubeButton_OnInstanceDiscard;

		}

		#endregion


		#region Event Handlers

		private void RotateCubeButton_OnInstanceReady(Button unityObject) => SubscribeToRotateCubeButton(unityObject);

		private void RotateCubeButton_OnInstanceDiscard(Button unityObject) => UnsubscribeFromRotateCubeButton(unityObject);

		private void RotateCubeButton_onClick() {
			Debug.Log("Rotate");
			transform.rotation = UnityEngine.Random.rotationUniform;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private ScriptableReferenceField<Button> m_RotateCubeButton;

		#endregion


		#region Private Methods

		private void SubscribeToRotateCubeButton(Button button) {
			Debug.Log($"Subscribed to {button}!");
			button.onClick.AddListener(RotateCubeButton_onClick);
		}

		private void UnsubscribeFromRotateCubeButton(Button button) {
			Debug.Log($"Unsubscribed from {button}!");
			button.onClick.RemoveListener(RotateCubeButton_onClick);
		}

		#endregion


	}

}