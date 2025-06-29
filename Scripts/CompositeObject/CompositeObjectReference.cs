namespace CocodriloDog.Core {
	
	using System;
	using UnityEngine;

	/// <summary>
	/// An object that can search for and reference <see cref="CompositeObject"/>s of the default root 
	/// or the overriden root.
	/// </summary>
	/// 
	/// <remarks>
	/// For this system to work, the <see cref="CompositeObject"/> instance must be registered before
	/// <see cref="Value"/> is requested, normally on <c>Awake</c>. For the <see cref="CompositeObject"/>
	/// to be garbage collected, the instance must be unregistered, normally <c>OnDestroy</c>. For this purpose,
	/// you can use <see cref="CompositeObject.RegisterAsReferenceable(UnityEngine.Object)"/> and
	/// <see cref="CompositeObject.UnregisterReferenceable(UnityEngine.Object)"/>
	/// </remarks>
	/// 
	/// <typeparam name="T">The type of <see cref="CompositeObject"/> to look for</typeparam>
	[Serializable]
	public class CompositeObjectReference<T> where T : CompositeObject {


		#region Public Properties

		/// <summary>
		/// The referenced <see cref="CompositeObject"/>.
		/// </summary>
		public T Value {
			get {
				if (!Application.isPlaying) {
					Debug.LogWarning("CompositeObjectReference<T>.Vale only works at runtime.");
				}
				return ReferenceableCompositeObjects.GetById(m_Source, m_Id) as T;
			}
		}

		#endregion


		#region Private Fields

		[Tooltip("The root of the CompositeObject.")]
		[SerializeField]
		private UnityEngine.Object m_Source;

		[Tooltip("Allows to choose another object as the source root.")]
		[SerializeField]
		private bool m_OverrideSource;

		[Tooltip("The unique Id of the CompositeObject.")]
		[SerializeField]
		private string m_Id;

		#endregion


	}

}