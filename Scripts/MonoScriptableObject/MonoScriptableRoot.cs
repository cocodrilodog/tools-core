namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	using UnityEngine;

	/// <summary>
	/// Base class for <c>MonoBehaviours</c> that will be the root object of the 
	/// <see cref="MonoScriptableObject"/> composite.
	/// </summary>
	public abstract class MonoScriptableRoot : MonoBehaviour, IMonoScriptableOwner {


		#region Public Properties

		/// <summary>
		/// The <see cref="MonoScriptableObject"/> that is currently selected. The editor
		/// of this object will replace the editor of the root object.
		/// </summary>
		public MonoScriptableObject SelectedMonoScriptableObject {
			get => m_SelectedMonoScriptableObject;
			set => m_SelectedMonoScriptableObject = value;
		}

		#endregion


		#region Public Methods

		public abstract MonoScriptableFieldBase[] GetMonoScriptableFields();

		public abstract void ConfirmOwnership();

		#endregion


		#region Unity Methods

		protected virtual void Reset() {
			Debug.Log($"This {this} was reset");
#if UNITY_EDITOR
			// Give time for the fields to point to non-null references.
			EditorApplication.delayCall += () => OnOwnerReset(this);
#endif
		}

		#endregion


		#region Private Fields

		[HideInInspector]
		[SerializeField]
		private MonoScriptableObject m_SelectedMonoScriptableObject;

		#endregion


		#region Private Methods

		private void OnOwnerReset(IMonoScriptableOwner resetOwner) {
			Debug.Log($"OnOwnerReset: {resetOwner}, {resetOwner.GetMonoScriptableFields().Length}");
			foreach (var field in resetOwner.GetMonoScriptableFields()) {
				var newOwner = field.Recreate((resetOwner as MonoBehaviour).gameObject);
				if (newOwner != null) {
					// Recursion
					OnOwnerReset(newOwner);
				}
			}
			resetOwner.ConfirmOwnership();
		}

		#endregion


	}

}