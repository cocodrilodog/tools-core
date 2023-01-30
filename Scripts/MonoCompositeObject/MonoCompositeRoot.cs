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
			EditorApplication.delayCall += () => OnParentReset(this);
#endif
		}

		#endregion


		#region Private Fields

		[HideInInspector]
		[SerializeField]
		private MonoCompositeObject m_SelectedMonoCompositeObject;

		#endregion


		#region Private Methods

		/// <summary>
		/// This handles the repeated references when a component is pasted via "Paste component as new".
		/// </summary>
		/// <param name="resetParent">The parent to reset</param>
		private void OnParentReset(IMonoCompositeParent resetParent) {
			Debug.Log($"OnOwnerReset: {resetParent}, {resetParent.GetChildren().Length}");
			foreach (var field in resetParent.GetChildren()) {
				var newParent = field.Recreate((resetParent as MonoBehaviour).gameObject);
				if (newParent != null) {
					// Recursion
					OnParentReset(newParent);
				}
			}
			resetParent.ConfirmChildren();
		}

		#endregion


	}

}