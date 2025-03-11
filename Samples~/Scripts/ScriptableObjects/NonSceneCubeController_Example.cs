namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// This example illustrates how a non-scene controller can subscribe to objects in scenes.
	/// 
	/// Non-scene objects refers to objects in other scenes, objects instantiated at runtime, or objects
	/// that live in the DonDestroyOnLoad space.
	/// 
	/// The subscription happens by using the events <see cref="ScriptableReference.OnInstanceReady"/>
	/// and <see cref="ScriptableReference.OnInstanceDiscard"/> which makes the process of subscribing when
	/// the value is ready and unsubscribing when the value is about to change to another instance.
	/// </summary>
	public class NonSceneCubeController_Example : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() {
			m_RotateCubeButton.OnInstanceReady += RotateCubeButton_OnInstanceReady;
			m_RotateCubeButton.OnInstanceDiscard += RotateCubeButton_OnInstanceDiscard;
		}

		private void OnDisable() {

			m_RotateCubeButton.OnInstanceReady -= RotateCubeButton_OnInstanceReady;
			m_RotateCubeButton.OnInstanceDiscard -= RotateCubeButton_OnInstanceDiscard;

			if (m_RotateCubeButton.Value != null) {
				m_RotateCubeButton.Value.onClick.RemoveListener(RotateCubeButton_onClick);
			}

		}

		#endregion


		#region Event Handlers

		private void RotateCubeButton_OnInstanceReady(Button unityObject) {
			Debug.Log($"Subscribed to {unityObject}!");
			unityObject.onClick.AddListener(RotateCubeButton_onClick);
		}

		private void RotateCubeButton_OnInstanceDiscard(Button unityObject) {
			Debug.Log($"Unsubscribed from {unityObject}!");
			unityObject.onClick.RemoveListener(RotateCubeButton_onClick);
		}

		private void RotateCubeButton_onClick() {
			Debug.Log("Rotate");
			transform.rotation = UnityEngine.Random.rotationUniform;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private ScriptableReferenceField<Button> m_RotateCubeButton;

		#endregion


	}

}