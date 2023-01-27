namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Concrete MonoScriptableObject Example 2.
    /// </summary>
    public class MonoSOE_2 : MonoScriptableObject_Example, IMonoScriptableOwner {


		#region Public Fields

		[SerializeField]
        public float Prop2;

        [SerializeField]
        public MonoScriptableField_Example m_ChildMonoSOE;

		#endregion


		#region Public Methods

		public MonoScriptableFieldBase[] GetMonoScriptableFields() {
			return new MonoScriptableFieldBase[] { m_ChildMonoSOE };
		}

		public void ConfirmOwnership() {
			foreach (var field in GetMonoScriptableFields()) {
				if (field.ObjectBase != null) {
					field.ObjectBase.SetOwner(this);
				}
			}
		}

		#endregion


	}

}