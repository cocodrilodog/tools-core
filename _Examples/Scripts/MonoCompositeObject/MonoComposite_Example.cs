namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Example for the MonoComposite systems.
    /// </summary>
    public class MonoComposite_Example : MonoCompositeRoot, IMonoCompositeParent {


		#region Public Fields

		[SerializeField]
        public MonoCompositeField_Example SomeScriptableField;

        [SerializeField]
        public MonoCompositeField_Example[] SomeScriptableFields;

		#endregion


		#region Public Methods

		public override MonoCompositeFieldBase[] GetChildren() {
			var fields = new List<MonoCompositeFieldBase>();
			if (SomeScriptableField != null) {
				fields.Add(SomeScriptableField);
			}
			if (SomeScriptableFields != null) {
				fields.AddRange(SomeScriptableFields);
			}
			return fields.ToArray();
		}

		public override void ConfirmChildren() {
			foreach(var field in GetChildren()) {
				if (field.ObjectBase != null) {
					field.ObjectBase.SetParent(this);
				}
			}
		}

		#endregion


	}

}