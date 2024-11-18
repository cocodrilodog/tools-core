namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	/// An editor that supports the <see cref="ButtonAttribute"/> and implements 
	/// <see cref="DrawProperty(UnityEditor.SerializedProperty)"/> so that individual
	/// properties can be overriden in subclasses.
	/// </summary>
	public abstract class CDEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			m_MethodsWithButtonByIndex = MethodsWithButtonUtility.GetMethodsWithButtonByIndex(target.GetType());
		}

		public override void OnInspectorGUI() {

			int index = 0;

			serializedObject.Update();

			CDEditorUtility.IterateChildProperties(serializedObject, p => {
				if (p.propertyPath == "m_Script") {
					CDEditorUtility.DrawDisabledField(p);
				} else {
					DrawProperty(p);
				}
				if(m_MethodsWithButtonByIndex.TryGetValue(index, out var methods)){
					foreach(var method in methods) {
						if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name))) {
							method.Invoke(target, null);
						}
					}
				}
				index++;
			});

			serializedObject.ApplyModifiedProperties();

		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Draws each of the properties of this object.
		/// </summary>
		/// <remarks>
		/// Override this to modify the drawing of specific properties.
		/// </remarks>
		/// <param name="property">The property</param>
		protected virtual void DrawProperty(SerializedProperty property) {
			EditorGUILayout.PropertyField(property);
		}

		#endregion


		#region Private Fields

		private Dictionary<int, List<MethodInfo>> m_MethodsWithButtonByIndex;

		#endregion


	}

}
