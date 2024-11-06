namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	/// An editor that can have buttons, by using the <see cref="ButtonAttribute"/> on methods.
	/// </summary>
	/// <remarks>
	/// It also implements <see cref="DrawProperty(UnityEditor.SerializedProperty)"/> so that specific
	/// properties can be overriden in subclasses.
	/// </remarks>
	[CustomEditor(typeof(MonoBehaviour), true)]
	public class EditorWithButtons : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			SetMethodsWithButton();
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
				if(m_MethodsWithButton.TryGetValue(index, out var method)){
					if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name))) {
						method.Invoke(target, null);
					}
				}
				index++;
			});

			serializedObject.ApplyModifiedProperties();

		}

		#endregion


		#region Protected Properties

		protected Dictionary<int, MethodInfo> MethodsWithButton => m_MethodsWithButton;

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

		private Dictionary<int, MethodInfo> m_MethodsWithButton;

		#endregion


		#region Private Methods

		private void SetMethodsWithButton() {

			// Store methods that have ButtonAttribute here
			var methods = new Dictionary<int, MethodInfo>();

			// Get all methods of the target object
			MethodInfo[] allMethods = target.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			// If the method has the ButtonAttribute, store it
			foreach (MethodInfo method in allMethods) {
				ButtonAttribute buttonAttribute = (ButtonAttribute)System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
				if (buttonAttribute != null) {
					methods[buttonAttribute.Index] = method;
				}
			}

			m_MethodsWithButton = methods;

		}

		#endregion


	}

}
