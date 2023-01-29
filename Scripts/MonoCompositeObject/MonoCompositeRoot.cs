namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	using UnityEngine;

	/// <summary>
	/// Base class for <c>MonoBehaviours</c> that will be the root object of the 
	/// <see cref="MonoCompositeObject"/> composite.
	/// </summary>
	public abstract class MonoCompositeRoot : MonoBehaviour, IMonoCompositeParent {


		#region Public Properties

		/// <summary>
		/// The <see cref="MonoCompositeObject"/> that is currently selected. The editor
		/// of this object will replace the editor of the root object.
		/// </summary>
		public MonoCompositeObject SelectedMonoCompositeObject {
			get => m_SelectedMonoCompositeObject;
			set => m_SelectedMonoCompositeObject = value;
		}

		#endregion


		#region Public Methods

		public abstract MonoCompositeFieldBase[] GetChildren();

		public abstract void ConfirmChildren();

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
		private MonoCompositeObject m_SelectedMonoCompositeObject;

		#endregion


		#region Private Methods

		private void OnOwnerReset(IMonoCompositeParent resetOwner) {
			Debug.Log($"OnOwnerReset: {resetOwner}, {resetOwner.GetChildren().Length}");
			foreach (var field in resetOwner.GetChildren()) {
				var newOwner = field.Recreate((resetOwner as MonoBehaviour).gameObject);
				if (newOwner != null) {
					// Recursion
					OnOwnerReset(newOwner);
				}
			}
			resetOwner.ConfirmChildren();
		}

		#endregion


	}

}