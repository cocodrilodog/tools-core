namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

	/// <summary>
	/// Concrete property drawer example.
	/// </summary>
    [CustomPropertyDrawer(typeof(MonoCompositeField_Example))]
    public class MonoCompositeField_Example_PropertyDrawer : MonoCompositeFieldPropertyDrawer {


		#region Protected Properties

		protected override List<Type> MonoCompositeTypes {
			get {
                if (m_MonoCompositeTypes == null){
                    m_MonoCompositeTypes = new List<Type> { 
                        typeof(MonoCOE_1),
						typeof(MonoCOE_2),
					};
                }
                return m_MonoCompositeTypes;
			}
        }

		#endregion


		#region Private Fields

		private List<Type> m_MonoCompositeTypes;

		#endregion


	}

}