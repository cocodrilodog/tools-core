namespace CocodriloDog.Core.Examples {
	using System;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

	/// <summary>
	/// Concrete property drawer example.
	/// </summary>
    [CustomPropertyDrawer(typeof(MonoScriptableField_Example))]
    public class MonoScriptableField_Example_PropertyDrawer : MonoScriptableFieldPropertyDrawer {


		#region Protected Properties

		protected override List<Type> MonoScriptableTypes {
			get {
                if (m_MonoScriptableTypes == null){
                    m_MonoScriptableTypes = new List<Type> { 
                        typeof(MonoSOE_1),
						typeof(MonoSOE_2),
					};
                }
                return m_MonoScriptableTypes;
			}
        }

		#endregion


		#region Private Fields

		private List<Type> m_MonoScriptableTypes;

		#endregion


	}

}