namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Example for the MonoScriptable systems.
    /// </summary>
    public class MonoScriptable_Example : MonoScriptableRoot, IMonoScriptableOwner {


		#region Public Fields

		[SerializeField]
        public MonoScriptableField_Example SomeScriptableField;

        [SerializeField]
        public MonoScriptableField_Example[] SomeScriptableFields;

		#endregion


		#region Public Methods

		public override MonoScriptableFieldBase[] GetMonoScriptableFields() {
			var fields = new List<MonoScriptableFieldBase>();
			if (SomeScriptableField != null) {
				fields.Add(SomeScriptableField);
			}
			if (SomeScriptableFields != null) {
				fields.AddRange(SomeScriptableFields);
			}
			return fields.ToArray();
		}

		public override void ConfirmOwnership() {
			foreach(var field in GetMonoScriptableFields()) {
				if (field.ObjectBase != null) {
					field.ObjectBase.SetOwner(this);
				}
			}
		}

		#endregion


	}

}