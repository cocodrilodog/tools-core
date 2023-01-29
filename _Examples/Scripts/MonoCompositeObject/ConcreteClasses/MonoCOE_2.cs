namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// Concrete MonoCompositeObject Example 2.
	/// </summary>
	public class MonoCOE_2 : MonoCompositeObject_Example, IMonoCompositeParent {


		#region Public Fields

		[SerializeField]
        public float Prop2;

        [SerializeField]
        public MonoCompositeField_Example m_ChildMonoCOE;

		#endregion


		#region Public Methods

		public MonoCompositeFieldBase[] GetChildren() {
			return new MonoCompositeFieldBase[] { m_ChildMonoCOE };
		}

		public void ConfirmChildren() {
			foreach (var field in GetChildren()) {
				if (field.ObjectBase != null) {
					field.ObjectBase.SetParent(this);
				}
			}
		}

		#endregion


	}

}